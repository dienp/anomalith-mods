# Proposal

## Why

The pinned recipe panel can cover the game's OV gain notification in the upper-right HUD.

## What Changes

- Hide the pinned recipe panel while the gameplay OV gain notification is active.
- Restore the same pin with current requirements after the notification finishes.
- Keep pickup highlights available and leave Workshop and Deploy behavior in place.

## Capabilities

### Modified Capabilities

- `upgrade-materials-tracking` defines gameplay checklist visibility while the OV notification is active.

## Impact

Upgrade Tracker reads the native MoneyUI notification state. No new settings or dependencies are needed.
