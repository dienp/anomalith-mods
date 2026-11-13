# upgrade-materials-tracking Specification

## Purpose
Carry the material requirements for one selected Workshop upgrade into a Dive.

## Requirements

### Requirement: Pin one Workshop upgrade

The tracker SHALL provide a visible pin toggle beside Required Materials for a revealed Workshop recipe with material costs. The toggle's right edge SHALL align with the native materials panel's right edge. Clicking it or pressing the configured pin key SHALL toggle the current recipe step. Pinning a different recipe or step SHALL replace the previous pin.

#### Scenario: Pin a Workshop recipe

- **WHEN** the player uses the Pin action on a revealed recipe with material costs and a development level still available
- **THEN** the tracker captures the recipe identity, current step, and material requirements and changes the action to Unpin

#### Scenario: Unpin the displayed recipe

- **WHEN** the player uses the Unpin action on the current pin
- **THEN** the tracker clears the pin and the button returns to Pin where another pin can be made

#### Scenario: Ineligible selection

- **WHEN** the highlighted recipe is hidden, fully developed, has no material costs, or is outside a supported Workshop menu
- **THEN** the tracker offers no new Pin action and preserves any existing pin unless the player clears it

#### Scenario: Clear a pin

- **WHEN** the player presses Shift+F7 with the default key configuration
- **THEN** the tracker clears the pin and its checklist

#### Scenario: Align both button states

- **WHEN** the player views a recipe with either the Pin or Unpin action visible
- **THEN** the action's right edge is flush with the outer right edge of the Required Materials panel

### Requirement: Display pinned requirements during gameplay

The tracker SHALL show the pinned recipe and live Workshop-style requirements during active Base and Dive gameplay. The heading SHALL include the native Workshop category icon when available. Rows SHALL match Workshop proportions and show native material icons, combined bag and storage counts, and distinct shortage colors. The checklist SHALL omit empty rows, pin controls, and the OV row. The Pinned Item setting and F8 SHALL control its visibility in both locations.

#### Scenario: Read a shortfall

- **WHEN** the player is walking at Base with the tracker enabled and owns one of the three required materials
- **THEN** the pinned recipe name and its material row appear, with the count `1/3` styled as a shortage

#### Scenario: Reach a material requirement

- **WHEN** the player meets one material requirement and another remains unfulfilled
- **THEN** the satisfied material row shows its current owned/required count in the normal color

#### Scenario: Read the OV requirement

- **WHEN** the requirements are visible for a step with an OV cost
- **THEN** the gameplay checklist omits the OV row and shows a short status if OV is the remaining blocker

#### Scenario: Leave active Dive gameplay

- **WHEN** a menu, dialog, cutscene, transition, title screen or unsupported gameplay state is active
- **THEN** the gameplay checklist is hidden

#### Scenario: Return to Base

- **WHEN** the player returns from a Dive and resumes active Base gameplay
- **THEN** the checklist reflects current inventory counts and the same pinned recipe

#### Scenario: Read the Combat Backpack category

- **WHEN** the tracker displays an existing Combat Backpack pin
- **THEN** the Workshop's Backpack icon appears beside its name

### Requirement: Preserve the pin across ScriptEngine reload

The tracker SHALL store its single pinned recipe in its own local configuration and restore it after a ScriptEngine reload. The tracker SHALL read game state without writing to inventory, game settings, or saves.

#### Scenario: Reload with a pin

- **WHEN** ScriptEngine reloads the plugin after a recipe has been pinned
- **THEN** the reloaded instance retains the recipe and obtains current counts when the checklist is next shown

### Requirement: Highlight nearby needed pickups

The tracker SHALL mark nearby, on-screen, unobstructed ground pickups containing a material with a positive shortfall while the Dive checklist is enabled.

#### Scenario: Spot a needed pickup

- **WHEN** a spawned ground pickup within 30 meters of the active gameplay camera is visible and contains a needed material
- **THEN** the tracker labels the pickup with the material's name

#### Scenario: Stop marking an ineligible pickup

- **WHEN** a pickup disappears, becomes obstructed, leaves range, or its material is no longer needed
- **THEN** the tracker stops marking it

### Requirement: Show material source information

F9 SHALL toggle a panel listing the current map section and the game's documented zones for outstanding materials. A verified encounter or section SHALL be shown where available, and a documented zone alone SHALL be presented as a broader lead.

#### Scenario: Look up Crystal of Madness

- **WHEN** Crystal of Madness is still needed and the player opens the sources panel
- **THEN** the tracker lists the Sentinel encounter at Yotsubadai Heights, Block E Roof and shows HE Corp Building as an additional documented zone

#### Scenario: Material without an area record

- **WHEN** the game provides no usable location record for an outstanding material
- **THEN** the tracker indicates that no zone is listed

### Requirement: Summarize a completed material checklist

When all material requirements are met or the pinned step is already developed, the tracker SHALL replace ingredient rows, pickup markers, and source details with a compact status. It SHALL show ready to build only when the native Workshop check passes for the pinned recipe step.

#### Scenario: Ready to build

- **WHEN** the last required material is obtained and the Workshop check passes
- **THEN** the tracker shows the recipe name and `Ready to build at the Workshop` without ingredient rows

#### Scenario: Workshop requirement still outstanding

- **WHEN** all materials are held and the Workshop check fails
- **THEN** the tracker shows a compact status identifying a known blocker or directing the player to the Workshop

#### Scenario: Older pin without recipe identity

- **WHEN** all materials are held for an older pin whose recipe identity was never recorded
- **THEN** the tracker shows materials ready and prompts the player to pin the recipe again to verify buildability

#### Scenario: A shortage returns

- **WHEN** a tracked material count falls below its requirement for a step that still needs to be built
- **THEN** the ingredient checklist returns with current counts

#### Scenario: Pinned recipe is built

- **WHEN** the game marks the pinned recipe step as developed
- **THEN** the tracker shows `Built` without ingredient rows or pickup markers even if development consumed materials

### Requirement: Show outstanding material sources on Deploy

Deploy SHALL show the pinned recipe and native category icon above Workshop-style rows for missing materials. Rows SHALL show native material icons where available and right-aligned owned/required quantities. The recipe heading SHALL lead directly into the rows. The tracker SHALL flag supported maps and discovered area tiles, and label tiles or list their sources when space is limited. The overlay SHALL preserve native focus, input, and undiscovered area names.

#### Scenario: Plan for a missing material with an area source

- **WHEN** the player browses a map with a discovered area tied to a missing material
- **THEN** its tile is highlighted with the material name, and a randomized loot source is identified as possible

#### Scenario: Read the pinned recipe and quantity

- **WHEN** a pinned weapon recipe requires two units of a material and the player owns one
- **THEN** Deploy shows the weapon or upgrade name, its category icon, and a right-aligned `1/2` beside the material

#### Scenario: Plan with a map-level source or unavailable destination

- **WHEN** a missing material has only a documented zone or its destination is absent from the Deploy list
- **THEN** the tracker shows the material and available location information without assigning it to an unsupported or undiscovered tile

#### Scenario: Change the pin or finish collecting

- **WHEN** the pin changes, is cleared, is built, or has no outstanding materials
- **THEN** Deploy hints reflect the current shortages and disappear when there are none

#### Scenario: Find the Communication Device Spring sniper encounter
- **WHEN** the player still needs Communication Device Spring for the active pin and views Miyanohatacho Shopping Street with Area 05 discovered
- **THEN** Deploy marks Area 05, Under the Overpass, with the material name and identifies the sniper using the game's localized enemy name

#### Scenario: Resolve enemy drops across recipes and areas
- **WHEN** any pinned recipe is short of a material linked to enemy drops in one or more discovered areas on the selected map
- **THEN** the tracker uses a shared game data index to mark each matching tile, labels the source as a possible enemy drop, and leaves other tiles and native navigation intact

#### Scenario: A drop has no supported encounter match
- **WHEN** a material has a documented zone but the index lacks an exact encounter there
- **THEN** Deploy retains its zone hint without guessing an area

### Requirement: Set pinned item visibility in Other Settings

With the ANOMALITH minimap Options integration loaded, Other Settings SHALL include a Pinned Item selector matching SSAO's ON/OFF interaction. The tracker SHALL persist the preference, use it for F8 in Base and Dives, and keep the pinned recipe available in the Workshop.

#### Scenario: Hide pinned recipe guidance

- **WHEN** the player turns Pinned Item OFF
- **THEN** the Base and Dive checklists, source hints, pickup labels and Deploy material hints are hidden, and the Workshop retains the pin

#### Scenario: Restore the pinned recipe guidance

- **WHEN** the player turns Pinned Item back ON
- **THEN** the existing pin shows current material requirements in the supported views
