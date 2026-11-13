# Upgrade materials tracking delta

## MODIFIED Requirements

### Requirement: Pin one Workshop upgrade

The tracker SHALL let the player pin the currently highlighted, revealed recipe with material costs in a supported Workshop development or reinforcement menu. A new pin SHALL replace the previous pin.

#### Scenario: Pin a Workshop recipe

- **WHEN** the player presses F7 on a revealed recipe with material costs and a development level still available
- **THEN** the tracker captures the recipe identity, current step, and material requirements and confirms the pin

#### Scenario: Ineligible selection

- **WHEN** the highlighted recipe is hidden, fully developed, has no material costs, or is outside a supported Workshop menu
- **THEN** the tracker preserves any existing pin

#### Scenario: Clear a pin

- **WHEN** the player presses Shift+F7
- **THEN** the tracker clears the pin and its checklist

### Requirement: Display live shortages during a Dive

The tracker SHALL display each material's remaining requirement based on the game's combined bag and storage count while any material is missing for an unbuilt pinned recipe. The checklist SHALL reuse the Storage UI's item-name and count visuals, with a right-aligned `x 000` quantity format. The tracker SHALL appear during active Dive gameplay, and F8 SHALL toggle its visibility without clearing the pin.

#### Scenario: Read a shortfall

- **WHEN** a pinned recipe requires three of a material and the player has one across the bag and storage
- **THEN** the Storage-style row shows `x 002` under the checklist's remaining-quantity heading

#### Scenario: Reach a material requirement

- **WHEN** the player meets one material requirement and another remains unfulfilled
- **THEN** the completed material shows `x 000`

#### Scenario: Leave active Dive gameplay

- **WHEN** the player is at the title, at base, in the training room, or in a loading transition
- **THEN** the Dive tracker is hidden

## ADDED Requirements

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
