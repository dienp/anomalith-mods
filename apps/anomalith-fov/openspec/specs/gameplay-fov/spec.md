# gameplay-fov Specification

## Purpose
Players can choose a custom vertical viewing angle for ANOMALITH's gameplay cameras that support its native field-of-view setting.

## Requirements

### Requirement: Configure gameplay field of view

The plugin SHALL read its own configuration at game startup. The override SHALL default to enabled at 65 vertical degrees and accept whole degrees from 40 through 100 inclusive.

#### Scenario: Enabled override

- **WHEN** the game runs with the override enabled at 65 degrees and initializes its FOV-adjustable cameras
- **THEN** those cameras use a vertical FOV of 65 degrees

#### Scenario: Disabled override

- **WHEN** the player sets `Enabled` to `false` and relaunches
- **THEN** the game uses its own FOV option

#### Scenario: Value outside the supported range

- **WHEN** the configured value is below 40 or above 100
- **THEN** the plugin clamps it to the nearest supported endpoint

### Requirement: Preserve the native camera and save systems

The override SHALL apply only to cameras affected by the game's FOV setting and SHALL leave the stored game option intact.

#### Scenario: Gameplay camera initialization

- **WHEN** the override is active and the game applies its FOV option
- **THEN** the game applies the override through its normal FOV update and the saved option retains its value
