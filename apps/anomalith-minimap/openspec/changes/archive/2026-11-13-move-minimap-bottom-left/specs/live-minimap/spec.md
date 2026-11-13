## MODIFIED Requirements

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
