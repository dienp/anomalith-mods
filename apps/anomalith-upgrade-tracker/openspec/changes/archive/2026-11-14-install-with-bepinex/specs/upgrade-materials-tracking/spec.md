## ADDED Requirements

### Requirement: Load at startup with BepInEx

Upgrade Tracker SHALL load from `BepInEx/plugins` when the game starts without ScriptEngine. The release archive SHALL install its DLL in that folder.

#### Scenario: Launch after extracting the release

- **WHEN** a player extracts the release into an ANOMALITH installation with BepInEx and launches the game
- **THEN** BepInEx loads Upgrade Tracker and the player can use it in gameplay without a reload key

## MODIFIED Requirements

### Requirement: Preserve the pin across ScriptEngine reload

The tracker SHALL store its single pinned recipe in its own local configuration and restore it after a game restart or a ScriptEngine reload. The tracker SHALL read game state without writing to inventory, game settings, or saves.

#### Scenario: Reload with a pin

- **WHEN** the game restarts or ScriptEngine reloads the plugin after a recipe has been pinned
- **THEN** the reloaded instance retains the recipe and obtains current counts when the checklist is next shown
