# Design

Auto Reload owns its Other Settings row so it can run independently. Use the same native OptionItemSelect prefab and boolean values as Minimap and Pinned Item. Keep the row parameter local and synchronize it with the existing Gameplay.Enabled config. Attach after normal menu initialization and remove the owned row when the plugin unloads. Repair row indices and selection if the list changes during a ScriptEngine reload.

Use a temporary gameplay observer to check the magazine going empty, the native reload state, and the resulting ammo count. Keep the observer out of the released plugin.
