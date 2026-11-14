# Verification

## Live results

ScriptEngine loaded the updated plugin in the running game and retained the
saved 1.5× setting. The Other Settings toggle recorded both OFF and ON. The
live probe measured one active patch and 1.50× native animation speed after
changing the slider. For the same gun, the transition into aim-down-sights
took about 693 ms at 1.5× and 574 ms at 2.0×.

With the mod enabled, scrolling in either direction selected a different
weapon while the wheel stayed closed. The OFF branch calls the original
shortcut lookup. Reopening the stock wheel with the final build was checked
in code.

## Repeat the checks

In the shooting range, switch between the same two loaded guns while holding
aim. Compare the duration from the start of the native swap until it enters
the aim-down-sights action. Repeat at 1.0× and 3.0× with Quick Weapon Switch
ON. Switch it OFF and confirm that the gun uses its stock swap speed even
with the slider set higher.

Scroll one notch each way and check that the first notch starts a swap
without opening the weapon wheel. Turn the toggle OFF and check that the
stock wheel returns.

Hot-reload the DLL with Other Settings open. Check for one Quick Weapon Switch
row and one Switch Speed slider. The slider should look like Music Volume
and show the saved multiplier. Check that adjacent rows remain selectable.
