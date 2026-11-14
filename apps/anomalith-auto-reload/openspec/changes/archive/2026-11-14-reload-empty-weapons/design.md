# Design

Use a Harmony postfix on the native reload condition in the action state machine. Only supplement a false reload condition for the active player, with an empty firearm, no firing cooldown, reserve ammo allowed by the game, and active gameplay. Keep the native bolt-action gate and let the game's action table handle priority, animation, and ammo. Quietly leave manual reload available if the hook is unavailable.

Check one magazine-fed firearm in the shooting range or a safe gameplay location. Use the game's reload route for single-round loading too.
