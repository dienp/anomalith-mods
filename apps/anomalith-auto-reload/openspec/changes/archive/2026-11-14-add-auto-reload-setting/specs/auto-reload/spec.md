# Auto Reload Settings Delta

## ADDED Requirements

### Requirement: Configure Auto Reload in Other Settings

The plugin SHALL add a native Auto Reload ON/OFF selector to Options → Other Settings and use the saved Auto Reload preference as its value.

#### Scenario: Change the preference in game

- **WHEN** the player changes Auto Reload using the native selector
- **THEN** the new preference takes effect on returning to play and is saved for later sessions

#### Scenario: Open Other Settings with Auto Reload installed

- **WHEN** the player opens Other Settings with Auto Reload installed by itself or alongside the other mods
- **THEN** exactly one Auto Reload row is available and existing rows continue to work
