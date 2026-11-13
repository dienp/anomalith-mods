# Retire the FOV mod

## Why

ANOMALITH has a native FOV setting. The standalone override duplicates it and can override players' saved preference when installed from the bundle.

## What changes

Remove the FOV plugin from the active source and release downloads. Publish a two-mod install ZIP and link it from the main README.

## Capabilities

### Modified Capabilities

- `gameplay-fov`: Retire the mod's FOV override.

## Impact

Affects the source tree, release downloads, and install documentation. Players who installed the old DLL must remove it to use the native setting.
