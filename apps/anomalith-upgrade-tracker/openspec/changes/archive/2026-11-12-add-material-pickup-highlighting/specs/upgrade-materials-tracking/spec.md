# Upgrade materials tracking delta

## MODIFIED Requirements

### Requirement: Display live shortages during a Dive

The tracker SHALL display each material's remaining requirement based on the game's combined bag and storage count. The checklist SHALL reuse the Storage UI's item-name and count visuals, with a right-aligned `x 000` quantity format. The checklist SHALL appear during active Dive gameplay, and F8 SHALL toggle its visibility without clearing the pin.

#### Scenario: Read a shortfall

- **WHEN** a pinned recipe requires three of a material and the player has one across the bag and storage
- **THEN** the Storage-style row shows `x 002` under the checklist's remaining-quantity heading

#### Scenario: Reach a material requirement

- **WHEN** the combined material count reaches or exceeds the recipe requirement
- **THEN** the checklist shows `x 000` for that material

#### Scenario: Leave active Dive gameplay

- **WHEN** the player is at the title, at base, in the training room, or in a loading transition
- **THEN** the Dive checklist is hidden

## ADDED Requirements

### Requirement: Highlight nearby needed pickups

The tracker SHALL mark nearby, on-screen, unobstructed ground pickups containing a material with a positive shortfall while the Dive checklist is enabled.

#### Scenario: Spot a needed pickup

- **WHEN** a spawned ground pickup within 30 meters of the active gameplay camera is visible and contains a needed material
- **THEN** the tracker labels the pickup with the material's name

#### Scenario: Stop marking an ineligible pickup

- **WHEN** a pickup disappears, becomes obstructed, leaves range, or its material is no longer needed
- **THEN** the tracker stops marking it

### Requirement: Show material source information

F9 SHALL toggle a panel listing the current map section and the game's documented zones for outstanding materials. A verified encounter or section SHALL be shown where available, and a documented zone alone SHALL be presented as a broader lead.

#### Scenario: Look up Crystal of Madness

- **WHEN** Crystal of Madness is still needed and the player opens the sources panel
- **THEN** the tracker lists the Sentinel encounter at Yotsubadai Heights, Block E Roof and shows HE Corp Building as an additional documented zone

#### Scenario: Material without an area record

- **WHEN** the game provides no usable location record for an outstanding material
- **THEN** the tracker indicates that no zone is listed
