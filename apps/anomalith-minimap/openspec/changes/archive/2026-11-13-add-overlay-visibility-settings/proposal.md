# Proposal

## Why

Bring minimap visibility into the game's Options menu.

## What Changes

Reuse the native Other Settings selector used by SSAO. The Minimap row uses the existing persisted preference and shares its state with F10. The same integration displays the tracker row when the updated tracker is loaded.

## Capabilities

### New Capabilities

### Modified Capabilities

- `live-minimap` adds the Minimap visibility preference to Other Settings.

## Impact

This change ships with the related `add-overlay-visibility-settings` change in the upgrade tracker unit. The preferences live in BepInEx configuration and leave game save data and native settings intact.
