# Proposal

## Why

The tracker shows the material shortfall. Players also need to recognize those pickups in a Dive and know which zones are worth visiting.

## What Changes

- Mark nearby, visible ground pickups that contain a material still needed for the pinned recipe.
- Reuse the Storage row visuals for item names, dividers, and remaining quantities.
- Show a material sources panel with the current section, documented zones, and specific sources verified from the installed build.
- Exclude unopened loot, off-screen markers, hidden loot, spawn changes, and guarantees about randomized drops.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `upgrade-materials-tracking` adds nearby pickup markers, source information, and native Storage row visuals.

## Impact

The existing ScriptEngine plugin reads live pickup objects, the game's occlusion mask, and native location records. It keeps the same saved pin format.
