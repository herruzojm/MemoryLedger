# MemoryLedger
Project Issues (English – GitHub/Jira-style)

EPIC: Portable App Foundation

Initialize repository and solution

Description: Create repo, solution, basic folder structure (src/, tests/, docs/), license, README.

Acceptance Criteria: Repo initialized; CI placeholder added; README with goals and high-level features.

Dependencies: None.

Estimate: 1 day.

Portable packaging (no installer)

Description: Produce self-contained builds for Windows/macOS/Linux that run from a folder/USB without installation.

Acceptance Criteria: Zipped artifacts run by double-click; no admin rights required; app writes only to its own folder or a user-selectable data path.

Dependencies: Issue 1.

Estimate: 2 days.

App settings and paths

Description: Implement a minimal settings file (e.g., JSON) stored alongside the executable or in a configurable path.

Acceptance Criteria: Settings persist across runs; changing data path works; no registry usage.

Dependencies: Issue 2.

Estimate: 1 day.

EPIC: Security & Encryption
4) Per-journal encryption (AES-256-GCM with strong KDF)

Description: Encrypt each journal with a user passphrase; derive key using Argon2id or PBKDF2 with high iterations + random salt; store only KDF params + salt; zero-knowledge design.

Acceptance Criteria: New journals require passphrase; encrypted at rest; tamper detection via AEAD tag; wrong key fails cleanly.

Dependencies: Issue 3.

Estimate: 3 days.

Unlock/lock journal session

Description: UX to open a journal by entering passphrase; ability to lock the journal manually or after inactivity.

Acceptance Criteria: Locked state hides entries; auto-lock after configurable idle time; re-unlock requires passphrase.

Dependencies: Issue 4.

Estimate: 1.5 days.

Change passphrase / key rotation

Description: Allow changing the passphrase; re-encrypt journal with new key; preserve metadata.

Acceptance Criteria: Old key invalidated; new passphrase works; failure is atomic (no data corruption).

Dependencies: Issue 4.

Estimate: 2 days.

Brute-force mitigation

Description: Add exponential backoff and limited attempts per app session on wrong passphrase.

Acceptance Criteria: After repeated failures, delays increase; does not lock out legitimate users permanently.

Dependencies: Issue 5.

Estimate: 0.5 day.

No plaintext leakage

Description: Ensure no temporary plaintext files; wipe sensitive buffers when feasible; disable crash dumps containing secrets.

Acceptance Criteria: Manual review; QA confirms no temp artifacts; clipboard sanitized where used.

Dependencies: Issue 4.

Estimate: 1 day.

EPIC: Journal Management
9) Create journal

Description: UI flow to create a new journal (name, optional description), set passphrase, create encrypted container.

Acceptance Criteria: Journal appears in list; can be opened and locked; metadata stored.

Dependencies: Issue 4.

Estimate: 1 day.

Open journal

Description: From start menu, select journal, enter passphrase, open entries view.

Acceptance Criteria: Wrong passphrase handled; success loads entries.

Dependencies: Issue 5.

Estimate: 1 day.

Delete journal (secure)

Description: Delete a journal with confirmation; attempt secure deletion of the container file if OS allows.

Acceptance Criteria: Deleted journal no longer listed; file removed; confirmation dialog required.

Dependencies: Issue 9.

Estimate: 0.5 day.

List journals

Description: Show available journals discovered in the data path; allow rename of display name (not file name).

Acceptance Criteria: Accurate list; rename reflected immediately.

Dependencies: Issue 3.

Estimate: 0.5 day.

EPIC: Data Model & Storage
13) Define entry schema and ID strategy

Description: Entry fields: id:int, date:date, title:string, description:string, intensity:int.

Acceptance Criteria: Auto-increment numeric id per journal; validation rules defined (e.g., intensity range 0–10 by default).

Dependencies: Issue 9.

Estimate: 0.5 day.

Encrypted storage backend

Description: Implement an encrypted data store (e.g., SQLite with at-rest encryption or app-layer encryption over SQLite/files).

Acceptance Criteria: CRUD operations persisted; all at rest encrypted; basic migration scaffold in place.

Dependencies: Issues 4, 13.

Estimate: 2 days.

EPIC: Entries CRUD & Listing
15) Entries list view (newest first)

Description: Default listing sorted by date descending; paging or virtualized list for large data.

Acceptance Criteria: Load time < 500 ms for 5k entries; stable sort; empty-state message.

Dependencies: Issue 14.

Estimate: 1 day.

Create entry

Description: Form to add a new entry with validation.

Acceptance Criteria: All fields saved; id assigned; list refreshes at top.

Dependencies: Issue 15.

Estimate: 0.5 day.

Edit entry

Description: Open entry, modify fields, save or cancel.

Acceptance Criteria: Updates persisted; validation enforced.

Dependencies: Issue 15.

Estimate: 0.5 day.

Delete entry

Description: Remove an entry with confirmation.

Acceptance Criteria: Entry disappears; cannot be restored unless we add export/backup later.

Dependencies: Issue 15.

Estimate: 0.5 day.

EPIC: Search & Filtering
19) Multi-field search

Description: Filter by date range, title contains, description contains, intensity range; combinable filters.

Acceptance Criteria: Filters compose (AND); clear-filters button; performance: < 300 ms on 5k entries.

Dependencies: Issue 15.

Estimate: 1.5 days.

Saved searches (optional)

Description: Allow saving named filter presets.

Acceptance Criteria: Create, apply, delete presets.

Dependencies: Issue 19.

Estimate: 0.5 day.

EPIC: Reporting
21) Average intensity report

Description: Given a date range (default = last 1 year from today), compute and display average intensity and count.

Acceptance Criteria: Correct mean; empty-range handled; default range pre-filled; result shown as number.

Dependencies: Issue 19.

Estimate: 0.5 day.

Export report to CSV (optional)

Description: Export the aggregated metric and underlying filtered entries to CSV.

Acceptance Criteria: CSV opens in Excel; UTF-8 with header.

Dependencies: Issue 21.

Estimate: 0.5 day.

EPIC: UX & Quality
23) Start menu for journal management

Description: Initial screen with Create / Open / Delete; requires correct passphrase for open/delete flows.

Acceptance Criteria: Flows reachable; errors readable; keyboard friendly.

Dependencies: Issues 9–12.

Estimate: 1 day.

Keyboard shortcuts

Description: Add shortcuts (e.g., Ctrl+N new entry, Ctrl+F search, Ctrl+L lock).

Acceptance Criteria: Documented in help; works across platforms; does not conflict with OS defaults.

Dependencies: Issues 15–19.

Estimate: 0.5 day.

Error handling & notifications

Description: Centralized error messages; avoid leaking sensitive details; actionable copy.

Acceptance Criteria: All known failures show clear messages; no stack traces in UI.

Dependencies: Broad.

Estimate: 0.5 day.

EPIC: Testing & Tooling
26) Unit tests: crypto and KDF

Description: Tests for key derivation, encrypt/decrypt round-trip, tamper detection.

Acceptance Criteria: Positive/negative cases; deterministic vectors for KDF.

Dependencies: Issue 4.

Estimate: 1 day.

Unit tests: CRUD and search

Description: Tests covering create/edit/delete, list sort, filters, and report math.

Acceptance Criteria: >80% coverage for data layer; edge cases (empty results, max intensity).

Dependencies: Issues 14–21.

Estimate: 1.5 days.

Smoke tests for portable builds

Description: Script to launch built app on each OS target and perform a minimal flow.

Acceptance Criteria: Scripted run creates a journal, adds an entry, generates average.

Dependencies: Issue 2.

Estimate: 1 day.

EPIC: Documentation
29) User Guide

Description: Docs covering creating/opening/locking journals, entries CRUD, search, and reporting.

Acceptance Criteria: Markdown in /docs; screenshots; troubleshooting section.

Dependencies: Core features done.

Estimate: 1 day.

Security model & backup guide

Description: Explain encryption, passphrase importance, how to back up journals safely.

Acceptance Criteria: Clear warnings; restore procedure verified.

Dependencies: Issues 4–6, 11.

Estimate: 0.5 day.

EPIC: Nice-to-Have (Backlog)
31) Import/Export journal (encrypted archive)

Description: Export a journal to a single encrypted archive and re-import it.

Acceptance Criteria: Round-trip fidelity; passphrase required on import.

Dependencies: Core storage.

Estimate: 2 days.

Charts for intensity over time

Description: Simple time-series chart for intensity mean per month.

Acceptance Criteria: Correct aggregation; togglable.

Dependencies: Issue 21.

Estimate: 1 day.

Theming (light/dark)

Description: Switchable theme; persist preference.

Acceptance Criteria: No readability regressions.

Dependencies: UI base.

Estimate: 0.5 day.
