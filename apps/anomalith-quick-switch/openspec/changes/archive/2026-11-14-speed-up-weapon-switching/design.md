# Design

Apply the saved multiplier, defaulting to 2.0×, to the incoming firearm's native switch animation speed at the point where the voluntary player swap sets it. Retain the native animation progress gates and weapon-specific values. Leave forced swaps, throwable selection, aim speed, firing rate, reload speed, and inventory alone. If the expected native call cannot be found, leave the game behavior in place and log a warning.

Use the game's native ON/OFF selector and the Music Volume slider prefab under Other Settings. Store the slider value in tenths so it works with native integer steps, and format the displayed number with one decimal and the multiplier symbol. Save both preferences, apply them on the next swap, and remove both rows and the Harmony patch on hot unload. Compare the same destination firearm at stock and accelerated speed in the running game.

At the native scroll branch, resolve an assigned weapon from the saved shortcuts before the wheel opens and pass it to the normal swap path. Mirror the native shortcut order and direction, skip empty slots and the equipped weapon, and wrap the list. Use the existing toggle for this behavior. Keep the game's input binding and menu guards.
