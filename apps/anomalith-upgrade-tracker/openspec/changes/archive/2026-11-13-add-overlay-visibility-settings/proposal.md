# Proposal

## Why

Let the player hide pinned recipe guidance without clearing the recipe.

## What Changes

Expose the Pinned Item ON/OFF preference in the native Other Settings integration provided by the accompanying minimap change. Use the same preference for F8. Apply it to Dive and Deploy guidance and keep the Workshop pin controls available.

## Capabilities

### New Capabilities

### Modified Capabilities

- `upgrade-materials-tracking` adds the Pinned Item visibility preference to Other Settings.

## Impact

This change ships with the related `add-overlay-visibility-settings` change in the minimap unit. The preferences live in BepInEx configuration and leave game save data and native settings intact.
