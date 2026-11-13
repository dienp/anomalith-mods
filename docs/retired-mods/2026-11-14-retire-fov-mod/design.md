# Design

Ship the Minimap and Upgrade Tracker DLLs under `BepInEx/scripts` at the ZIP root. Use a new release tag so the new package points to its matching source. Retire the superseded ZIP and standalone FOV DLL from the previous release. Keep the retirement record in the documentation.

The quick install instructions cover BepInEx, ScriptEngine, extraction, and removal of the old override. The change does not modify the game or saves.
