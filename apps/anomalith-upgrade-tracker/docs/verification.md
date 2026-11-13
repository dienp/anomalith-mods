# Runtime verification

1. Hot-load the Release DLL through ScriptEngine and check the BepInEx log
   for the tracker startup message.
2. Highlight a revealed Workshop recipe with material requirements and press
   F7. Compare the pin confirmation with the selected recipe.
3. Enter a Dive. Check the displayed material shortages against the Workshop
   counts. F8 should hide and show the checklist.
4. Reload with ScriptEngine while the game is running. Confirm the same pin
   is restored and displayed with current inventory counts.

## Local result

The Release build hot-loaded through ScriptEngine 11.2 on BepInEx 5.4.23.6.
F7 pinned the Anti-Matter Rifle from Workshop Development. The Dive checklist
showed Everlasting Metal Fragment and High SUR Gun Parts as ready and Crystal
of Madness as `Need 1`. Those statuses matched the Workshop counts of 253/26,
252/5, and 0/1.

The first reload exposed an incomplete pin written by Unity's serializer.
The plugin now uses the game's Newtonsoft.Json assembly. A temporary probe
recovered the same material IDs from the live item catalog and saved the
complete recipe through the tracker's normal save path. With that probe
removed, ScriptEngine reloaded the tracker and it restored the pin and drew
the same checklist in the same running Dive.

## Pickup highlighting and Storage rows

- ScriptEngine 11.2 reloaded the Release DLL during a Dive without restarting
  ANOMALITH. The tracker restored the Anti-Matter Rifle pin.
- The HUD used the native Storage name and quantity fonts and divider sprite.
  The three rows showed `x 000`, `x 000`, and `x 001`. The visual check
  caught clipped heading text, which was corrected and checked in the Dive.
- F9 showed Yotsubadai Heights, Block E Roof for the Sentinel and HE Corp
  Building as a Dive Report zone.
- A real Crystal of Madness pickup on Block E Roof was highlighted. The
  active camera was about three meters from the pickup even though the
  player transform reported 39 meters in this area. The range check now
  uses the same camera origin as the visibility and obstruction checks.
- The temporary diagnostics were removed. The final installed tracker
  rendered the marker and restored the pin without them.

## Completed and built recipes

ScriptEngine loaded version 1.2.0 and restored the active Firing Rate pin. A
read-only check used the live game data and native Storage card to verify
that the ready Semi-Automatic Magnum displayed `Ready to build at the Workshop`
and the developed Anti-Matter Rifle displayed `Built`. Both cards had zero
ingredient rows and hid sources. The check also exercised a legacy pin and
verified that a returning material shortage restores the ingredient rows.
The temporary checker was removed, and ScriptEngine restored Firing Rate again.

## Deploy source hints

- ScriptEngine hot-loaded version 1.3.0 in the same ANOMALITH process and
  restored the Assault Rifle pin. Live inventory had 237 of 20 Everlasting
  Metal Fragments, 247 of 4 High SUR Gun Parts, and 1 of 2 Protective
  Equipment Fragments. Only Protective Equipment Fragments appeared on Deploy.
- At 1920×1080, Miyanohatacho Shopping Street showed an outlined Area 05 and
  an adjacent material callout. The source list showed Under the Overpass
  and identified it as a possible soldier drop. Yotsubadai Heights showed
  a map-level lead and HE Corp Building was listed as unavailable in this save.
- The native UI check confirmed the overlay graphics do not receive input
  and all label rectangles were on the Deploy canvas. A temporary high-SUR
  shortage exercised the possible-loot tiles. Clearing the test shortages
  hid the entire overlay. The actual pin stayed set to Assault Rifle.

## Workshop pin toggle and Deploy material rows

- Version 1.4.0 hot-loaded through ScriptEngine and restored the existing
  Assault Rifle pin. In the live Workshop, the control used the native
  `Button_on` sprite beside Required Materials. Unpinning changed the action
  to `Pin recipe` and pinning again restored `Unpin recipe` and the same pin.
- Screenshots at 1920×1080 verified the button placement and the Deploy
  layout. The Assault Rifle heading appeared over the Storage-style
  Protective Equipment Fragments row, with `x 001` right-aligned and the
  Under the Overpass source below. Area 05 kept its outline and adjacent
  material label. A live check verified the Storage divider, text bounds,
  and absence of input interception in the Deploy overlay.
- A reinforcement sample using the live item catalog produced
  `Revolver · Firing Rate` without changing the saved pin.

## Workshop button alignment

Version 1.4.1 hot-loaded through ScriptEngine. On the live Workshop canvas,
the right edge moved from x=855 on the inset ingredient row to x=875 on
the native materials panel. A geometry check verified both Pin and Unpin
at x=875, and the Attachment screen was visually checked at 1920×1080.
The Exposure Protection Suit pin persisted across reload.

## Pinned Item visibility

Version 1.5.0 hot-loaded in the Rail Yard and restored the Combat Backpack
pin. Other Settings displayed Pinned Item as a native ON/OFF selector.
After toggling it off, the Dive checklist was absent while the minimap
remained visible. The saved pin and OFF preference survived a hot reload.

## Workshop-style gameplay checklist

Version 1.6.0 hot-loaded in the Workshop and restored the Combat Backpack
pin. The checklist was absent in the Workshop menu. On returning to Base
gameplay at 1920×1080, the HUD showed the pinned name, Still needed heading,
and the Contactless Carpet icon and shortage count of `0/1`. The OV row
showed its native icon and `81840/1540`, matching the Workshop. The gameplay
panel used the same striped row treatment and had no pin button or empty rows.

## Category icon and Workshop proportions

Version 1.6.1 hot-loaded with the existing Combat Backpack pin. In the
1920×1080 Base HUD, the native Backpack tab icon appeared next to the
recipe name. Contactless Carpet displayed its larger native material icon
and a right-aligned orange `0/1` in a 618 × 44 row. The OV row was absent.

## Deploy styling and attachment fit

Version 1.7.0 hot-loaded while Yotsubadai Heights was selected on Deploy.
The Shotgun · Attack Power pin showed the native Shotgun and material
icons, a striped Communication Device Spring row, and an orange `0/1`.
The heading led directly into the material row. A runtime layout check
on the final build confirmed the source label, its available height, and
the count.

The live weapon catalog returned ten compatible revealed weapons for
Angled, including Shotgun. Assault Rifle and Prototype Submachine Gun
were identified as needing development. The attachment results were
checked in the running game through a temporary diagnostic. The Workshop
panel's placement was not visually checked during that run.

## Compatible weapon rows

Version 1.7.1 hot-reloaded with Angled highlighted. At 1920 × 1080, all
ten compatible weapons appeared on individual rows. Assault Rifle and
Prototype Submachine Gun appeared under Not developed. The panel cleared
the attachment preview, Required Materials, and Pin recipe.
## Communication Device Spring Deploy check

Version 1.7.2 built in Release and loaded automatically through ScriptEngine.
The startup log confirmed that the Firing Rate pin was restored. A read-only
check against the installed DLL resolved the Spring to `map_105`, and each of
the three shipped Area 05 encounter layouts contained one sniper spawner.
The Deploy screen and a live drop were not retested in this run.

## Shared enemy drop index

Version 1.8.0 hot reloaded while Shopping Street was open in Deploy.
With Shotgun · Firing Rate pinned, the overlay showed the Spring at
`0/1`, outlined Area 05, labeled the tile, and showed the game's
Unknown Trooper (Sniper) name in the details. The installed build's
lookup also returned the Sentinel on Block E Roof, distinct Self
variants in HE Corp Building, and multiple areas for Protective
Equipment Fragments. Only the Shotgun pin was checked in the live menu.
## BepInEx startup install

With the Release DLL in `BepInEx/plugins` and ScriptEngine outside the load
path, BepInEx 5.4.23.6 loaded Upgrade Tracker 1.8.0 at startup. The tracker
restored the Revolver Attack Power pin and displayed the Base checklist
with Theater Lamp at `0/1`.
