## REMOVED Requirements

### Requirement: Configure gameplay field of view
**Reason**: ANOMALITH exposes a native FOV setting.
**Migration**: Remove `AnomalithFov.dll` and set the field of view in the game.

### Requirement: Preserve the native camera and save systems
**Reason**: The project is retiring the entire override.
**Migration**: Use the game's native FOV setting.
