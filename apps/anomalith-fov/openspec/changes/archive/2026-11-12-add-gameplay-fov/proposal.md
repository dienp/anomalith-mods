# Proposal

## Why

ANOMALITH players can use a wider gameplay view with a configurable BepInEx plugin for the local Windows build.

## What Changes

- Add a configurable vertical FOV override for the cameras already controlled by the game's FOV option.
- Allow the player to disable the override through the BepInEx configuration.
- Exclude a custom settings UI, changes to saved game options, and changes to other camera systems.

## Capabilities

### New Capabilities

- `gameplay-fov` provides a configurable override for the game's designated FOV cameras.

### Modified Capabilities

None.

## Impact

The plugin targets the Windows x64 Unity Mono build and uses BepInEx 5 with its bundled Harmony library. Its build references the local game and loader.
