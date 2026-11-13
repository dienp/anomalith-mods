# Proposal

## Why

Players need to remember which materials to bring back for a Workshop upgrade while exploring an Anomaly Zone.

## What Changes

- Pin one visible, material-based Workshop recipe at its current development or reinforcement level.
- Display live material shortages during a Dive using the game's bag and storage counts.
- Keep the pin through area transitions and ScriptEngine reloads.
- Exclude automatic crafting, item highlighting, currency and research requirements, and multiple pins.

## Capabilities

### New Capabilities

- `upgrade-materials-tracking` links a Workshop recipe to a live in-Dive checklist.

### Modified Capabilities

None.

## Impact

A standalone BepInEx plugin reads Workshop and inventory state from the local Unity Mono build. ScriptEngine loads the DLL, and the plugin stores its pin in its own configuration.
