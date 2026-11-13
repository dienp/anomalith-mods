## ADDED Requirements

### Requirement: Set minimap visibility in Other Settings

Other Settings SHALL include a Minimap selector with the same ON/OFF values, arrows, and interaction as SSAO. It SHALL use the persisted minimap preference shared with the configured shortcut.

#### Scenario: Toggle the minimap from Options

- **WHEN** the player changes Minimap from ON to OFF and returns to gameplay
- **THEN** the minimap is hidden until re-enabled, and reopening Options shows the current preference

#### Scenario: Reload the mods

- **WHEN** ScriptEngine reloads the minimap plugin
- **THEN** Other Settings shows one Minimap row reflecting the stored setting
