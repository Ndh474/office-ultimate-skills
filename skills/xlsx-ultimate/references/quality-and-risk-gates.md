# XLSX verification by impact

Select the smallest evidence set that can change the decision. A successful command, package comparison, engine reload, or render answers only its declared question.

## Operation matrix

| Operation | Minimum sufficient evidence |
|---|---|
| Read-only inspection | Inspect the requested sheets, ranges, formulas, objects, or package facts. Render only for a visual question. |
| Exact literal edit | Writer target readback, stored type/value, style continuity, the set of changed package parts, and dependent-cache invalidation coverage when downstream formulas depend on the cell. |
| Formula edit | Stored formula, reference and coverage status, calculation-mode/cache state, and representative dependent outputs when a fresh engine result is required. |
| Local structural edit | Owned cell/table/name/validation/chart identities, relationships, styles, formulas, and dependency closure. |
| Builder or object edit | Writer evidence plus preservation checks for every native feature and unowned package state in scope. A successful reload alone is insufficient. |
| Broad or destructive edit | Before/after ranges, identities, formulas, review/protection state, dependencies, and exact changed package parts. |
| New XLSX | Requested sheets, types, formulas, tables, charts, validations, and representative outputs. Render only when visual or print behavior is part of acceptance. |
| Template or reference match | Mechanical role and format comparison at the declared scope; render the affected ranges or sheets when appearance matters. |
| CSV, TSV, or ODS conversion | Explicit coercion/loss policy plus representative values, formulas, precision, dates, identifiers, blanks, and destination behavior when required. |
| Explicit audit or release | Run the named checks and bind results to the exact artifact and the application that produced any evaluated or rendered evidence. |

Reuse exact target readback and candidate checks emitted by the writer. Do not repeat them on the unchanged output. Add a separate inspection only for dependencies, native objects, calculation behavior, or preservation facts the writer did not cover.

## Package and calculation integrity

For the selected scope, verify:

- every changed, added, or deleted part is declared and edited XML is structurally valid;
- affected relationships and content-type declarations remain consistent;
- sheet, table, name, drawing, chart, comment, and review identities remain valid;
- formulas, types, number formats, validation, conditional formatting, filters, hidden state, protection, and calculation settings match the requested operation;
- unknown or unowned parts remain unchanged, or the writer refuses publication;
- dependency evidence exposes unsupported syntax, unexpanded ranges, incomplete records, and depth limits instead of claiming complete coverage;
- cached and freshly calculated results are labelled according to the evaluation evidence actually available, and a receipt never marks a check `pass` for something it did not exercise.

Use `audit package` when package graph integrity is material, `security` for untrusted workbooks when that risk is actionable, and `audit provenance` only for a prepared formula-record lineage question. Do not suppress a refusal by rebuilding the workbook with a less protective tool.

## Visual and application evidence

Use `preview` for a bounded sheet or range and `render` for PDF or print-layout evidence. Inspect only the relevant scope for cut-off values, missing glyphs, broken number formats, unreadable widths or heights, hidden rows or columns, chart labels, print area, page breaks, scaling, and repeated headers. Use `visual-diff` only when a visual regression matters.

Render evidence does not prove formula recalculation, validation interaction, filters, pivots, or macros. When these are acceptance criteria, obtain evidence for the specific behavior. This runtime evaluates and renders through Calc only; a criterion requiring Excel or another application needs authorized independent verification, or must be reported as unverified. Do not open an unsafe original to work around a native-input refusal.

## Publication and interruption

Writers validate a staged candidate and install without replacing an unexpected competing path. With overwrite, an existing output is backed up and restored without replacement if installation fails. Unsupported atomic installation refuses; a recovery conflict retains both the competing output and the reported backup. Successful cleanup warnings name retained files or directories and must be kept with the result.

Cancellation is checked before publication and between preparation stages. If a cancellation is observed after installation, the error includes `details.completed_publications`; inspect those paths before retrying, because some can be temporary artifacts already cleaned. CLI SIGINT/SIGTERM exits are 130/143, not an ordinary success. Cancellation cannot preempt synchronous JavaScript, and this contract does not promise crash-atomic replacement. Never delete a reported retained work directory until owned-process cleanup is confirmed.

## Handoff

Report the artifact path, material changes, calculation state when relevant, actual visual scope, and limitations that change how the workbook may be used. Omit internal diagnostics and duplicated check inventories unless requested or intended as evidence.
