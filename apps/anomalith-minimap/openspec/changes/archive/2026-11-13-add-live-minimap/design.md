## Context

The native pause map uses per-area floor textures, an orthographic camera definition, floor-height markers and a rotation from the map table. The native HUD exposes visibility, and the battle and dialog managers expose transitions and modal state.

## Decisions

- Read the native pause map's definitions and player icon. Independently acquire the current floor texture from the native Addressables location and release it on a floor change or unload. Use the same map camera orientation, orthographic scale, floor thresholds and area rotation as the pause map.
- Place a compact clipped view above the lower-right status HUD shown in the reference. Use the native title typeface and map arrow, a dark HUD surface and the localized map name. Center the view around the player and update from the native player transform each frame.
- Gate the view on the game's gameplay state, HUD, dialog, pause and cutscene signals. The minimap owns its objects and never opens, moves or configures the pause map.
