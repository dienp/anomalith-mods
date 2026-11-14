# Quick Weapon Switch Delta

## Purpose

Reduce how long it takes to be ready to shoot after selecting a firearm during gameplay.

## ADDED Requirements

### Requirement: Speed up voluntary firearm swaps

The plugin SHALL default to enabled and scale the incoming firearm's normal swap animation speed by the saved multiplier for player-initiated swaps during gameplay.

#### Scenario: Switch to a firearm

- **WHEN** the player selects a firearm and the normal swap is allowed
- **THEN** the faster animation advances through the existing readiness gates

#### Scenario: Preserve other actions

- **WHEN** the game forces a swap, the player selects a throwable, or the player aims, shoots, or reloads
- **THEN** the plugin leaves that action's normal rules and animation speed in effect

### Requirement: Cycle weapons on the first scroll

The plugin SHALL cycle weapons without opening the weapon wheel when Quick Weapon Switch is ON and a gameplay scroll can trigger a normal swap.

#### Scenario: Scroll up or down during gameplay

- **WHEN** the player scrolls once during eligible gameplay
- **THEN** the game begins switching to the next valid shortcut in that direction, wrapping and skipping empty shortcuts and the current weapon

#### Scenario: Preserve the original scroll behavior

- **WHEN** Quick Weapon Switch is OFF
- **THEN** the game uses its normal wheel and scroll behavior

### Requirement: Provide persistent Other Settings controls

The plugin SHALL add one native Quick Weapon Switch ON/OFF selector and one native Switch Speed slider in Other Settings, initialized from the saved preferences.

#### Scenario: Adjust switch speed

- **WHEN** the player adjusts the Switch Speed slider from 1.0× to 3.0× in 0.1× steps
- **THEN** the slider shows the selected multiplier, saves it and applies it on the next eligible swap while enabled

#### Scenario: First use

- **WHEN** the player has no saved multiplier
- **THEN** the slider defaults to 2.0×

#### Scenario: Turn Quick Weapon Switch off

- **WHEN** the player sets the selector to OFF
- **THEN** the next firearm swap uses its normal speed, scrolling uses the normal weapon wheel, and later sessions remember the choice

#### Scenario: Hot-reload in a running game

- **WHEN** ScriptEngine unloads and reloads the plugin
- **THEN** both preferences are retained and the game has one of each setting row and one active speed adjustment
