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
- **THEN** the minimap is 384 × 282 pixels and the enlarged floor plan stays above the status bars

## ADDED Requirements

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
