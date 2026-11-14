using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace AnomalithAutoReload
{
    [BepInPlugin(PluginId, "ANOMALITH Auto Reload", "1.0.0")]
    public sealed class AutoReloadPlugin : BaseUnityPlugin
    {
        private const string PluginId = "labs.anomalith.autoreload";
        private static ConfigEntry<bool> autoReload;
        private Harmony harmony;

        private void Awake()
        {
            autoReload = Config.Bind("Gameplay", "Enabled", true,
                "Automatically start a normal reload when the active firearm runs empty.");
            var condition = AccessTools.DeclaredMethod(typeof(CancelDriver), "HasFlag",
                new[] { typeof(CancelCommands) });
            if (condition?.ReturnType != typeof(bool))
            {
                Logger.LogWarning("The native reload condition was not found in this game build.");
                return;
            }

            // ScriptEngine can create the replacement before destroying the old component.
            harmony = new Harmony($"{PluginId}.{GetInstanceID()}");
            try
            {
                var postfix = new HarmonyMethod(typeof(AutoReloadPlugin).GetMethod(nameof(AllowEmptyReload),
                    BindingFlags.Static | BindingFlags.NonPublic));
                harmony.Patch(condition, postfix: postfix);
                Logger.LogInfo($"Auto Reload loaded. Enabled: {autoReload.Value}.");
            }
            catch (Exception error)
            {
                harmony.UnpatchSelf();
                Logger.LogWarning($"Auto Reload could not attach to the native reload condition: {error.Message}");
            }
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

        private static void AllowEmptyReload(CancelDriver __instance, CancelCommands __0, ref bool __result)
        {
            if (__result || __0 != CancelCommands.RELOAD || autoReload?.Value != true)
                return;

            var battle = BattleManager.Instance;
            if (battle == null)
                return;

            var player = __instance.GetPlayer();
            if (player == null || player != battle.GetCurrentPlayer() || player.IsDead())
                return;

            var weapon = player.GetCurrentWeapon();
            if (weapon == null || weapon.GetWeaponType() == WeaponBase.WEAPON_TYPE.THROWABLE ||
                !weapon.IsEmpty() || weapon.GetFireCoolDown() > 0)
                return;

            if (battle.IsInputStop() || battle.IsChangingArea || battle.IsCutScene() ||
                BattleManager.IsStopInput() || PauseMenu.IsOpen ||
                (PauseMenu.Instance != null && PauseMenu.Instance.IsOpenProcess()) ||
                (DialogManager.Instance != null && DialogManager.IsOpenDialog))
                return;

            // Bolt-action weapons need their native cocking animation to finish first.
            if (weapon.CockingType == WeaponBase.COCKING_TYPE.USE &&
                (player._Anim == null || !player._Anim.GetCurrentAnimatorStateInfo(3).IsName("IDLE")))
                return;

            if (Inventory.Instance != null && SaveRootGame.Flg != null && player.CanReload())
                __result = true;
        }
    }
}
