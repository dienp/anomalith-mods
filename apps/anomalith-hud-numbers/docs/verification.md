# Runtime verification

HUD Numbers 1.0.0 hot-loaded through ScriptEngine in the running game.
At 1920 × 1080 in the Base, HP 128 / 128 and MP 130 / 130 appeared to
the left of the corresponding bars, with the game's font and an outline.
Labels were positioned in HUD canvas coordinates to remain upright when
the native gauge image was mirrored.

A temporary probe checked fractional HP and MP rounding, negative HP,
MP above its maximum, NaN, and a zero maximum. After reloading the mods
with Options open and returning to gameplay, exactly one HP label and one
MP label existed with the current player values. The probe was removed.

Version 1.0.1 added the native HP/MP Numbers ON/OFF selector to Other
Settings. A temporary live probe switched it OFF, returned to gameplay,
and confirmed both labels were disabled. Reopening Options and reloading
the mod preserved OFF with exactly one selector. Switching ON restored
both labels without restarting. The original ON preference and gameplay
state were restored, and the test probe was removed.

Version 1.0.2 copies the live health bar color on every numeric HUD update.
A temporary runtime probe confirmed identical HP text and bar RGB values
of 0.255, 0.859, 0.800, with the MP text retaining its existing color.
