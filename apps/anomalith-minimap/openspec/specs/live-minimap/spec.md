# live-minimap Specification

## Purpose
Show the current ANOMALITH floor and player location in the gameplay HUD using the game's map data.

## Requirements

### Requirement: Show live player location

The minimap SHALL appear in the bottom-left corner during active Base and Dive exploration where the native pause map has data. It SHALL use the current native map and floor and show the player's position and facing. It SHALL avoid intercepting input.

#### Scenario: Move or turn

- **WHEN** the player moves or turns during exploration
- **THEN** the floor plan moves relative to the player arrow and the arrow follows the player's facing in real time

#### Scenario: Change floor or area

- **WHEN** the player enters a different mapped floor or area
- **THEN** the minimap shows the new map and floor without retaining the previous floor image

#### Scenario: Native map unavailable

- **WHEN** the current floor has no native map or its image is still loading
- **THEN** the minimap is hidden

#### Scenario: Read the enlarged map above the HUD

- **WHEN** active gameplay is displayed at 1920 × 1080
- **THEN** the minimap sits near the bottom-left edges, matches the full health bar's width, and shows a square, zoomed-in view around the player

### Requirement: Hide outside active gameplay

The minimap SHALL hide while a dialog, cutscene, game menu, or loading transition is active and SHALL return when active gameplay resumes with map data available.

#### Scenario: Open and close the pause map

- **WHEN** the pause menu opens and then closes
- **THEN** the minimap is absent in the menu and returns to the current position during gameplay

#### Scenario: Dialog or cutscene

- **WHEN** the game enters a dialog or cutscene
- **THEN** the minimap is hidden for its duration

### Requirement: Show native points of interest

The minimap SHALL show the native map's current main and side objectives, doors and area connections, portals, and available fast-travel points on the displayed floor. Markers SHALL use recognizable native icons and keep the player arrow legible.

#### Scenario: Read landmarks while moving

- **WHEN** a point on the current floor enters the visible portion of the minimap
- **THEN** its icon appears at the same projected location on the floor plan

#### Scenario: Change floor or objective

- **WHEN** the displayed floor or the game's available objectives and connections change
- **THEN** the minimap updates its markers without retaining points from the previous floor or objective

#### Scenario: Browse near overlapping landmarks

- **WHEN** multiple icons overlap on the minimap
- **THEN** the player arrow stays visible and a higher-priority marker represents that location

### Requirement: Toggle the minimap

The minimap SHALL support a configurable on/off shortcut that defaults to F10. The setting SHALL persist in the plugin configuration. While off, the minimap SHALL be hidden and stop updating its map and landmarks.

#### Scenario: Turn the map off and back on

- **WHEN** the player presses the shortcut twice during active gameplay
- **THEN** the minimap hides on the first press and resumes at the current location on the second

#### Scenario: Reload with the map off

- **WHEN** ScriptEngine reloads the plugin after the player disables it
- **THEN** the minimap stays off until the player enables it

#### Scenario: Toggle repeatedly on the same floor

- **WHEN** the current floor image is loaded and the player turns the minimap off and on again without changing floors
- **THEN** the minimap reuses the loaded floor image and saves the final preference outside the input frame

### Requirement: Keep the scene visible through the map

The minimap SHALL use a translucent background while keeping its floor plan, player arrow and landmark icons legible.

#### Scenario: Explore with the map visible

- **WHEN** the minimap appears during gameplay
- **THEN** the world remains visible through the map background

### Requirement: Set minimap visibility in Other Settings

Other Settings SHALL include a Minimap selector with the same ON/OFF values, arrows, and interaction as SSAO. It SHALL use the persisted minimap preference shared with the configured shortcut.

#### Scenario: Toggle the minimap from Options

- **WHEN** the player changes Minimap from ON to OFF and returns to gameplay
- **THEN** the minimap is hidden until re-enabled, and reopening Options shows the current preference

#### Scenario: Reload the mods

- **WHEN** ScriptEngine reloads the minimap plugin
- **THEN** Other Settings shows one Minimap row reflecting the stored setting
