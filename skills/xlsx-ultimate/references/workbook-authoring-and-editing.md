# XLSX authoring and editing fidelity

Existing `.xlsm` workbooks use the same read and edit operations as `.xlsx`. An edit must output another `.xlsm`; the VBA project and its relationship remain intact and its bytes are not changed. The skill never runs VBA: calculation and rendering receive a temporary macro-free `.xlsx` copy. If the workbook also contains an XLM macro sheet, ActiveX control or embedded active content, the application operation refuses with `MACRO_WORKING_COPY_UNSAFE` because that content cannot be removed safely. If a formula or defined name calls a function that cannot be proved built-in, application operations refuse with `MACRO_CALCULATION_UNSAFE`: without VBA, even `IFERROR` can conceal a wrong recalculated number. Read and edits without recalculation still preserve the macro project. The read-only `security` check still reports `MACRO_PRESENT` as a blocking risk. `create` and the new-workbook builder produce `.xlsx` only.

## Model native structure

Translate the supplied data and calculation contract into native worksheets, cells, formulas, tables, names, charts, validations, and print settings. For every tabular region, preserve or establish row grain, headers, keys, literal inputs, calculated columns, number formats, and boundary rows. Use the stable builder in [authoring-api.md](authoring-api.md) when its methods cover the request. Keep formulas as formulas and data as cells rather than flattening a worksheet to an image.

For an established workbook, reuse the existing sheet, table, row, column, and style roles. Preserve column widths, row heights, hidden state, frozen panes, filters, validations, conditional formats, print area, page breaks, scaling, and relevant accessibility metadata unless the requested operation owns them. A controlling template may require exact geometry, number formats, formula patterns, or native objects.

## Choose the preservation lane

Inspect the target and record the exact cells, objects, and expected prior state. `range set` edits literal values or formulas through a targeted worksheet-part lane and leaves untouched package parts byte-identical. It refuses protected sheets, merged-cell intersections, coupled array or shared formulas, and table header or totals edits that require related metadata changes. A write directly below or right of a table does not grow it and is warned about, as described in [Resize a table or add a totals row](#resize-a-table-or-add-a-totals-row). How every writer treats cached results and calculation mode is in [data-and-formulas.md](data-and-formulas.md).

OPC-signed package mutations refuse, including signatures declared at relocated or missing parts. A preserved VBA-project signature is a different structure. Generic edits to a protected sheet refuse until an explicit supported unprotect operation; this includes formatting, native objects and page setup. Existing layout exceptions remain: hiding/showing rows or columns honors the corresponding format permission, and tab color may change. Safe automatic cache/reference maintenance on dependent protected sheets is not a user content overwrite.

`range clear`, `range copy`, repairs, and builder continuation edits also rewrite only the parts they change. Generic content changes through clear/copy and builder values/formulas share the range-set ownership guards, including signed packages, protected sheets, merges, coupled formulas and table-owned content. Clearing only formats keeps cell content and may include merges or table headers on an unprotected, unsigned sheet. It also invalidates formula caches that depend on those formats. Copies require equal source and destination dimensions (`COPY_SHAPE_MISMATCH` otherwise), and use the original source state for the selected mode even when ranges overlap; self-copy does not erase cells. Native stored error and ISO-date scalar types survive every copy mode; `values` copies cached formula results without carrying formulas or source styles. Formula-preserving copies refuse imported array/shared/data-table source formulas they cannot reproduce safely; `values` mode may deliberately copy their stored results.

Their results are unevaluated unless `--recalculate` (or `recalculate: true` on the builder's `export`) is passed, so say so rather than implying fresh numbers.

Publication does not replace a destination that appears during candidate work, even when overwrite was requested. It refuses when an inspected existing destination changes during candidate work, and requires a filesystem supporting atomic no-replace installation (`ATOMIC_PUBLICATION_UNAVAILABLE` otherwise). If another file appears during failed-overwrite recovery, `OUTPUT_RECOVERY_CONFLICT` retains that file and the previous output's backup; inspect the reported `backup_path` before deciding how to recover. Other restoration failures report `OUTPUT_RECOVERY_FAILED` with the same recovery information. Do not delete a retained backup before recovering the previous output.

## Insert and delete whole rows or columns

Use `rows insert|delete` or `columns insert|delete` for a one-shot edit, or the matching builder methods in [authoring-api.md](authoring-api.md) when composing more changes. Positions start at row 1 or column A. Insert takes `--count`; delete takes one inclusive span such as `3:5` or `B:D`. Read `<command> --help` for exact syntax. The source and output must be different files.

These operations move cell contents, formulas, defined names, tables, merged areas, conditional formatting, validation, hyperlinks, filters, chart references, drawing anchors, notes, sparklines and layout references. Formula caches that may depend on the edit are cleared and the workbook requests recalculation when opened. Receipts list changed parts, broken references when explicitly permitted, and any dynamic-reference warnings. Parts outside the edit stay byte-identical.

A delete that would turn a reference into `#REF!` is refused with affected owners listed. Use `--allow-ref-errors` only when those broken references are intended; the builder accepts `{ allowRefErrors: true }`. A partly deleted area shrinks. `INDIRECT` and `OFFSET` text arguments cannot be rewritten and are reported. Edits also refuse protected sheets, a partial overlap with an array or data-table formula, removal of a table header, totals row or every data row, and reference-bearing package parts they cannot update safely. Inspect the refusal rather than rebuilding the workbook with another tool.

## Rename, delete, hide, move, or copy sheets

Use `sheet rename|delete|hide|unhide|move|copy` for a one-shot edit, or the matching builder methods in [authoring-api.md](authoring-api.md). Sheet names are matched without regard to case, and tab positions start at 1. Read `sheet --help` for exact syntax.

A rename rewrites every reference to the sheet: cell, table, conditional-formatting and validation formulas, defined names, chart series, sparklines and internal hyperlink locations. Text inside formulas, such as an `INDIRECT` or `HYPERLINK` argument, is not a reference; it stays as written and the receipt warns about it.

A delete follows the broken-reference policy of row and column deletes: references to the sheet, or to a table on it such as `Sales[v]`, from elsewhere refuse the delete with their owners listed, unless `--allow-ref-errors` makes them `#REF!`, as Excel does for references to a deleted sheet. A hyperlink location is left as written. A 3-D span such as `First:Last!A1` whose endpoint is deleted narrows by one sheet without an opt-in. The sheet's table, chart, note and picture parts go with it; sheet-scoped names, print areas included, are removed.

Delete, move and copy keep sheet-scoped names and the active tab pointing at the same sheets. The active tab stays visible when its sheet is hidden, and the last visible sheet cannot be hidden or deleted. `hide --very-hidden` also hides the sheet from Excel's Unhide list.

A copy carries cells, styles, merges, conditional formatting, validation, notes, pictures, tables and charts. Its tables get new workbook-unique names, reported in the receipt, and formulas and charts on the copy that referenced the source sheet or its tables refer to the copy. Sheet-scoped names are copied too; hyperlink locations are kept as written. The copy is visible whatever the source's state.

Formulas whose result depends on sheet order or names, such as `SHEET`, `SHEETS`, `CELL` and 3-D spans, lose their cached results. Sheet operations refuse, naming the part, when the sheet owns an object they do not model, such as a pivot table, or when such a part refers to the sheet.

## Define or delete names

Use `names list|set|delete` for a one-shot edit, or `listNames`, `defineName` and `deleteName` in [authoring-api.md](authoring-api.md). A name belongs to the workbook, or to one sheet with `--scope`; names are matched without regard to case. Setting a name that exists in the same scope changes what it refers to, and formulas that use it lose their cached results.

A name must start with a letter, underscore or backslash and continue with letters, digits, periods and underscores. Refused, because Excel would misread them or owns them:

- names that read as a cell reference, such as `A1`, `R1C1` or `R`, and `TRUE` or `FALSE`;
- names starting with `_xlnm.`, which print settings own (set them with `setPageSetup`), or `_xlfn.`, `_xlpm.` or `_xlws.`;
- a name a table already uses; tables and names share one namespace.

References in a name must name their sheet, such as `=Data!$B$2:$B$9` or `=Data!$A:$A`, and the sheet must exist. A relative reference such as `Data!B2` or `Data!A:A` is accepted with a warning, because Excel reads it relative to the cell that uses the name.

A delete follows the broken-reference policy: formulas that would show `#NAME?` refuse it with their owners listed, unless `--allow-ref-errors` accepts that. A sheet-scoped name shadows the workbook's name on its own sheet, and a LET or LAMBDA parameter of the same name is not a use.

## Resize a table or add a totals row

Tables are changed through the builder: `resizeTable` and `setTableTotals` in [authoring-api.md](authoring-api.md). A table name is matched without regard to case.

When `addTable` creates a headered table, its stored header cells and native column names must agree. Empty headers become `Column<n>`, duplicate names receive a case-insensitive numeric suffix, and non-text literal headers become text. Styles are preserved; affected formula caches are invalidated and normalization is reported. Formula headers refuse with `TABLE_HEADER_FORMULA` rather than being replaced. Headerless tables leave their first data row unchanged. Preflight refuses protected, signed, merged/coupled or overlapping table targets before changing headers or parts.

A write never extends a table. Excel grows a table when someone types directly below or right of it, but a file edit does not, so `SUM(Sales[Amount])` would leave the new cells out. When `range set`, `range copy` or a builder write lands in the row directly below a table, or the column directly right of it, the result warns; resize the table to take the cells in. A row below a totals row does not warn, since it is outside the table by intent.

A resize keeps the header in its row and must overlap the current range, with at least one data row. New columns are named from their header cells; an empty header becomes `Column<n>` and a duplicate name gets a number, both written back to the header cell. Calculated columns are filled into new rows, except cells that already hold content, which the receipt lists. The filter follows the new range. It refuses:

- a table with a totals row: remove the totals row, resize, then add it again;
- a range that would overlap another table, the sheet's filter, merged cells, an array formula or a pivot table;
- a shrink that would break a formula: one naming a column that leaves, or reading `[@Column]` from a row that leaves the table.

Chart series read cells by address and are not resized with the table; the receipt names such charts.

`setTableTotals` adds a totals row below the table, or reconfigures an existing one. Each column takes a function (`sum`, `average`, `count`, `countNums`, `min`, `max`, `stdDev`, `var`, `none`), written as `SUBTOTAL` so filtered-out rows are skipped as in Excel, a `{ label }`, or a `{ formula }`. It refuses when the row below the table is not empty. `null` removes the totals row and its cells, and is refused while a formula reads `Table[#Totals]`.

Formulas that read the table lose their cached results.

## Preserve style and formula intent

Existing target cells retain their styles and number formats unless a format change is requested. When adding cells or rows, reuse the matching semantic role from the table, calculated column, total row, neighboring period, or template. Do not copy formatting from the nearest cell when it serves a different role.

Treat text beginning with `=` as literal unless formula intent is explicit. Avoid coercing identifiers, postal codes, or account numbers because they resemble numbers. For dates and numeric values, resolve locale, unit, precision, and date system when ambiguity changes the stored result.

On copy, choose `values`, `formulas`, or `all` according to the requested ownership. Preserve relative and absolute references, calculated-column behavior, and number formats. Read back only the destination facts not already proved by the writer.

## Apply bounded repairs

Use `repair` for a known formula, cell type, date, or validation correction with a specified target and expected prior state. Verify affected formulas or constraints when the repair can propagate. Do not turn repair into an automatic guess about missing data or calculation logic.
