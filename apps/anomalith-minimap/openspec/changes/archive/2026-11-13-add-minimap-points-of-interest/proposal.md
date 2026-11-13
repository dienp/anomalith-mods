## Why

Native map landmarks make it easier to identify the player's route and the next useful place to go without opening the pause map.

## What Changes

- Show active mission objectives, area connections, portals and available fast-travel points on the live minimap using the native icons.
- Keep markers on their current floor and keep the player arrow visible where points overlap.
- Enlarge the map and its floor plan while keeping the lower-right HUD placement.

## Capabilities

### Modified Capabilities

- `live-minimap` adds read-only point-of-interest markers.

## Impact

The minimap reads the same world and quest records as the pause map. The markers do not provide travel, interactions, or new objectives.
