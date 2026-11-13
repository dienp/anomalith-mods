# Design

## Context

The installed Unity Mono build runs BepInEx 5.4.23.6. The managed body of `CameraCtrl.SetFovVal` reads `FOV_OPT` through `SaveDataOption.GetOptionInfoValue`, checks a cached value, and applies changes to `FOVChangeableCam`. `CameraCtrl.Update` calls this method.

## Goals / Non-Goals

Apply the configured value at the existing FOV update. Keep scope, event, and UI camera handling under the game's control.

## Decisions

- Use one Harmony patch on `CameraCtrl.SetFovVal`. Insert a call after the specific option read to substitute the configured value before the game performs its change check and lens updates. Patching the global option getter would affect other callers.
- Use BepInEx configuration for an enable switch and bounded integer FOV. Install the patch only when enabled at startup.
- Log one application per launch after reading back the live Cinemachine lens values. Include the original setting and applied value.
- Reference the installed assemblies through an MSBuild property. Copy the built plugin DLL into `BepInEx/plugins`, verify the enabled override in gameplay, and check that the disabled branch skips patch registration.

## Risks / Trade-offs

A game update may change the option read. The patch requires exactly one matching read and logs a startup failure if it cannot identify the hook. Disable the override in the configuration or remove the plugin DLL and restart to restore the native setting.
