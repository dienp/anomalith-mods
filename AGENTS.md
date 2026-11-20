# ANOMALITH mod development

## Mod settings

Every user-facing mod must provide its own ON/OFF toggle under
**Options → Other Settings**. Include this with each new mod and keep it
working when updating an existing mod.

- Use the game's native ON/OFF selector and a clear feature name.
- Persist the toggle in the mod's BepInEx configuration.
- Make the toggle available when the mod is installed by itself.
- Switching OFF must stop the mod's behavior and hide its UI. Switching
  ON must restore normal operation without restarting the game.
- Reopening Options or reloading the mod must preserve the setting and
  leave exactly one settings row. Clean up the row when unloading.
- Verify OFF, ON, persistence, and reload behavior before delivering.

## ANOMALITH playtesting

After a live ANOMALITH test that uses Windows screen capture, finish the
computer-use completion protocol and close the capture session before
reporting the result. For the unified computer-use tool, reset its session
with `cua_repl.js_reset` after submitting its completion summary. Confirm
that capture stopped using a non-capture check so the yellow border does
not stay on. Leave ANOMALITH running unless the user asks to close it.
