## ADDED Requirements

### Requirement: Load at startup with BepInEx

Minimap SHALL load from `BepInEx/plugins` when the game starts without ScriptEngine. The release archive SHALL install its DLL in that folder.

#### Scenario: Launch after extracting the release

- **WHEN** a player extracts the release into an ANOMALITH installation with BepInEx and launches the game
- **THEN** BepInEx loads Minimap and the player can use it in gameplay without a reload key
