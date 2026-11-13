# Runtime checks

1. Launch ANOMALITH with BepInEx and `Enabled = true`.
2. Load a save into gameplay. Check the loader log for the configured vertical
   FOV, the number of matching gameplay lenses, and the native saved value.
3. Exit the game. Set `Enabled = false` and restart. Load the same save and
   compare the camera view and the log, then restore the enabled setting.

## Local verification

- Release build passed against Unity `6000.3.18f1` and BepInEx `5.4.23.6`.
- BepInEx loaded ANOMALITH FOV 1.0.0 at 100°. The plugin read back 100° on
  17 gameplay lenses. A temporary read-only diagnostic measured 100° on the
  active Unity output camera after 30 stable frames in gameplay and read the
  original 90° option from the game.
- The Release assembly's disabled branch returns before installing the Harmony
  patch. The local gameplay run exercised the enabled configuration.
- The local config was set to 90° for the next launch, and the temporary
  diagnostic was removed from the game directory.
