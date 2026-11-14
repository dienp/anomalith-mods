# auto-reload Specification

## Purpose

Automatically start a normal firearm reload when the active weapon runs out of ammunition during gameplay.

## Requirements

### Requirement: Reload an empty firearm during play

The plugin SHALL be enabled by default and SHALL use the native reload action for the player's active empty firearm when the game allows reloading.

#### Scenario: Fire the last round

- **WHEN** the player's active firearm empties and the native action state allows a reload with reserve ammo
- **THEN** the normal reload begins without pressing reload, using the normal animation and ammo

#### Scenario: Keep native action restrictions

- **WHEN** the firearm is partially loaded, reserve ammo is unavailable, another action blocks reloading, or gameplay is paused or interrupted
- **THEN** the plugin does not request a reload

#### Scenario: Disable automatic reload

- **WHEN** the player disables the plugin option
- **THEN** the game handles reloads normally

### Requirement: Configure Auto Reload in Other Settings

The plugin SHALL add a native Auto Reload ON/OFF selector to Options → Other Settings and use the saved Auto Reload preference as its value.

#### Scenario: Change the preference in game

- **WHEN** the player changes Auto Reload using the native selector
- **THEN** the new preference takes effect on returning to play and is saved for later sessions

#### Scenario: Open Other Settings with Auto Reload installed

- **WHEN** the player opens Other Settings with Auto Reload installed by itself or alongside the other mods
- **THEN** exactly one Auto Reload row is available and existing rows continue to work
