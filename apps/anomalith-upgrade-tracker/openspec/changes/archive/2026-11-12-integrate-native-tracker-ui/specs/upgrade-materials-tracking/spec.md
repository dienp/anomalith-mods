## MODIFIED Requirements

### Requirement: Pin one Workshop upgrade

The tracker SHALL provide a visible pin toggle beside Required Materials for a revealed Workshop recipe with material costs. Clicking it or pressing the configured pin key SHALL toggle the current recipe step. Pinning a different recipe or step SHALL replace the previous pin.

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

### Requirement: Show outstanding material sources on Deploy

Deploy SHALL show the pinned recipe above Storage-style rows for missing materials, with right-aligned quantities. Reinforcement pins SHALL name the weapon and upgrade. The tracker SHALL flag maps with sources and highlight discovered tiles with supported area sources. It SHALL label those tiles with the materials or list them when graph space is limited. The overlay SHALL preserve native focus, input and undiscovered area names.

#### Scenario: Plan for a missing material with an area source

- **WHEN** the player browses a map with a discovered area tied to a missing material
- **THEN** its tile is highlighted with the material name, and a randomized loot source is identified as possible

#### Scenario: Read the pinned recipe and quantity

- **WHEN** a pinned weapon recipe still needs one of a material
- **THEN** Deploy shows the weapon or upgrade name and a right-aligned `x 001` beside the material

#### Scenario: Plan with a map-level source or unavailable destination

- **WHEN** a missing material has only a documented zone or its destination is absent from the Deploy list
- **THEN** the tracker shows the material and available location information without assigning it to an unsupported or undiscovered tile

#### Scenario: Change the pin or finish collecting

- **WHEN** the pin changes, is cleared, is built, or has no outstanding materials
- **THEN** Deploy hints reflect the current shortages and disappear when there are none
