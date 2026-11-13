## Why

The minimap currently repositions every landmark and rebuilds the native map ID every frame. The player also wants an on/off control and a more transparent panel.

## What Changes

- Move the floor plan and its landmarks together, and refresh landmark data less often.
- Add a persistent minimap toggle with a configurable F10 shortcut.
- Keep the current floor image ready while hidden and defer preference writes so repeated toggles avoid asset reloads and disk I/O on the input frame.
- Reduce the panel's background opacity while keeping the map and markers readable.
- Make the panel a compact square and zoom in on the floor plan.

## Capabilities

### Modified Capabilities

- `live-minimap` adds an optional HUD overlay and reduces repeated work while preserving movement and visibility behavior.

## Impact

Only the standalone minimap plugin and its own configuration change. The game and its pause map keep their existing controls and settings.
