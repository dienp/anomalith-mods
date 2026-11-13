## Context

The native Workshop loads Required Materials graphics and text styles from dialog prefabs. The tracker currently uses Storage styling during a Dive.

## Decisions

Read Workshop row assets from the game's dialog prefab and render them in a compact HUD. Use the same combined inventory count as the Workshop and look up the pinned step's current OV cost through its catalog. Use the native drop icons where the game has revealed them. Keep a legible fallback for missing assets or older pins with unknown OV costs.

Gate the HUD on the active Base or Dive state and normal gameplay visibility. Use the existing Pinned Item setting and F8 in both locations. Keep ground markers and F9 sources scoped to Dives.

## Risks / Trade-offs

UI assets may be unavailable while scenes initialize. Reuse existing view instances and defer or fall back during asset unavailability.
