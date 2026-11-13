# Proposal

## Why

After all materials are collected, the tracker should tell the player whether
the pinned upgrade can be built and make room on the Dive HUD.

## What Changes

- Replace the ingredient list and source details with a compact status once all material requirements are met.
- Use the Workshop's native buildability check before displaying a ready-to-build message.
- Show a built status when the pinned Workshop step has already been completed.
- Keep older pins readable and display a materials-ready status when their recipe cannot be identified.
- Exclude automatic development and changes to game saves or inventory.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `upgrade-materials-tracking` adds a compact completion status for the pinned upgrade.

## Impact

The existing tracker records the selected recipe identity and reads the live Workshop catalogs. The same hot-reload workflow deploys it.
