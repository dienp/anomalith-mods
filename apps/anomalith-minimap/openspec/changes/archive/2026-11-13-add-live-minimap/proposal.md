## Why

The existing pause map interrupts play when the player only needs to check their position. A small HUD map can show that position during exploration.

## What Changes

- Show a live minimap above the lower-right HUD in the base and during Dives.
- Reuse the pause map's floor images, orientation, and player arrow.
- Hide the minimap during dialogs, cutscenes, menus, and area transitions.

## Capabilities

### New Capabilities

- `live-minimap` shows the current floor and player location during gameplay.

## Impact

This is a standalone BepInEx plugin loaded by ScriptEngine. It adds a read-only HUD view. Fast travel, pins, objectives, and extra markers are outside the first version.
