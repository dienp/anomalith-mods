# Speed up weapon switching

## Why

Players want to fire sooner after selecting a different gun.

## What Changes

- Add a standalone Quick Weapon Switch mod that runs the player's normal firearm swap at twice its usual animation speed by default.
- Add a native ON/OFF control under Options → Other Settings with a saved preference.
- Add a native Switch Speed slider styled like Music Volume, with a saved multiplier from 1.0× to 3.0× in 0.1× steps.
- Let the first scroll notch cycle the equipped weapon while Quick Weapon Switch is ON.
- Keep the game's normal aim, fire, reload, and swap restrictions.

## Capabilities

### New Capabilities

- `quick-weapon-switch` speeds up player-initiated firearm swaps.

## Impact

Adds a BepInEx plugin with optional ScriptEngine hot reload for local development.
