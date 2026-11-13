# Spec Delta

## MODIFIED Requirements

### Requirement: Show outstanding material sources on Deploy

Deploy SHALL show the pinned recipe and native category icon above Workshop-style rows for missing materials. Rows SHALL show native material icons where available and right-aligned owned/required quantities. The recipe heading SHALL lead directly into the rows. The tracker SHALL flag supported maps and discovered area tiles, and label tiles or list their sources when space is limited. The overlay SHALL preserve native focus, input, and undiscovered area names.

#### Scenario: Plan for a missing material with an area source

- **WHEN** the player browses a map with a discovered area tied to a missing material
- **THEN** its tile is highlighted with the material name, and a randomized loot source is identified as possible

#### Scenario: Read the pinned recipe and quantity

- **WHEN** a pinned weapon recipe requires two units of a material and the player owns one
- **THEN** Deploy shows the weapon or upgrade name, its category icon, and a right-aligned `1/2` beside the material

#### Scenario: Plan with a map-level source or unavailable destination

- **WHEN** a missing material has only a documented zone or its destination is absent from the Deploy list
- **THEN** the tracker shows the material and available location information without assigning it to an unsupported or undiscovered tile

#### Scenario: Change the pin or finish collecting

- **WHEN** the pin changes, is cleared, is built, or has no outstanding materials
- **THEN** Deploy hints reflect the current shortages and disappear when there are none

#### Scenario: Find the Communication Device Spring sniper encounter
- **WHEN** the player still needs Communication Device Spring for the active pin and views Miyanohatacho Shopping Street with Area 05 discovered
- **THEN** Deploy marks Area 05, Under the Overpass, with the material name and identifies the Unknown Soldier sniper as a possible drop source
