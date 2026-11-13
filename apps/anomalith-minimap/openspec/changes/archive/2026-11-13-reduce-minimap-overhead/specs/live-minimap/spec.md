## MODIFIED Requirements

### Requirement: Show live player location

The minimap SHALL appear above the lower-right status HUD during active Base and Dive exploration where the native pause map has data. It SHALL use the current native map and floor and show the player's position and facing. It SHALL avoid intercepting input.

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
- **THEN** the minimap is square, matches the full health bar's width and right edge, and shows a zoomed-in view around the player

## ADDED Requirements

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
