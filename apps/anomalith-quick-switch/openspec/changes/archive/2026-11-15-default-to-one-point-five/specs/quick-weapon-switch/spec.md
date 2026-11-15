# Quick Weapon Switch Delta

## MODIFIED Requirements

### Requirement: Provide persistent Other Settings controls

The plugin SHALL add one native Quick Weapon Switch ON/OFF selector and one native Switch Speed slider in Other Settings, initialized from the saved preferences.

#### Scenario: Adjust switch speed

- **WHEN** the player adjusts the Switch Speed slider from 1.0× to 3.0× in 0.1× steps
- **THEN** the slider shows the selected multiplier, saves it and applies it on the next eligible swap while enabled

#### Scenario: First use

- **WHEN** the player has no saved multiplier
- **THEN** the slider defaults to 1.5×

#### Scenario: Turn Quick Weapon Switch off

- **WHEN** the player sets the selector to OFF
- **THEN** the next firearm swap uses its normal speed, scrolling uses the normal weapon wheel, and later sessions remember the choice

#### Scenario: Hot-reload in a running game

- **WHEN** ScriptEngine unloads and reloads the plugin
- **THEN** both preferences are retained and the game has one of each setting row and one active speed adjustment

#### Scenario: Reset to default

- **WHEN** the native slider is reset to its default
- **THEN** it returns to 1.5×
