using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using UnityEngine;

namespace AnomalithUpgradeTracker
{
    [BepInPlugin("labs.anomalith.upgradetracker", "ANOMALITH Upgrade Tracker", "1.8.0")]
    public sealed class UpgradeTrackerPlugin : BaseUnityPlugin
    {
        private ConfigEntry<KeyboardShortcut> pinKey;
        private ConfigEntry<KeyboardShortcut> clearKey;
        private ConfigEntry<KeyboardShortcut> toggleKey;
        private ConfigEntry<KeyboardShortcut> sourcesKey;
        private ConfigEntry<bool> showPinnedItem;
        private ConfigEntry<string> savedPin;
        private FieldInfo currentSelector;
        private PinnedRecipe pin;

        private string[] materialNames;
        private string[] materialStatus;
        private int[] heldCounts;
        private readonly Dictionary<string, (string Name, int Count)> needed =
            new Dictionary<string, (string Name, int Count)>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> sourceCache = new Dictionary<string, string>(StringComparer.Ordinal);
        private string sourceArea;
        private readonly PickupHighlights highlights = new PickupHighlights();
        private WorkshopChecklistView checklistView;
        private DeploySourcesView deployView;
        private WorkshopPinView workshopView;
        private AttachmentCompatibilityView compatibilityView;
        private float nextWorkshopRefresh;
        private bool showDeploy;
        private float nextDeployRefresh;
        private bool showSources;
        private string sourcesText;
        private string reportedSources;
        private string completionStatus;
        private bool reportedPickup;
        private bool showChecklist;
        private bool reportedChecklist;
        private float nextRefresh;

        private GUIStyle titleStyle;
        private GUIStyle materialStyle;
        private GUIStyle statusStyle;

        private void Awake()
        {
            currentSelector = typeof(DevMenuBase).GetField("m_CurrentSelector",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (currentSelector?.FieldType != typeof(DevelopItemSelector))
            {
                Logger.LogError("Could not find the Workshop recipe selection in this game build.");
                enabled = false;
                return;
            }

            pinKey = Config.Bind("Controls", "Pin", new KeyboardShortcut(KeyCode.F7),
                "Pin or unpin the highlighted Workshop recipe.");
            clearKey = Config.Bind("Controls", "Clear", new KeyboardShortcut(KeyCode.F7, KeyCode.LeftShift),
                "Clear the pinned recipe.");
            toggleKey = Config.Bind("Controls", "ToggleChecklist", new KeyboardShortcut(KeyCode.F8),
                "Show or hide pinned recipe guidance during Base or Dive gameplay.");
            sourcesKey = Config.Bind("Controls", "ToggleSources", new KeyboardShortcut(KeyCode.F9),
                "Show or hide the material sources during a Dive.");
            savedPin = Config.Bind("Tracker", "PinnedRecipe", "",
                "The recipe captured in the Workshop.");
            showPinnedItem = Config.Bind("Tracker", "ShowPinnedItem", true,
                "Show the pinned recipe checklist, pickup highlights, and Deploy material hints.");

            if (!string.IsNullOrWhiteSpace(savedPin.Value))
            {
                try
                {
                    var restored = JsonConvert.DeserializeObject<PinnedRecipe>(savedPin.Value);
                    if (restored?.IsValid == true)
                    {
                        SetPin(restored);
                        Logger.LogInfo($"Restored pin: {pin.name}.");
                    }
                    else
                        Logger.LogWarning("The saved Workshop pin is incomplete. Pin a recipe to replace it.");
                }
                catch (JsonException error)
                {
                    Logger.LogWarning($"Could not read the saved Workshop pin. {error.Message}");
                }
            }

            Logger.LogInfo($"Ready. Toggle pin with {pinKey.Value}, checklist with {toggleKey.Value}, sources with {sourcesKey.Value}.");
        }

        private void Update()
        {
            if (clearKey.Value.IsDown() && pin != null)
                ClearPin();
            else if (pinKey.Value.IsDown())
                ToggleWorkshopPin();

            var battle = BattleManager.Instance;
            bool inDive = battle != null && battle.GetCurretState() == BattleManager.BATTLE_STATE.ON_AREA;
            bool activeGameplay = battle != null && (inDive ||
                battle.GetCurretState() == BattleManager.BATTLE_STATE.ON_BASE) &&
                !battle.IsChangingArea && !battle.IsOnDemo() && !battle.IsCutScene() &&
                !battle.IsPlayerInputStop() && !BattleManager.IsStopInput() && Time.timeScale > 0 &&
                !PauseMenu.IsOpen && (PauseMenu.Instance == null || !PauseMenu.Instance.IsOpenProcess()) &&
                (DialogManager.Instance == null || !DialogManager.IsOpenDialog);
            if (activeGameplay && toggleKey.Value.IsDown())
                showPinnedItem.Value = !showPinnedItem.Value;
            if (activeGameplay && inDive && sourcesKey.Value.IsDown())
            {
                showSources = !showSources;
                nextRefresh = 0;
            }

            var inventory = Inventory.Instance;
            var hud = UIManager.Instance;
            bool canShow = pin != null && showPinnedItem.Value && activeGameplay && inventory != null &&
                hud != null && hud.GetShowUI() && hud.HUDCanvas != null &&
                hud.HUDCanvas.isActiveAndEnabled;

            if (canShow && (!showChecklist || Time.unscaledTime >= nextRefresh))
            {
                RefreshChecklist(inventory);
                string status = pin.GetCompletionStatus(inventory, needed.Count == 0);
                if (status != completionStatus && !string.IsNullOrEmpty(status))
                    Logger.LogInfo($"{pin.name}: {status}");
                completionStatus = status;
                if (!inDive || !string.IsNullOrEmpty(status))
                    highlights.Clear();
                sourcesText = inDive && showSources && string.IsNullOrEmpty(status) ? BuildSources(battle.GetCurrentMapID) : "";
                if (checklistView == null)
                {
                    checklistView = WorkshopChecklistView.TryCreate(transform);
                    if (checklistView != null)
                        Logger.LogInfo("Pinned checklist is using the native Workshop material styles.");
                }
                checklistView?.Show(pin, materialNames, heldCounts, sourcesText, completionStatus);
                if (!string.IsNullOrEmpty(sourcesText) && reportedSources != sourcesText)
                {
                    reportedSources = sourcesText;
                    Logger.LogInfo("Material sources: " + sourcesText.Replace("\n", " | "));
                }
            }
            else if (!canShow && showChecklist)
            {
                checklistView?.Hide();
                highlights.Clear();
            }

            showChecklist = canShow;
        }

        private void LateUpdate()
        {
            if (showChecklist && string.IsNullOrEmpty(completionStatus) &&
                BattleManager.Instance != null && BattleManager.Instance.GetCurretState() == BattleManager.BATTLE_STATE.ON_AREA)
                highlights.Refresh(needed);
            RefreshWorkshop();
            RefreshDeploy();
        }

        private void RefreshDeploy()
        {
            var dialog = showPinnedItem.Value && pin != null && DialogManager.Instance != null && DialogManager.IsOpenDialog
                ? DialogManager.GetTopDialog() as PortalWarpDialog : null;
            if (dialog == null || !dialog.isActiveAndEnabled || dialog.IsDisable || Inventory.Instance == null)
            {
                if (showDeploy)
                    deployView?.Hide();
                showDeploy = false;
                return;
            }
            if (showDeploy && Time.unscaledTime < nextDeployRefresh)
                return;

            showDeploy = true;
            nextDeployRefresh = Time.unscaledTime + 0.25f;
            var inventory = Inventory.Instance;
            RefreshChecklist(inventory);
            if (needed.Count == 0 || pin.GetCompletionStatus(inventory, false) == PinnedRecipe.Built)
            {
                deployView?.Hide();
                return;
            }

            if (deployView?.IsFor(dialog) != true)
            {
                deployView?.Destroy();
                deployView = new DeploySourcesView(dialog);
            }
            deployView.Refresh(pin, materialNames, heldCounts);
        }

        private void OnDestroy()
        {
            checklistView?.Destroy();
            deployView?.Destroy();
            workshopView?.Destroy();
            compatibilityView?.Destroy();
        }

        private DevMenuBase GetWorkshopMenu()
        {
            var workshop = DevelopmentMenuManager.instance;
            if (workshop == null || !workshop.m_IsMenuOpen || DialogManager.Instance == null ||
                !DialogManager.IsOpenDialog)
                return null;

            var menu = DialogManager.GetTopDialog() as DevMenuBase;
            return menu == null || !menu.isActiveAndEnabled || menu.IsDisable || menu.IsDisableOperation
                ? null : menu;
        }

        private static PinnedRecipe.Kind RecipeKind(DevMenuBase menu)
        {
            return menu switch
            {
                WeaponDevMenu _ => PinnedRecipe.Kind.Weapon,
                WeaponReinforceMenu _ => PinnedRecipe.Kind.Reinforcement,
                ArmorDevMenu _ => PinnedRecipe.Kind.Armor,
                AttachmentDevMenu _ => PinnedRecipe.Kind.Attachment,
                _ => PinnedRecipe.Kind.Unknown
            };
        }

        private void RefreshWorkshop()
        {
            var menu = GetWorkshopMenu();
            var kind = RecipeKind(menu);
            if (kind == PinnedRecipe.Kind.Unknown)
            {
                workshopView?.Hide();
                compatibilityView?.Hide();
                nextWorkshopRefresh = 0;
                return;
            }
            if (Time.unscaledTime < nextWorkshopRefresh)
                return;
            nextWorkshopRefresh = Time.unscaledTime + 0.08f;

            var selected = currentSelector.GetValue(menu) as DevelopItemSelector;
            var recipe = selected == null ? null : selected.GetDevData();
            if (menu is AttachmentDevMenu attachmentMenu && recipe is AttachmentDevData attachment &&
                !attachment.IsSecletInfo())
            {
                if (compatibilityView?.IsFor(attachmentMenu) != true)
                {
                    compatibilityView?.Destroy();
                    compatibilityView = AttachmentCompatibilityView.TryCreate(attachmentMenu);
                }
                compatibilityView?.Show(attachment);
            }
            else
                compatibilityView?.Hide();
            bool canPin = PinnedRecipe.CanPin(recipe);
            bool isPinned = IsPinnedSelection(recipe, kind);
            if (!canPin && !isPinned)
            {
                workshopView?.Hide();
                return;
            }

            if (workshopView?.IsFor(menu) != true)
            {
                workshopView?.Destroy();
                workshopView = WorkshopPinView.TryCreate(menu, ToggleWorkshopPin);
            }
            workshopView?.Show(isPinned, pinKey.Value.ToString());
        }

        private bool IsPinnedSelection(DevelopmetData recipe, PinnedRecipe.Kind kind)
        {
            if (recipe == null || recipe.IsSecletInfo() || pin == null)
                return false;
            return pin.Matches(recipe, kind, !recipe.IsMaxDevLevel());
        }

        private void ToggleWorkshopPin()
        {
            var menu = GetWorkshopMenu();
            var kind = RecipeKind(menu);
            if (kind == PinnedRecipe.Kind.Unknown)
                return;

            var selected = currentSelector.GetValue(menu) as DevelopItemSelector;
            if (IsPinnedSelection(selected == null ? null : selected.GetDevData(), kind))
            {
                ClearPin();
                return;
            }
            var recipe = PinnedRecipe.ReadSelection(selected, kind);
            if (recipe == null)
                return;

            SavePin(recipe);
            var costs = pin.materials.Select(material => $"{Inventory.GetItemName(material.id)} ×{material.required}");
            Logger.LogInfo($"Pinned {pin.DisplayName}: {string.Join(", ", costs)}.");
        }

        private void ClearPin()
        {
            Logger.LogInfo($"Unpinned {pin.DisplayName}.");
            savedPin.Value = "";
            SetPin(null);
        }

        private void SavePin(PinnedRecipe recipe)
        {
            savedPin.Value = JsonConvert.SerializeObject(recipe);
            SetPin(recipe);
        }

        private void SetPin(PinnedRecipe recipe)
        {
            pin = recipe;
            int count = pin?.materials.Length ?? 0;
            materialNames = new string[count];
            materialStatus = new string[count];
            heldCounts = new int[count];
            needed.Clear();
            sourceCache.Clear();
            highlights.Clear();
            checklistView?.Hide();
            deployView?.Hide();
            sourcesText = "";
            reportedSources = null;
            completionStatus = "";
            reportedPickup = false;
            nextRefresh = 0;
            nextWorkshopRefresh = 0;
            showChecklist = false;
            showDeploy = false;
            reportedChecklist = false;
        }

        private void RefreshChecklist(Inventory inventory)
        {
            nextRefresh = Time.unscaledTime + 0.25f;
            needed.Clear();
            for (int index = 0; index < pin.materials.Length; index++)
            {
                var material = pin.materials[index];
                int held = inventory.GetTotalItemCount(material.id);
                int missing = Math.Max(0, material.required - held);
                string name = Inventory.GetItemName(material.id);
                materialNames[index] = string.IsNullOrWhiteSpace(name) ? material.id : name;
                heldCounts[index] = held;
                materialStatus[index] = missing == 0 ? "Ready" : $"Need {missing}";
                if (missing > 0)
                    needed[Inventory.ConvertItemID(material.id)] = (materialNames[index], missing);
            }
        }

        private string BuildSources(string mapId)
        {
            if (sourceArea != mapId)
            {
                sourceArea = mapId;
                sourceCache.Clear();
            }
            string currentSection = MaterialSources.CurrentSection(mapId);
            var lines = new List<string> { "Material sources\nCurrent area: " + currentSection };
            foreach (var material in needed)
            {
                if (!sourceCache.TryGetValue(material.Key, out string source))
                {
                    source = MaterialSources.Describe(material.Key, mapId);
                    sourceCache[material.Key] = source;
                }
                lines.Add(material.Value.Name + "\n" + source);
            }
            return string.Join("\n\n", lines);
        }

        private void OnGUI()
        {
            if (Event.current.type != EventType.Repaint || !showChecklist)
                return;

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    clipping = TextClipping.Clip
                };
                titleStyle.normal.textColor = new Color(0.90f, 0.97f, 0.95f);
                materialStyle = new GUIStyle(titleStyle)
                {
                    fontSize = 17,
                    fontStyle = FontStyle.Normal
                };
                statusStyle = new GUIStyle(materialStyle) { alignment = TextAnchor.MiddleRight };
            }

            float scale = Mathf.Clamp(Screen.height / 1080f, 0.65f, 2f);
            Matrix4x4 matrix = GUI.matrix;
            Color color = GUI.color;
            try
            {
                GUI.matrix = matrix * Matrix4x4.Scale(new Vector3(scale, scale, 1));
                float width = Mathf.Min(390f, Screen.width / scale - 40f);
                float left = Screen.width / scale - width - 20f;
                float top = 38f;

                foreach (var marker in highlights.Markers)
                {
                    float x = marker.Position.x / scale;
                    float y = marker.Position.y / scale;
                    float markerWidth = Mathf.Clamp(materialStyle.CalcSize(new GUIContent(marker.Name)).x + 22f, 88f, 310f);
                    float markerLeft = Mathf.Clamp(x - markerWidth * 0.5f, 4f, Screen.width / scale - markerWidth - 4f);
                    float markerTop = Mathf.Max(4f, y - 34f);
                    DrawBackground(new Rect(markerLeft, markerTop, markerWidth, 28f));
                    GUI.color = new Color(0.48f, 0.91f, 0.81f);
                    GUI.DrawTexture(new Rect(x - 2f, y - 3f, 4f, 4f), Texture2D.whiteTexture);
                    GUI.Label(new Rect(markerLeft + 8f, markerTop, markerWidth - 16f, 28f), marker.Name, materialStyle);
                    GUI.color = Color.white;
                    if (!reportedPickup)
                    {
                        reportedPickup = true;
                        Logger.LogInfo($"Highlighted needed pickup: {marker.Name} ({BattleManager.Instance.GetCurrentMapID}).");
                    }
                }

                if (checklistView == null)
                {
                    bool complete = !string.IsNullOrEmpty(completionStatus);
                    int rowCount = complete ? 0 : pin.materials.Length;
                    DrawBackground(new Rect(left, top, width, 65f + 25f * rowCount));
                    GUI.Label(new Rect(left + 14f, top + 8f, width - 28f, 28f), pin.DisplayName, titleStyle);
                    GUI.Label(new Rect(left + 14f, top + 35f, width - 28f, 20f),
                        complete ? completionStatus : "Still needed", materialStyle);
                    for (int index = 0; index < rowCount; index++)
                    {
                        float row = top + 54f + 25f * index;
                        GUI.Label(new Rect(left + 14f, row, width - 118f, 24f), materialNames[index], materialStyle);
                        GUI.Label(new Rect(left + width - 94f, row, 80f, 24f),
                            $"{heldCounts[index]}/{pin.materials[index].required}", statusStyle);
                    }
                }

                if (!reportedChecklist && string.IsNullOrEmpty(completionStatus))
                {
                    reportedChecklist = true;
                    var statuses = materialNames.Select((name, index) => $"{name} ({materialStatus[index]})");
                    Logger.LogInfo($"Pinned checklist for {pin.name}: {string.Join(", ", statuses)}.");
                }
            }
            finally
            {
                GUI.matrix = matrix;
                GUI.color = color;
            }
        }

        private static void DrawBackground(Rect panel)
        {
            GUI.color = new Color(0.045f, 0.09f, 0.105f, 0.92f);
            GUI.DrawTexture(panel, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
