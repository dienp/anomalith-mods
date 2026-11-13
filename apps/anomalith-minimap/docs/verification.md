# Runtime verification

## Live location and visibility

- ScriptEngine loaded the Release DLL in the existing ANOMALITH process.
  The minimap used the Base floor image `map_000_kyoten` and the native
  `Map_PlayerIcon`. A 1920×1080 screenshot confirmed that the panel sits
  above the lower-right HUD. None of its graphics received input.
- The map offset changed as the player walked and the arrow turned with the
  character. The HUD position and native pause-map camera projection
  matched to six decimal places in viewport coordinates at the tested
  Base position.
- The live view hid during the pause map, a dialog and a scripted scene,
  and returned during exploration. The first live test used the Base
  floor. Floor selection elsewhere uses the game's own height thresholds.

## Larger map and native landmarks

- ScriptEngine loaded version 1.1.0 in the same process. The Base screenshot
  at 1920×1080 showed the 384 × 282 panel above the status bars, with a
  larger floor plan, the native player arrow, the main objective diamond,
  and native fast-travel dots.
- The main objective remained visible where it shared a location with a
  fast-travel point. The overlay had no input-receiving graphics.
- The live marker layout was checked in the Base. Other landmark types
  use the pause map's corresponding sprites, quest triggers and map objects.

## Health bar alignment, transparency and performance

- The updated map was visually checked in the Base at 1920×1080. It was
  about 261 pixels square, with both horizontal edges aligned to the full
  health bar. Scene details remained visible through the background.
  The floor image uses a 35% higher scale than version 1.1.0.
- In stable on/off/on runs, median minimap update time was 51–52 µs in
  1.1.0 at the Base and 18–20 µs in 1.2.0 in the Sewers. These timings
  cover the minimap callback only. Whole-game frame times varied across
  the runs and with the overlay off.
- The older toggle cleared and rebound the native image after each off/on
  cycle in `map_301_subway`. In 1.2.1, two off/on cycles on
  `map_302_subway` kept the same Floor 4 texture and showed or hid the
  canvas as requested. The saved preference matched each resting state.
  The re-enable handler measured 22–49 µs compared with 491–672 µs before
  the change. These short runs used different subway areas.

## Bottom-left placement

In the live Rail Yard Dive at 1920×1080, the hot-reloaded minimap was
approximately 261 pixels square, inset 27 pixels from the left and bottom
edges. The health bar on the opposite side was the same width.

## Other Settings

Version 1.3.0 hot-loaded in the Rail Yard with the upgrade tracker loaded.
Other Settings displayed Minimap and Pinned Item below Reset to Default.
Both used the game's ON/OFF labels and directional arrows. Switching both
off saved their settings. A second hot reload while Other Settings was
open restored both OFF rows without duplicates.
