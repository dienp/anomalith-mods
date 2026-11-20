# ANOMALITH mod development

## Mod settings

Every user-facing mod must provide its own ON/OFF toggle under
**Options → Other Settings**. Include this with each new mod and keep it
working when updating an existing mod.

- Use the game's native ON/OFF selector and a clear feature name.
- Persist the toggle in the mod's BepInEx configuration.
- Make the toggle available when the mod is installed by itself.
- Switching OFF must stop the mod's behavior and hide its UI. Switching
  ON must restore normal operation without restarting the game.
- Reopening Options or reloading the mod must preserve the setting and
  leave exactly one settings row. Clean up the row when unloading.
- Verify OFF, ON, persistence, and reload behavior before delivering.

## ANOMALITH playtesting

After a live ANOMALITH test that uses Windows screen capture, finish the
computer-use completion protocol and close the capture session before
reporting the result. For the unified computer-use tool, reset its session
with `cua_repl.js_reset` after submitting its completion summary. Confirm
that capture stopped using a non-capture check so the yellow border does
not stay on. Leave ANOMALITH running unless the user asks to close it.

## Release workflow

When the user says "release the mods" or asks for a new mod release,
complete this workflow end to end. This instruction defines the requested
scope, including publishing the release and its announcement.

- Review the pending changes, build all current mods in Release mode,
  and run the relevant checks. Resolve failures before publishing.
- Choose the next suite release version based on the latest published
  release. Keep individual plugin versions consistent with their changes.
- Sync the release changes to `main` and push them to GitHub. Preserve
  unrelated work and resolve branch differences without force-pushing.
- Package the current suite as a ZIP with the DLLs under
  `BepInEx/plugins`. Exclude retired mods, temporary probes, and local
  configuration files. Verify the archive contents.
- Create and push the version tag. Publish a GitHub release at
  https://github.com/dienp/anomalith-mods with the suite ZIP, individual
  plugin DLLs, SHA-256 checksums, and release notes.
- Publish the same suite ZIP as the new primary file on
  https://www.nexusmods.com/anomalith/mods/10. Archive the previous main
  file, update the mod version, and add the changelog.
- Update the Nexus description and installation details for the current
  features. Include relevant screenshots supplied for the release in the
  repository/release notes and Nexus gallery.
- Publish a concise announcement on the mod's Posts tab in the user's
  casual, direct style. Describe safeguards accurately when the original
  reported issue has not been reproduced.
- Verify the public release files, description, screenshots, and post.
  Finish the computer-use completion protocol and provide release links
  plus a saved screenshot showing the published result.

Proceed with the authorized workflow without redundant confirmation.
Request user action only for a concrete blocker such as login or a
required permission prompt.
