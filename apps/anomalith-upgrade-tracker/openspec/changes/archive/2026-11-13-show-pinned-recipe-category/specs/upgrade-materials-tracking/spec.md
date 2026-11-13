## MODIFIED Requirements

### Requirement: Display pinned requirements during gameplay

The tracker SHALL show the pinned recipe and live Workshop-style requirements during active Base and Dive gameplay. The heading SHALL include the native Workshop category icon when available. Rows SHALL match Workshop proportions and show native material icons, combined bag and storage counts, and distinct shortage colors. The checklist SHALL omit empty rows, pin controls, and the OV row. The Pinned Item setting and F8 SHALL control its visibility in both locations.

#### Scenario: Read a shortfall

- **WHEN** the player is walking at Base with the tracker enabled and owns one of the three required materials
- **THEN** the pinned recipe name and its material row appear, with the count `1/3` styled as a shortage

#### Scenario: Reach a material requirement

- **WHEN** the player meets one material requirement and another remains unfulfilled
- **THEN** the satisfied material row shows its current owned/required count in the normal color

#### Scenario: Read the OV requirement

- **WHEN** the requirements are visible for a step with an OV cost
- **THEN** the gameplay checklist omits the OV row and shows a short status if OV is the remaining blocker

#### Scenario: Leave active Dive gameplay

- **WHEN** a menu, dialog, cutscene, transition, title screen or unsupported gameplay state is active
- **THEN** the gameplay checklist is hidden

#### Scenario: Return to Base

- **WHEN** the player returns from a Dive and resumes active Base gameplay
- **THEN** the checklist reflects current inventory counts and the same pinned recipe

#### Scenario: Read the Combat Backpack category

- **WHEN** the tracker displays an existing Combat Backpack pin
- **THEN** the Workshop's Backpack icon appears beside its name
