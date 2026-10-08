# XLSX data and calculation fidelity

## Preserve stored meaning

Identify the row grain, keys, source field mapping, period, units, currency, precision, and update cadence only to the extent needed to store or transform data correctly. Distinguish blank, missing, zero, false, error, and not-applicable values. Preserve leading zeros and text formatting in identifiers. Store quantities and dates as typed values only when their meaning and locale are established; formatting changes display, not the underlying value.

Profiling over bounded prepared records can reveal type drift, duplicate keys, missing keys, and date or identifier inconsistencies. It does not authorize coercion. Reconciliation must compare explicit keys in both directions and report duplicate keys and unmatched rows instead of silently retaining one record.

For a stored workbook range, use `analyze profile <input> <sheet> <range>`. The first row supplies unique, nonempty column names; the result counts blank, zero, missing and stored types by column. Use `analyze reconcile <left> <sheet> <range> <right> <sheet> <range> --key <column> --value <column>` to compare numeric values by a named key, with optional `--tolerance` and `--max-results`. It reports duplicate and unmatched keys in both directions. Neither command recalculates formulas or changes either workbook.

## Bound materialized data

Range reads, value/formula writes, clears and copies, and delimited conversion refuse more than 1000000 logical cells or 64 MiB of serialized scalar/formula content per materialized result. A read returning both values and formulas shares one content budget. The limits are checked before dense allocation or publication; a tiny file with distant occupied cells can still exceed them. Use smaller explicit ranges rather than increasing the hard cap. Analysis keeps its stricter 100000-cell limit and formatting/layout retain their own limits. These bounds do not prohibit sparse structural references to large worksheet areas.

## Trace calculations with coverage limits

Keep four states distinct:

- formula text stored in the cell;
- referenced cells and ranges found by static analysis;
- cached value stored in the package;
- value freshly evaluated by a named calculation engine.

When a formula changes, inspect its absolute and relative references, copied-formula pattern, error tokens, downstream dependencies, and representative boundary cells. Static formula and dependency analysis operates only on supplied records and supported reference syntax. An unexpanded range, missing record, unsupported named, structured, external, or dynamic reference, or depth limit makes coverage partial. Absence of an edge under partial coverage does not prove no dependency.

`analyze formulas <input> <sheet> <range>` reports a bounded formula inventory, copied-pattern hints, broken references and stored error tokens from that range. `--max-results` bounds each reported list while preserving its count and truncation flag. A pattern finding is a review hint, and stored formula caches can be stale. A formula node that has no stored text, such as an unresolved shared-formula follower, is counted under `coverage.omitted_formula_count`; coverage is partial until it is resolved in a workbook application.

Do not replace a formula with its current value unless a static snapshot is requested. Do not blanket-wrap missing input with `IFERROR(...,0)` or convert an absent lookup into zero. For sheet rename, range move, or copied formulas, verify the updated references and check a relevant changed input instead of trusting an old cache.

Formula-like strings from delimited or other untrusted text remain literal. Explicit formula authoring may introduce external data or executable functions; inspect those references before storing them.

Formula writers store functions listed as future functions, plus `XMATCH` as saved by Excel 2024, in Excel's OOXML form (`_xlfn.` or `_xlfn._xlws.`), and store LET/LAMBDA parameter names with `_xlpm.`. Formula reads show the names without those storage prefixes. A bare name matching a table is written as `Table[#Data]`, and the `@` this-row shorthand as the `[#This Row]` keyword (`Sales[@v]` is stored as `Sales[[#This Row],[v]]` and read back as `Sales[@v]`); the formula receipt lists these rewrites. Unknown function names are left unchanged and named in a warning so unsupported functions can be identified instead of treated as evaluated. Spill-capable formulas such as `UNIQUE` carry dynamic-array metadata; their spill cells have no stored values until recalculation. Formulas the writer did not change retain their stored text.

An outer `XMATCH` with an array constant, cell range, table column or spill expression as its lookup value carries dynamic-array metadata. A scalar lookup stays a scalar formula. When Calc returns only a non-error anchor for a dynamic array, the receipt reports `coverage: "partial"`, `coverage_issues: ["dynamic_array_anchor_only"]` and the affected cells in `unverified_arrays`, including cases that might legitimately produce one cell. A trustworthy anchor is retained; formulas that may read its unverified spill region and their dependents are withheld, and related chart caches are cleared. That region conservatively extends down/right to the worksheet boundary because its actual extent is unknown. An anchor that depends on another unverified array can itself be withheld. Do not claim complete spill evaluation; recognition of array behavior is not a general formula-shape evaluator.

## Preserve calculation behavior

Preserve workbook calculation mode, iterative-calculation settings, and external-refresh behavior unless the operation explicitly changes them. Writers keep the calculation mode and only mark a full recalculation on load; `range set --calc-mode auto` forces automatic mode and is an explicit choice, not a default.

Invalidating a cache is not evaluating it. Every write (`range set`, `range clear`, `range copy`, `repair`, builder edits) clears the cached result of each formula that can depend on the edit, directly or transitively, and reports it in `dependencies`. For an array formula this includes its stored output cells and formulas reading those cells:

- Cell and range references, full-column and full-row references, defined names that stand for cells, and table references are resolved to the cells they read.
- A formula whose inputs cannot be pinned to cells (`INDIRECT`, `OFFSET`, a name defined by a formula, unparsed syntax) is cleared on every edit and listed under `conservatively_invalidated` with the reason. A reader that does not recalculate sees a blank there until the workbook recalculates.
- Creating a table invalidates references to that identity even when its headers need no normalization. Adding a worksheet invalidates sheet-count/order-sensitive results and their dependents.
- `CELL` reads column widths and cell formats, so `setColumnWidth`, `setFormat`, hyperlink styling, format clear and copies that change styles clear the cached result of every `CELL` call, and of formulas reading it, even though no cell value changed.
- `coverage: verified` means no formula that could depend on the edit keeps a cache; `partial` means the scan reached its limit and some caches may be stale. Say so when it matters.
- `range set --keep-stale-cache` skips invalidation and warns that stale values may remain.

`--recalculate` stores fresh results from Calc ([setup.md](setup.md) explains prerequisites and how results reach the package); `calculation.evaluation.engine` names it. LibreOffice evaluates some forms of Excel table reference wrongly, so with LibreOffice the results for formulas using them, and for formulas depending on those, are withheld: their caches stay empty and `withheld_cells` counts them. Stored are only forms checked to match Excel's answers on LibreOffice 26.8: a reference naming a table that has a header row, to its columns, a column span, `[#Data]`, `[#All]`, `[#Headers]`, `[#Totals]` (when the table has a totals row), `[#Headers],[#Data]`, `[#Data],[#Totals]`, or `[@Column]`. Withheld are a table without a header row, a reference without its table name (`[@v]` or `[v]` alone), a bare table name, spaces inside the brackets, the whole-row `[@]`, a column the table lacks, and every table reference under any other LibreOffice release. A formula using a defined name that stands for a formula rather than cells counts as depending on every withheld cell. `CELL` and `INFO` results, and those of formulas depending on them, are withheld from calculation: they describe the throwaway working copy the application calculated, such as its path, and LibreOffice's `CELL` format codes and prefixes differ from Excel's. The workbook recalculates them when opened. Formulas that evaluated to an error are listed in `error_cells`; inspect unexpected errors with the reported compatibility limits. Report material results as unevaluated when the writer produced no fresh result, and prefer `range get --recalculate` over stored values when freshness matters.

## Chart-cache evidence

`dependencies.chart_caches` and `calculation.evaluation.chart_caches` report invalidated counts, conservative reasons, changed parts and their own coverage. Source edits, transitive formula changes, structural rewrites and calculation grafts clear affected derived caches owned by standard chart reference nodes. Literal series, unaffected caches, chart identities and formatting remain. Unresolved standard references are cleared conservatively; unowned chart extensions remain with partial chart coverage. Worksheet-formula coverage does not certify chart coverage, and recalculating cell values does not rebuild serialized chart caches. When displayed points matter, render and inspect the affected chart; do not call retained or absent cache data fresh.

For charts, totals, summaries, and filters, verify the owned source range, unit, time period, aggregation, denominator, hidden-row behavior, and cache/workbook synchronization only when those properties are part of the requested artifact contract.

Recalculated reads and writes include the results in the array output range saved by the application, not just the anchor. A write retains the formula and styles, updates its output range, and removes old output values when the array shrinks. If the application supplies an incomplete array, its old results are cleared and calculation coverage is partial. A calculated array that would replace unrelated source data is refused with `CALCULATED_ARRAY_CONFLICT`. When table-reference results are withheld, their array outputs are withheld too. LibreOffice may retain a newly authored array's one-cell range; expansion is not verified in that case. Report this limit rather than switching applications.

## Reading formatted dates

`range get` retains its raw `values` and `formulas` and adds a sparse `date_cells` array for numeric cells with a date/time base number format. `inspect` includes the same evidence for cells in each table preview window. These are stored-value reads unless recalculation was explicitly requested.

For a serial `46331` with format `mm/dd/yyyy` in a 1900 workbook, a record looks like:

```json
{
  "address": "D38",
  "raw_value": 46331,
  "number_format": "mm/dd/yyyy",
  "date_system": "1900",
  "iso_value": "2026-11-05T00:00:00.000",
  "formatted_value": "11/05/2026",
  "format_status": "formatted"
}
```

Use `iso_value` to avoid ambiguous day/month ordering. It represents a timezone-free calendar value, not an instant in the host timezone. Numeric month/day/year, hours/minutes/seconds, AM/PM, quoted/escaped literals and English month names are supported. Cell formats take precedence over row and column defaults. General numbers and date-looking text are not coerced into dates.

Formatting is derived from the base number format, not column width, conditional formatting or an application's locale. Built-in format codes use their standard definitions; month names use English. Locale/calendar directives, conditions, multiple sections, weekday names, elapsed time, fractional seconds and unrecognized tokens return `formatted_value: null`, `format_status: "unsupported"` and a reason. Out-of-range serials are also identified. A supported format can display Excel's fictitious 1900-02-29 (serial 60), with `excel_1900_leap_day: true` and `iso_value: null`; the 1900 zero calendar day is withheld. ISO values use millisecond precision; source numbers remain unchanged.

This is static formatting evidence, not native application display verification. Date evidence shares the range's returned-content budget with values and formulas.
