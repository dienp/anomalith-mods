using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AnomalithFov
{
    [BepInPlugin(PluginId, "ANOMALITH FOV", "1.0.0")]
    public sealed class FovPlugin : BaseUnityPlugin
    {
        private const string PluginId = "labs.anomalith.fov";

        private static int configuredFov;
        private static int nativeFov;
        private static bool reportedApplication;
        private static ManualLogSource log;

        private Harmony harmony;

        private void Awake()
        {
            var enabled = Config.Bind("Gameplay", "Enabled", true,
                "Apply the FOV override on game startup.");
            var degrees = Config.Bind("Gameplay", "VerticalFov", 65,
                new ConfigDescription("Vertical gameplay FOV in degrees. Restart to apply.",
                    new AcceptableValueRange<int>(40, 100)));

            if (!enabled.Value)
            {
                Logger.LogInfo("FOV override disabled. The game uses its own FOV setting.");
                return;
            }

            configuredFov = degrees.Value;
            reportedApplication = false;
            log = Logger;
            harmony = new Harmony(PluginId);

            try
            {
                harmony.Patch(
                    AccessTools.Method(typeof(CameraCtrl), nameof(CameraCtrl.SetFovVal)),
                    postfix: new HarmonyMethod(typeof(FovPlugin), nameof(ReportApplication)),
                    transpiler: new HarmonyMethod(typeof(FovPlugin), nameof(ReplaceFovRead)));
                Logger.LogInfo($"Gameplay FOV override ready ({configuredFov}° vertical).");
            }
            catch (Exception error)
            {
                harmony.UnpatchSelf();
                harmony = null;
                Logger.LogError($"Could not install the gameplay FOV override. {error}");
            }
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

        private static IEnumerable<CodeInstruction> ReplaceFovRead(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            var optionGetter = AccessTools.Method(typeof(SaveDataOption),
                nameof(SaveDataOption.GetOptionInfoValue), new[] { typeof(string) });
            int optionReadIndex = -1;

            for (int i = 1; i < code.Count; i++)
            {
                var previous = code[i - 1];
                if (previous.opcode != OpCodes.Ldstr || !Equals(previous.operand, "FOV_OPT")
                    || !code[i].Calls(optionGetter))
                    continue;

                if (optionReadIndex >= 0)
                    throw new InvalidOperationException("Found more than one FOV option read.");

                optionReadIndex = i;
            }

            if (optionReadIndex < 0)
                throw new InvalidOperationException("The game's FOV option read was not found.");

            code.Insert(optionReadIndex + 1,
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FovPlugin), nameof(OverrideFov))));
            return code;
        }

        private static int OverrideFov(int originalValue)
        {
            nativeFov = originalValue;
            return configuredFov;
        }

        private static void ReportApplication(CameraCtrl __instance)
        {
            if (reportedApplication || __instance.FOVChangeableCam == null)
                return;

            int cameraCount = 0;
            foreach (var camera in __instance.FOVChangeableCam)
            {
                if (camera == null)
                    continue;

                if (camera.m_Lens.FieldOfView != configuredFov)
                    return;

                cameraCount++;
            }

            if (cameraCount == 0)
                return;

            reportedApplication = true;
            log.LogInfo($"Verified {configuredFov}° vertical FOV on {cameraCount} gameplay camera lenses. Saved FOV option: {nativeFov}°.");
        }
    }
}
