# Verification

The Release build was checked against ANOMALITH and BepInEx 5.4.23.6. The
plugin attached its native reload hook at startup. On a development install
with ScriptEngine 11.2, a DLL file change triggered a reload in the same game
process, and a runtime check found exactly one Auto Reload hook afterward.

## Gameplay check

In the Base shooting range, fire a magazine-fed firearm until it reaches zero.
Release the fire button after the last shot. Check that the normal reload
starts without pressing the reload button and that the ammo display updates.
Also check that a partially loaded firearm stays loaded without a reload.
