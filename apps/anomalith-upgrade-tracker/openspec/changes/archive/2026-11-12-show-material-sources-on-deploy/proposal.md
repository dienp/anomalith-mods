## Why

Players need to choose where to Dive for a pinned upgrade while looking at the Deploy map. The existing source panel is available only during a Dive.

## What Changes

- Flag available Deploy maps with sources for missing materials.
- Highlight discovered area tiles with area-level evidence and show nearby material names. Identify loot-table results as possible drops and show region-only records in the map summary.
- Clear hints as shortages resolve and preserve native Deploy controls and progression.

## Capabilities

### New Capabilities

### Modified Capabilities

- `upgrade-materials-tracking` adds material source hints to Deploy.

## Impact

The BepInEx tracker reads the native Deploy dialog, map graph, item tables and inventory. Existing game controls, saves, and settings remain owned by the game.
