## ADDED Requirements

### Requirement: Set pinned item visibility in Other Settings

With the ANOMALITH minimap Options integration loaded, Other Settings SHALL include a Pinned Item selector matching SSAO's ON/OFF interaction. The tracker SHALL persist the preference, use it for F8 and keep the pinned recipe available in the Workshop.

#### Scenario: Hide pinned recipe guidance

- **WHEN** the player turns Pinned Item OFF
- **THEN** the Dive checklist, source hints, pickup labels and Deploy material hints are hidden, and the Workshop retains the pin

#### Scenario: Restore the pinned recipe guidance

- **WHEN** the player turns Pinned Item back ON
- **THEN** the existing pin shows current material requirements in the supported views
