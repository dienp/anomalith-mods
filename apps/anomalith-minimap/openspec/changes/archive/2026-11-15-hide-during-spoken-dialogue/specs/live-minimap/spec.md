# Live Minimap Delta

## MODIFIED Requirements

### Requirement: Hide outside active gameplay

The minimap SHALL hide while a dialog, in-game conversation shown in the side-message panel, cutscene, game menu, or loading transition is active and SHALL return when active gameplay resumes with map data available and the minimap enabled.

#### Scenario: Open and close the pause map

- **WHEN** the pause menu opens and then closes
- **THEN** the minimap is absent in the menu and returns to the current position during gameplay

#### Scenario: Dialog or cutscene

- **WHEN** the game enters a dialog or cutscene
- **THEN** the minimap is hidden for its duration

#### Scenario: In-game conversation during exploration

- **WHEN** an NPC or radio conversation appears in the game's side-message panel during Base or Dive exploration
- **THEN** the minimap stays hidden until the whole panel closes, including between dialogue lines
- **AND** it returns at the current position if gameplay still allows it and the Minimap setting is ON
