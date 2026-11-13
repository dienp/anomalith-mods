# Proposal

## Why

Deploy only has a few hand-entered enemy sources. A recipe that needs any other enemy material often shows only a zone even when the game ships an exact encounter.

## What Changes

- Index enemy drops by item, area, and enemy variant using the installed game's encounter scenes and loot tables.
- Highlight discovered Deploy areas for missing materials in any pin and identify enemy drops as possible.
- Preserve zone hints for sources the index cannot locate. Exclude spawn guarantees and save-specific predictions.

## Capabilities

### Modified Capabilities

- `upgrade-materials-tracking`: Resolve exact enemy drop areas for any pin.

## Impact

Adds a reproducible offline indexing step and a compact embedded index. Deploy does not load encounter scenes.
