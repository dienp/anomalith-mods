## ADDED Requirements

### Requirement: Show outstanding material sources on Deploy

The tracker SHALL flag available maps with sources for the currently pinned recipe's missing materials. It SHALL highlight discovered area tiles with supported area-level sources and place readable material names near the tiles, with a compact area list when space is limited. The overlay SHALL preserve native focus, input and undiscovered area names.

#### Scenario: Plan for a missing material with an area source

- **WHEN** the player browses a map with a discovered area tied to a missing material
- **THEN** its tile is highlighted with the material name, and a randomized loot source is identified as possible

#### Scenario: Plan with a map-level source or unavailable destination

- **WHEN** a missing material has only a documented zone or its destination is absent from the Deploy list
- **THEN** the tracker shows the material and available location information without assigning it to an unsupported or undiscovered tile

#### Scenario: Change the pin or finish collecting

- **WHEN** the pin changes, is cleared, is built, or has no outstanding materials
- **THEN** Deploy hints reflect the current shortages and disappear when there are none
