using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AnomalithMinimap
{
    [BepInPlugin("labs.anomalith.minimap", "ANOMALITH Minimap", "1.3.0")]
    public sealed class MinimapPlugin : BaseUnityPlugin
    {
        private PauseMenuMap nativeMenu;
        private PauseMenu nativePause;
        private NativeMap native;
        private NativeMapPoints nativePoints;
        private readonly List<NativeMapPoints.Point> points = new List<NativeMapPoints.Point>();
        private MinimapView view;
        private string sceneName;
        private string mapId;
        private MapImageSetData map;
        private int requestedFloor = -1;
        private AsyncOperationHandle<Texture2D> texture;
        private bool boundTexture;
        private bool failedTexture;
        private float nextLookup;
        private float nextPointRefresh;
        private float mapRotation;
        private ConfigEntry<bool> showMap;
        private ConfigEntry<KeyboardShortcut> toggleKey;
        private OverlayOptions options;
        private float saveAfter = float.PositiveInfinity;
        private Task saveTask;

        private void Awake()
        {
            showMap = Config.Bind("General", "Enabled", true, "Show the minimap during gameplay.");
            toggleKey = Config.Bind("Controls", "Toggle", new KeyboardShortcut(KeyCode.F10),
                "Turn the minimap on or off.");
            Config.SaveOnConfigSet = false;
            Config.SettingChanged += ScheduleSave;
            options = new OverlayOptions(showMap, Logger);
            Logger.LogInfo($"Minimap {(showMap.Value ? "enabled" : "disabled")}. Toggle with {toggleKey.Value}.");
        }

        private void LateUpdate()
        {
            if (toggleKey.Value.IsDown())
                Toggle();
            options.Refresh();
            SaveWhenIdle();
            if (!showMap.Value)
            {
                view?.Hide();
                var currentBattle = BattleManager.Instance;
                if (requestedFloor != -1 && (currentBattle == null || currentBattle.GetCurrentMap != sceneName))
                    ClearFloor();
                return;
            }

            var battle = BattleManager.Instance;
            var pause = PauseMenu.Instance;
            var hud = UIManager.Instance;
            if (!CanShow(battle, pause, hud))
            {
                HideView();
                return;
            }

            var player = battle.GetCurrentPlayer();
            var position = player == null ? null : player.GetMainTransform();
            var mapManager = MapManager.Instance;
            if (position == null || mapManager == null || !EnsureView(pause))
            {
                HideView();
                return;
            }

            string currentScene = battle.GetCurrentMap;
            if (sceneName != currentScene)
            {
                sceneName = currentScene;
                string graphicId = battle.GetCurrentGraphicMapID;
                ClearFloor();
                mapId = graphicId;
                map = native.Find(graphicId);
                mapRotation = CSVParams.instance == null ? 0 : -CSVParams.instance.GetCSVParam(graphicId, "Rotate");
            }

            Vector3 worldPosition = position.position;
            int floor = NativeMap.FindFloor(mapManager.m_GetFloorPoint, worldPosition.y);
            if (map?.m_ImageIDList == null || floor >= map.m_ImageIDList.Count ||
                !TrySetFloor(mapManager, floor))
            {
                HideView();
                return;
            }

            Vector2 projected = native.Project(map, worldPosition);
            if (float.IsNaN(projected.x) || float.IsNaN(projected.y) ||
                Mathf.Abs(projected.x) > 0.5f || Mathf.Abs(projected.y) > 0.5f)
            {
                HideView();
                return;
            }
            bool refreshPoints = Time.unscaledTime >= nextPointRefresh;
            if (refreshPoints)
            {
                nativePoints.Collect(native, map, mapManager, floor, points);
                view.SetPoints(points);
                nextPointRefresh = Time.unscaledTime + 1f;
            }
            view.UpdateLocation(projected, -position.rotation.eulerAngles.y + mapRotation);
            if (refreshPoints)
                view.MatchHealthBarWidth(hud.Health);
        }

        private void Toggle()
        {
            showMap.Value = !showMap.Value;
            if (!showMap.Value)
                view?.Hide();
            else if (boundTexture)
                nextPointRefresh = Mathf.Max(nextPointRefresh, Time.unscaledTime + 0.2f);
        }

        private void ScheduleSave(object sender, SettingChangedEventArgs args)
        {
            saveAfter = Time.unscaledTime + 0.75f;
        }

        private void SaveWhenIdle()
        {
            if (Time.unscaledTime < saveAfter || (saveTask != null && !saveTask.IsCompleted))
                return;

            saveAfter = float.PositiveInfinity;
            var config = Config;
            var logger = Logger;
            saveTask = Task.Run(() => SaveConfig(config, logger));
        }

        private static void SaveConfig(ConfigFile config, ManualLogSource logger)
        {
            try { config.Save(); }
            catch (Exception error) { logger.LogWarning($"Could not save the minimap setting. {error.Message}"); }
        }

        private void HideView()
        {
            if (view?.IsVisible == true)
            {
                view.Hide();
                nextPointRefresh = 0;
            }
        }

        private static bool CanShow(BattleManager battle, PauseMenu pause, UIManager hud)
        {
            if (battle == null || pause == null || hud == null || Time.timeScale <= 0 ||
                battle.IsChangingArea || battle.IsOnDemo() || battle.IsCutScene() ||
                battle.IsPlayerInputStop() || BattleManager.IsStopInput() ||
                PauseMenu.IsOpen || pause.IsOpenProcess() || DialogManager.IsOpenDialog ||
                !hud.GetShowUI() || hud.HUDCanvas == null || !hud.HUDCanvas.isActiveAndEnabled)
                return false;

            var state = battle.GetCurretState();
            return state == BattleManager.BATTLE_STATE.ON_BASE || state == BattleManager.BATTLE_STATE.ON_AREA;
        }

        private bool EnsureView(PauseMenu pause)
        {
            if (nativePause == pause && nativeMenu != null && view != null)
                return true;

            var pauseMap = pause.GetTabUI(PauseMenu.TabType.Map) as PauseMenuMap;
            if (pauseMap == null)
                return false;
            var assets = NativeMap.TryRead(pauseMap);
            if (assets == null)
                return false;

            ClearFloor();
            view?.Destroy();
            nativePause = pause;
            nativeMenu = pauseMap;
            native = assets;
            nativePoints = new NativeMapPoints(pauseMap);
            sceneName = null;
            view = new MinimapView(transform, native);
            return true;
        }

        private bool TrySetFloor(MapManager manager, int floor)
        {
            if (requestedFloor != floor)
            {
                ClearFloor();
                requestedFloor = floor;
            }
            if (boundTexture)
                return true;
            if (failedTexture)
                return false;

            if (!texture.IsValid())
            {
                if (Time.unscaledTime < nextLookup)
                    return false;

                nextLookup = Time.unscaledTime + 0.5f;
                string imageId = map.m_ImageIDList[floor];
                if (string.IsNullOrEmpty(imageId))
                {
                    failedTexture = true;
                    return false;
                }
                var location = manager.GetMapImage(imageId);
                if (location == null)
                    return false;

                texture = Addressables.LoadAssetAsync<Texture2D>(location);
            }
            if (!texture.IsDone)
                return false;

            if (texture.Status != AsyncOperationStatus.Succeeded || texture.Result == null)
            {
                Logger.LogWarning($"Could not load the minimap image for {mapId}, floor index {floor}.");
                Addressables.Release(texture);
                texture = default;
                failedTexture = true;
                return false;
            }

            string floorName = map.m_ImageIDList.Count > 1 ? $"Floor {floor + 1}" : "";
            view.SetFloor(texture.Result, AreaName(mapId), floorName, mapRotation);
            boundTexture = true;
            Logger.LogInfo($"Loaded native minimap for {mapId}, floor index {floor} ({texture.Result.name}).");
            return true;
        }

        private static string AreaName(string id)
        {
            var strings = StringManager.instance;
            string name = strings == null ? "" : strings.GetMapName(id, true);
            return Regex.Replace(name ?? "", "<[^>]*>", "").Split('\n')
                .Select(line => line.Trim()).LastOrDefault(line => line.Length > 0) ?? id;
        }

        private void ClearFloor()
        {
            view?.ClearFloor();
            if (texture.IsValid())
                Addressables.Release(texture);
            texture = default;
            requestedFloor = -1;
            boundTexture = false;
            failedTexture = false;
            nextLookup = 0;
            nextPointRefresh = 0;
            points.Clear();
        }

        private void OnDestroy()
        {
            options?.Dispose();
            Config.SettingChanged -= ScheduleSave;
            saveTask?.GetAwaiter().GetResult();
            if (!float.IsPositiveInfinity(saveAfter))
                SaveConfig(Config, Logger);
            ClearFloor();
            view?.Destroy();
        }
    }
}
