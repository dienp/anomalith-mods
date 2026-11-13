# attachment-compatibility Specification

## Purpose
Show which revealed weapons fit a selected Workshop attachment before the player spends materials developing it.

## Requirements

### Requirement: Show compatible weapons in attachment development

The Workshop Attachment tab SHALL list the revealed weapons that can equip the highlighted attachment according to native customization rules. Each weapon SHALL have its own row. Weapons that still need development SHALL appear together under a separate heading. The panel SHALL follow selection and preserve native input and undiscovered names.

#### Scenario: Compare two attachments in the same slot

- **WHEN** the player highlights a different attachment in the same category
- **THEN** the list reflects the new attachment's individual fit data

#### Scenario: Attachment fits a revealed weapon that is not developed

- **WHEN** a compatible weapon is revealed but has not been developed
- **THEN** its name appears in its own row under Not developed, below any developed weapons

#### Scenario: No revealed weapons fit

- **WHEN** the selected revealed attachment has no compatible revealed weapons
- **THEN** the panel says no known weapons fit

#### Scenario: Leave the attachment tab

- **WHEN** the highlighted attachment is unrevealed or the player leaves the attachment development menu
- **THEN** the panel is hidden

#### Scenario: Read an attachment with several compatible weapons

- **WHEN** the selected attachment fits several revealed weapons
- **THEN** the panel shows one name per row and keeps the native Required Materials controls clear
