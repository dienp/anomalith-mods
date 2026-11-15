using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithStoragePreview
{
    [BepInPlugin(PluginId, "ANOMALITH Inventory Storage Preview", "1.0.0")]
    public sealed class StoragePreviewPlugin : BaseUnityPlugin
    {
        private const string PluginId = "labs.anomalith.storagepreview";
        private const string CountPrefix = "x ";
        private static readonly FieldInfo DetailText =
            AccessTools.DeclaredField(typeof(ItemPopUp), "m_InventoryPopUpItemDeteilText");
        private static readonly Dictionary<TextMeshProUGUI, Image> Icons = new Dictionary<TextMeshProUGUI, Image>();
        private static ConfigEntry<bool> enabledPreview;
        private Harmony harmony;
        private StoragePreviewOption option;

        private IEnumerator Start()
        {
            // Wait for ScriptEngine to remove the previous instance and its patch.
            yield return null;
            enabledPreview = Config.Bind("Inventory", "Enabled", true, "Show the Base Storage count in Inventory tooltips.");
            harmony = new Harmony($"{PluginId}.{GetInstanceID()}");
            try
            {
                var popup = AccessTools.DeclaredMethod(typeof(ItemPopUp), nameof(ItemPopUp.SetPopUp),
                    new[] { typeof(InventoryCellItemData) });
                if (popup == null || DetailText == null)
                    throw new InvalidOperationException("The Inventory tooltip is unavailable.");

                harmony.Patch(popup,
                    postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(StoragePreviewPlugin), nameof(PositionIcon))),
                    transpiler: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(StoragePreviewPlugin), nameof(AddCountBeforeLayout))));
                option = new StoragePreviewOption(enabledPreview, Logger);
                Logger.LogInfo("Inventory Storage Preview loaded.");
            }
            catch (Exception error)
            {
                harmony.UnpatchSelf();
                Logger.LogWarning($"Could not attach Inventory Storage Preview. {error.Message}");
            }
        }

        private void LateUpdate()
        {
            option?.Refresh();
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            option?.Dispose();
            foreach (var icon in Icons.Values)
            {
                if (icon != null)
                {
                    icon.enabled = false;
                    Destroy(icon.gameObject);
                }
            }
            Icons.Clear();
        }

        private static IEnumerable<CodeInstruction> AddCountBeforeLayout(IEnumerable<CodeInstruction> instructions)
        {
            var code = instructions.ToList();
            var layout = AccessTools.DeclaredMethod(typeof(Canvas), nameof(Canvas.ForceUpdateCanvases));
            int index = code.FindIndex(instruction => instruction.Calls(layout));
            if (index < 0 || code.FindLastIndex(instruction => instruction.Calls(layout)) != index)
                throw new InvalidOperationException("Expected one layout pass in the Inventory tooltip.");

            // The game has filled the description and will measure and clamp the full tooltip.
            code.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(code[index]),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call,
                    AccessTools.DeclaredMethod(typeof(StoragePreviewPlugin), nameof(AppendCount)))
            });
            return code;
        }

        private static void AppendCount(ItemPopUp popup, InventoryCellItemData item)
        {
            var description = DetailText.GetValue(popup) as TextMeshProUGUI;
            if (description == null)
                return;

            Icons.TryGetValue(description, out var icon);
            if (icon != null)
                icon.enabled = false;

            if (!enabledPreview.Value)
                return;

            var inventory = Inventory.Instance;
            if (item == null || inventory == null || SaveRootGame.Inventory?.Repository == null ||
                string.IsNullOrEmpty(item.GetItemID))
                return;

            int count = inventory.GetRepositoryItemCount(item.GetItemID);
            string quantity = $"{CountPrefix}{count:000}";
            var sprite = GetStorageSprite();
            if (!description.richText || sprite == null)
            {
                description.text = $"{description.text.TrimEnd()}\n\nIn Storage: {quantity}";
                return;
            }

            if (icon == null)
            {
                var iconObject = new GameObject("Storage count icon", typeof(RectTransform), typeof(Image));
                iconObject.transform.SetParent(description.transform, false);
                icon = iconObject.GetComponent<Image>();
                icon.preserveAspect = true;
                icon.raycastTarget = false;
                Icons[description] = icon;
            }

            icon.sprite = sprite;
            icon.color = new Color32(239, 251, 247, 255);
            icon.enabled = true;
            description.text = $"{description.text.TrimEnd()}\n<size=45%>\n</size>" +
                $"<space=2.65em><color=#EFFBF7><size=125%><b>{quantity}</b></size></color>";
        }

        private static Sprite GetStorageSprite()
        {
            var pause = PauseMenu.Instance;
            if (pause == null)
                return null;

            var tab = pause.GetTabList()?.Find(candidate => candidate.tabType == PauseMenu.TabType.Repository);
            var image = tab?.tabIcon != null ? tab.tabIcon.GetComponent<Image>() : null;
            return image != null ? image.overrideSprite : null;
        }

        private static void PositionIcon(ItemPopUp __instance, InventoryCellItemData __0)
        {
            var description = DetailText.GetValue(__instance) as TextMeshProUGUI;
            if (description == null || !Icons.TryGetValue(description, out var icon) || icon == null)
                return;

            if (__0 == null || !description.gameObject.activeInHierarchy)
            {
                icon.enabled = false;
                return;
            }
            if (!icon.enabled)
                return;

            var text = description.textInfo;
            int markerIndex = description.text.LastIndexOf(CountPrefix, StringComparison.Ordinal);
            for (int i = text.characterCount - 1; i >= 0; i--)
            {
                var marker = text.characterInfo[i];
                if (marker.index != markerIndex)
                    continue;

                float bottom = marker.bottomLeft.y;
                float top = marker.topLeft.y;
                for (int j = i + 1; j < text.characterCount; j++)
                {
                    var glyph = text.characterInfo[j];
                    if (!glyph.isVisible)
                        continue;

                    bottom = Mathf.Min(bottom, glyph.bottomLeft.y);
                    top = Mathf.Max(top, glyph.topLeft.y);
                }

                float size = description.fontSize * 1.65f;
                float gap = description.fontSize * 0.4f;
                var rect = icon.rectTransform;
                rect.sizeDelta = new Vector2(size, size);
                rect.localPosition = new Vector3(marker.origin - size * 0.5f - gap, (bottom + top) * 0.5f, 0);
                return;
            }
            icon.enabled = false;
        }
    }
}
