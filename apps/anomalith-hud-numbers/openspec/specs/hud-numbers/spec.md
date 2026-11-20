# hud-numbers Specification

## Purpose
Show readable current and maximum HP and MP beside the existing HUD bars.

## Requirements

### Requirement: Show player resource values

The HUD SHALL show current and maximum HP and MP from the current player's native status beside the corresponding bars. HP SHALL round up, current MP SHALL round down, and maximum values SHALL round up. Remaining values SHALL be clamped between zero and their maximum. Invalid values SHALL be hidden.

#### Scenario: Health or mana changes

- **WHEN** the player takes damage, heals, spends MP, regenerates MP, or changes a maximum value
- **THEN** the numeric values update within 0.1 seconds of active frames

#### Scenario: Player status is unavailable

- **WHEN** the current player or initialized status is unavailable
- **THEN** the labels are hidden until valid status is available

### Requirement: Follow the native HUD

The labels SHALL follow the native bars' positions, scaling, and visibility while remaining upright when a gauge image is mirrored. They SHALL receive no input. Reloading the mod or changing the player HUD SHALL remove old labels.

#### Scenario: Hide the HUD or a gauge

- **WHEN** the HUD or corresponding gauge is hidden
- **THEN** its numeric label is hidden

#### Scenario: Reload the mod

- **WHEN** the mod is reloaded with an active player HUD
- **THEN** exactly one HP label and one MP label appear beside the current bars

#### Scenario: Health bar changes color

- **WHEN** the current health bar changes color
- **THEN** the HP number matches the bar's current color within the next numeric HUD update

### Requirement: Toggle HP/MP Numbers in Other Settings

Other Settings SHALL include a native HP/MP Numbers ON/OFF selector that works when this mod is installed by itself. It SHALL use the persisted General.Enabled preference.

#### Scenario: Disable and re-enable numbers

- **WHEN** the player switches HP/MP Numbers OFF and returns to gameplay
- **THEN** both numeric labels are hidden
- **AND** switching the setting back ON restores both labels without restarting

#### Scenario: Reload with numbers disabled

- **WHEN** the player switches HP/MP Numbers OFF and reloads the mod
- **THEN** both labels remain hidden
- **AND** reopening Other Settings shows exactly one HP/MP Numbers row set to OFF
