## Context

The current update moves and configures each point image every frame. It also rebuilds the map ID from the scene name and reruns the native quest queries every 0.4 seconds. The current full-panel frame image makes the view nearly opaque even behind the translucent viewport.

## Decisions

- Cache the native map ID by scene name and reuse the current pause map reference. Move the map and landmarks under one transform. Lay out marker images only when the native points refresh or the floor changes. Keep visibility checks and player tracking responsive.
- Refresh native landmarks once per second while visible and immediately when returning to a new floor or from a hidden state.
- Disable the HUD canvas to hide the map. Keep the current floor image and marker objects ready for an immediate resume in the same area. Release obsolete images on area or floor changes and on unload.
- Coalesce toggle preference writes and save off the game thread. Flush a pending setting on unload. Defer the next native landmark query briefly when a cached overlay is shown again.
- Use an outline-only frame and lower-opacity panel fills. Size the square from the native health gauge's full rectangle, align both right edges, and keep the bottom offset. Increase the scale of the floor plan by approximately 35%.
- Compare frame times with the minimap temporarily visible and hidden in the same scene. Measure the minimap update separately to avoid attributing unrelated frame-time variation to it. Exercise repeated toggles in the live subway areas and check that the texture stays loaded and the preference persists.
