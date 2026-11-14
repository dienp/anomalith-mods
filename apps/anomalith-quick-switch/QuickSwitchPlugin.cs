using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace AnomalithQuickSwitch
{
    [BepInPlugin(PluginId, "ANOMALITH Quick Weapon Switch", "1.0.0")]
    public sealed class QuickSwitchPlugin : BaseUnityPlugin
    {
        private const string PluginId = "labs.anomalith.quickswitch";
        private static readonly WeaponShortcutUI.Arrow[] ScrollOrder =
        {
            WeaponShortcutUI.Arrow.Left, WeaponShortcutUI.Arrow.Up, WeaponShortcutUI.Arrow.Right
        };
        internal const int MinimumSpeed = 10;
        internal const int DefaultSpeed = 20;
        internal const int MaximumSpeed = 30;
        private static ConfigEntry<bool> quickSwitch;
        private static ConfigEntry<int> switchSpeed;
        private Harmony harmony;
        private SwitchOption option;

        private IEnumerator Start()
        {
            // ScriptEngine destroys the old component at the end of the frame.
            yield return null;
            quickSwitch = Config.Bind("Gameplay", "Enabled", true, "Speed up normal firearm swaps.");
            switchSpeed = Config.Bind("Gameplay", "SpeedTenths", DefaultSpeed, new ConfigDescription(
                "Weapon switch speed in tenths of the normal speed. 20 means 2.0×.",
                new AcceptableValueRange<int>(MinimumSpeed, MaximumSpeed)));
            harmony = new Harmony($"{PluginId}.{GetInstanceID()}");
            try
            {
                var swap = AccessTools.DeclaredMethod(typeof(PlayerCtrl), nameof(PlayerCtrl.ChangeWeapon), Type.EmptyTypes);
                harmony.Patch(swap, transpiler: new HarmonyMethod(
                    AccessTools.DeclaredMethod(typeof(QuickSwitchPlugin), nameof(PatchSwap))));
                option = new SwitchOption(quickSwitch, switchSpeed, Logger);
                Logger.LogInfo($"Quick Weapon Switch loaded. Enabled: {quickSwitch.Value}.");
            }
            catch (Exception error)
            {
                harmony.UnpatchSelf();
                Logger.LogWarning($"Could not attach Quick Weapon Switch. {error.Message}");
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
        }

        private static IEnumerable<CodeInstruction> PatchSwap(IEnumerable<CodeInstruction> instructions)
        {
            var code = instructions.ToList();
            var getter = AccessTools.DeclaredMethod(typeof(WeaponBase), nameof(WeaponBase.GetChangeSpd));
            int speedIndex = FindCall(code, getter);

            // Replace the speed lookup inside the voluntary swap only. The game still sets
            // GUN_CHANGE_SPD and runs the original transition and readiness checks.
            code[speedIndex].opcode = OpCodes.Ldarg_0;
            code[speedIndex].operand = null;
            code.Insert(speedIndex + 1, new CodeInstruction(OpCodes.Call,
                AccessTools.DeclaredMethod(typeof(QuickSwitchPlugin), nameof(GetSwapSpeed))));

            int scrollStart = FindCall(code, AccessTools.DeclaredMethod(
                typeof(SaveDataLoadout), nameof(SaveDataLoadout.GetCurrentWeaponArrow))) - 1;
            int scrollEnd = code.FindIndex(scrollStart + 1, instruction =>
                instruction.Calls(AccessTools.DeclaredMethod(typeof(CharaObjBase), "GetUp")));
            var select = AccessTools.DeclaredMethod(typeof(Inventory), nameof(Inventory.GetCurrentWeapon),
                new[] { typeof(WeaponShortcutUI.Arrow), typeof(bool) });
            if (scrollStart < 4 || scrollEnd <= scrollStart ||
                code[scrollStart - 4].opcode != OpCodes.Ldloc_1)
                throw new InvalidOperationException("The native scroll selection path has changed.");

            var scrollSelectors = Enumerable.Range(scrollStart, scrollEnd - scrollStart)
                .Where(index => code[index].Calls(select)).ToArray();
            if (scrollSelectors.Length != 4)
                throw new InvalidOperationException("Expected four shortcut lookups in the scroll path.");

            // The last two lookups are used when the wheel is closed. Substitute a weapon
            // directly instead of the lookup that opens the wheel on the first notch.
            var directSelect = AccessTools.DeclaredMethod(typeof(QuickSwitchPlugin), nameof(SelectWithWheelClosed));
            for (int i = scrollSelectors.Length - 1; i >= 2; i--)
            {
                int index = scrollSelectors[i];
                code[index].opcode = OpCodes.Ldarg_0;
                code[index].operand = null;
                code.Insert(index + 1, new CodeInstruction(OpCodes.Ldloc_1));
                code.Insert(index + 2, new CodeInstruction(OpCodes.Call, directSelect));
            }
            return code;
        }

        private static int FindCall(List<CodeInstruction> code, MethodInfo target)
        {
            int index = code.FindIndex(instruction => instruction.Calls(target));
            if (index < 0 || code.FindLastIndex(instruction => instruction.Calls(target)) != index)
                throw new InvalidOperationException($"Expected one {target?.Name} call in PlayerCtrl.ChangeWeapon.");
            return index;
        }

        private static WeaponBase SelectWithWheelClosed(Inventory inventory, WeaponShortcutUI.Arrow arrow,
            bool outside, PlayerCtrl player, float scroll)
        {
            var battle = BattleManager.Instance;
            if (quickSwitch.Value && battle != null && battle.GetCurrentPlayer() == player &&
                !DialogManager.IsOpenDialog)
                return GetScrollWeapon(inventory, player, scroll);

            return inventory.GetCurrentWeapon(arrow, outside);
        }

        private static WeaponBase GetScrollWeapon(Inventory inventory, PlayerCtrl player, float scroll)
        {
            var loadout = SaveRootGame.Loadout;
            if (loadout == null)
                return null;

            var current = player.GetCurrentWeapon();
            var point = loadout.GetCurrentWeaponPointData();
            int start = point == null ? -1 : Array.IndexOf(ScrollOrder, point.m_ArrowType);
            if (start >= 0 && point.m_IsOutSide)
                start += ScrollOrder.Length;

            // Match the game's inner then outer shortcut order in both directions.
            int slots = 2 * ScrollOrder.Length;
            int direction = scroll > 0 ? -1 : 1;
            int slot = start < 0 && direction < 0 ? 0 : start;
            for (int i = 0; i < slots; i++)
            {
                slot = (slot + direction + slots) % slots;
                var arrow = ScrollOrder[slot % ScrollOrder.Length];
                int layer = slot / ScrollOrder.Length;
                if (layer >= loadout.GetShortcutLoadoutCount(arrow))
                    continue;

                string id = loadout.GetShortcutLoadout(arrow, layer)?.GetWeaponID();
                if (string.IsNullOrEmpty(id))
                    continue;

                var weapon = inventory.GetWeapon(id);
                if (weapon != null && weapon != current)
                    return weapon;
            }
            return null;
        }

        private static float GetSwapSpeed(WeaponBase weapon, PlayerCtrl player)
        {
            float speed = weapon.GetChangeSpd();
            var battle = BattleManager.Instance;
            if (quickSwitch.Value && battle != null && battle.GetCurrentPlayer() == player &&
                weapon.GetWeaponType() != WeaponBase.WEAPON_TYPE.THROWABLE &&
                speed > 0 && !float.IsInfinity(speed))
                return speed * (switchSpeed.Value / 10f);

            return speed;
        }
    }
}
