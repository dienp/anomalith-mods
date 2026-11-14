# Auto-reload empty weapons

## Why

ANOMALITH's native empty reload requires a trigger press while aiming. Players want a reload to start when the last round is spent.

## What Changes

- Add an enabled-by-default BepInEx mod that requests a reload for the active empty firearm.
- Preserve native reload timing, animations, ammo rules, and player actions.
- Support disabling in the mod config. Exclude topping off partial magazines, infinite ammo, and reloads outside active play.

## Capabilities

### New Capabilities

- `auto-reload`: Request a normal reload once the current firearm is empty and the player can reload.

## Impact

Adds one standalone plugin and an entry in the BepInEx release bundle.
