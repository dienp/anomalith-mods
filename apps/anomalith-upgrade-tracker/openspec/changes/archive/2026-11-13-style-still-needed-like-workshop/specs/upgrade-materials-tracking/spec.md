## RENAMED Requirements

- FROM: `### Requirement: Display live shortages during a Dive`
- TO: `### Requirement: Display pinned requirements during gameplay`

## MODIFIED Requirements

### Requirement: Display pinned requirements during gameplay

The tracker SHALL show the pinned recipe and live Workshop-style requirements during active Base and Dive gameplay. Material rows SHALL use combined bag and storage counts, right-aligned owned/required quantities, native icons where available, and distinct shortage colors. The checklist SHALL omit empty rows and pin controls. The Pinned Item setting and F8 SHALL control its visibility in both locations.

#### Scenario: Read a shortfall

- **WHEN** the player is walking at Base with the tracker enabled and owns one of the three required materials
- **THEN** the pinned recipe name and its material row appear, with the count `1/3` styled as a shortage

#### Scenario: Reach a material requirement

- **WHEN** the player meets one material requirement and another remains unfulfilled
- **THEN** the satisfied material row shows its current owned/required count in the normal color

#### Scenario: Read the OV requirement

- **WHEN** the requirements are visible and the pinned step has a known positive OV cost
- **THEN** the last row displays the player's current OV and the step's cost in the same owned/required format

#### Scenario: Leave active Dive gameplay

- **WHEN** a menu, dialog, cutscene, transition, title screen or unsupported gameplay state is active
- **THEN** the gameplay checklist is hidden

#### Scenario: Return to Base

- **WHEN** the player returns from a Dive and resumes active Base gameplay
- **THEN** the checklist reflects current inventory counts and the same pinned recipe

### Requirement: Set pinned item visibility in Other Settings

With the ANOMALITH minimap Options integration loaded, Other Settings SHALL include a Pinned Item selector matching SSAO's ON/OFF interaction. The tracker SHALL persist the preference, use it for F8 in Base and Dives, and keep the pinned recipe available in the Workshop.

#### Scenario: Hide pinned recipe guidance

- **WHEN** the player turns Pinned Item OFF
- **THEN** the Base and Dive checklists, source hints, pickup labels and Deploy material hints are hidden, and the Workshop retains the pin

#### Scenario: Restore the pinned recipe guidance

- **WHEN** the player turns Pinned Item back ON
- **THEN** the existing pin shows current material requirements in the supported views
