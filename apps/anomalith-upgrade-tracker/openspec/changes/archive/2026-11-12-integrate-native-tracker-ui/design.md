## Context

The tracker already reads the Workshop's active selector, writes one pin to its own BepInEx configuration, and owns the Deploy overlay. The Workshop and Storage screenshots establish the typography, button treatment, and material row layout.

## Decisions

- Anchor the toggle to the native Required Materials section. Reuse Workshop font and button visuals, and route clicks and the configured shortcut through the same action. Keep native menu navigation and confirmation intact.
- Use the recipe type, ID, parent weapon, and level to decide whether the selected step is pinned. Legacy pins remain readable and can be replaced to capture their identity. Retain the explicit clear shortcut.
- Use the native Storage name and count styles and divider for the Deploy source list. Use the pinned recipe as the heading and include the weapon for reinforcement pins. Show a shortage count on each item row with the existing source notes below it.
- Keep the Deploy graph and its overlap checks, and remove view objects when ScriptEngine unloads the mod.
