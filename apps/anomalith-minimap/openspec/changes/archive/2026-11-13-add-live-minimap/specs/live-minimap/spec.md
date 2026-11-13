## Purpose

Show the current ANOMALITH floor and player location in the gameplay HUD using the game's map data.

## ADDED Requirements

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

### Requirement: Hide outside active gameplay

The minimap SHALL hide while a dialog, cutscene, game menu, or loading transition is active and SHALL return when active gameplay resumes with map data available.

#### Scenario: Open and close the pause map

- **WHEN** the pause menu opens and then closes
- **THEN** the minimap is absent in the menu and returns to the current position during gameplay

#### Scenario: Dialog or cutscene

- **WHEN** the game enters a dialog or cutscene
- **THEN** the minimap is hidden for its duration
