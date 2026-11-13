## ADDED Requirements

### Requirement: Show compatible weapons in attachment development

The Workshop Attachment tab SHALL list the revealed weapons that can equip the highlighted attachment according to the native customization rules. The list SHALL distinguish weapons that still need development. It SHALL follow selection and preserve native input and undiscovered names.

#### Scenario: Compare two attachments in the same slot

- **WHEN** the player highlights a different attachment in the same category
- **THEN** the list reflects the new attachment's individual fit data

#### Scenario: Attachment fits a revealed weapon that is not developed

- **WHEN** a compatible weapon is revealed but has not been developed
- **THEN** its name indicates that it needs development

#### Scenario: No revealed weapons fit

- **WHEN** the selected revealed attachment has no compatible revealed weapons
- **THEN** the panel says no known weapons fit

#### Scenario: Leave the attachment tab

- **WHEN** the highlighted attachment is unrevealed or the player leaves the attachment development menu
- **THEN** the panel is hidden
