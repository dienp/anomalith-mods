## Context

The Workshop uses category enums as indexes into the native tab sprite lists. Existing pins carry recipe identity, which allows category resolution without repinning.

## Decisions

Resolve the category from the recipe catalog, and use the parent weapon's category for reinforcement. Borrow the sprite from the corresponding native Workshop dialog prefab. Leave the heading readable if a legacy pin lacks identity or a sprite is unavailable.

Use the Workshop screenshot's row proportions for the HUD, keeping the current recipe heading and status. Keep the Workshop's OV check for build readiness and omit the currency breakdown from the HUD.
