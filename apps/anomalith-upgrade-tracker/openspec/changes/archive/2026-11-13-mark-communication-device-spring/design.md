# Design

## Context

The drop table assigns `ITM_ENE_025` to `EN_007_VarB_TABLE`. Shopping Street Area 05 encounter scenes include `EN_007` variant B.

## Goals / Non-Goals

Show the known encounter on the existing discovered-area overlay. Leave spawn availability and additional zones unpredicted.

## Decisions

Use the existing encounter source for `map_105` so Deploy and the F9 hints agree. Keep the enemy hint conditional and use the localized area and material names.

## Risks / Trade-offs

A verified encounter configuration can still be unavailable on a particular visit, so the source is shown as possible.
