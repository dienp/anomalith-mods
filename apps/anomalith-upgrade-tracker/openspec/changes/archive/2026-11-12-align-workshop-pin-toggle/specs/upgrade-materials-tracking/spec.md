## MODIFIED Requirements

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
