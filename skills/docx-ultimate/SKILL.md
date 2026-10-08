---
name: docx-ultimate
description: Create, inspect, edit, search, validate and render DOCX documents with native paragraphs, tables, local images, sections, notes and scalar templates. Use for document artifacts and their verification, not prose-only requests or unrelated spreadsheet, presentation or PDF work.
---

# DOCX Ultimate

Resolve this folder as `<skill-root>`. Use `node <skill-root>/dist/docx-ultimate.mjs`; the runtime requires Node.js 22.13 or newer. The folder is standalone.

## Choose the operation

Start with `capabilities` and `--help`. Use `describe` or `describe <command-or-batch-operation>` for exact current request fields, units, limits, target types and refusal boundaries. Availability means an implemented entry point, not universal format support or native acceptance.

- `read`, `inspect`, `search`: bounded stored-content views and exact source locators
- `validate`: declared schema, package and identifier checks
- `create`, `edit`, `markdown`: checked schema2 operation batches and builders
- `read-table-data`, `export-table`: typed local CSV/TSV/cached-XLSX input and stored DOCX table text export
- `doctor`: resource and native prerequisite diagnosis
- `native-smoke`: retained real known-answer evidence
- `render`: LibreOffice Writer PDF/page images in a fresh directory

Read only the references needed for the task:
- [Reading and targeting](references/reading.md)
- [Authoring and templates](references/authoring.md)
- [Table data](references/table-data.md)
- [Native rendering and evidence](references/native.md)
- [API requests and results](references/requests.md)

## Preserve intent and source

Keep the input immutable and choose a distinct output. Preserve content, native objects, formatting, review state and unknown structures outside the requested change. A reference or template controls only the requested match level; do not clear its content by default.

Read or search before editing. Use returned source locators with exact expectations; never guess a path from a label. Validate the whole batch before publication. Named output references refer only to earlier outputs in that batch. Unsupported review, field, control or compatibility boundaries refuse rather than flattening.

Local Markdown assets must remain inside the chosen asset root. Image authoring supports bounded local PNG/JPEG; there is no remote asset fetch. Cached spreadsheet imports never recalculate formulas. Advanced review authoring, field refresh, floating-image authoring, repeated/bound templates, charts and equation authoring are unavailable.

## Verify the result

Read the structured status, error and coverage fields. Successful schema checks do not establish native safety, field freshness or layout. Ordinary edits mark affected stored fields dirty without recalculating them.

For native work, run `doctor`, then `native-smoke` on the actual machine. LibreOffice Writer is the sole application; there is no engine selector or fallback. Review every produced page, including downstream pagination after an edit. Native PDF display can differ from stored field caches. Record actual checks and unresolved coverage; do not describe an unrun platform or unreviewed image as accepted.
