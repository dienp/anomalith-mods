# Design

Reuse the Storage tab sprite at a larger size. Inset the row and center the icon on the visible bounds of the quantity. Format the quantity like the Storage screen, using a lowercase x and at least three digits. Keep the spacing relative to the tooltip font and use the game's tooltip layout pass.

Use the native Options selector pattern from Auto Reload. Save the setting in a BepInEx config entry, default it to ON, and read it when the game draws a tooltip. Remove the settings row when the plugin unloads.
