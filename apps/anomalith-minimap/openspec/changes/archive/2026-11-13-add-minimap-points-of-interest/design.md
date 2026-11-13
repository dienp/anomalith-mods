## Context

The live view already shares the pause map's floor and projection. The pause map exposes icon templates and builds its point lists from the quest and map managers.

## Decisions

- Read native main and side quest triggers, mapped door and warp connections, portals, and available fast travel. Use the same floor threshold and map rotation as the floor plan. Use the game's fast-travel lock.
- Refresh point data while the minimap is visible. Reuse image objects and update their visible positions as the player moves. Draw the player arrow above all points. Prioritize objectives where map markers overlap.
- Reuse native sprites at a compact scale. Leave the markers read-only with no labels or navigation input.
- Use a 384 × 282 panel on the 1920 × 1080 HUD reference canvas. Enlarge the floor plan with it and keep the bottom and right offsets.
