# Public authoring API

Use this API for a new workbook that needs tables, charts, formatting or other native objects, or for an edit that adds them to an existing workbook. It is the stable builder surface for ordinary composition; the calls below are the whole of it.

## Environment and import

Use Node.js 22.13 or newer. Import the builder from the skill's bundle, which needs no install step.

The builder runs on the built-in engine and needs nothing but Node. `export()` does not evaluate formulas and reports `calculation.evaluated: false`; unaffected stored results remain, while dependent caches are invalidated; pass `export(path, { recalculate: true })` to evaluate in Calc first. Chart types and series contracts are in [charts.md](charts.md).

For cell-level work that needs no objects or formatting, prefer the CLI: `range set`, `inspect` and `convert`.

```js
import {
  AUTHORING_API_VERSION,
  createXlsxBuilder,
  openXlsxBuilder,
} from "<skill-root>/dist/xlsx-ultimate.mjs";
```

For an absolute Windows import, use a file URL such as `file:///C:/skills/xlsx-ultimate/dist/xlsx-ultimate.mjs`, or convert the path with Node's `pathToFileURL` before dynamic `import()`. A bare `C:/...` import is not a valid Node module URL. Workbook input and output arguments still take ordinary filesystem paths.

`createXlsxBuilder({ sheetName })` creates an in-memory workbook with one sheet. `openXlsxBuilder(source)` loads an existing `.xlsx` or `.xlsm` for editing in place: parts the builder does not touch are published byte for byte. An opened `.xlsm` must be exported as `.xlsm`, preserving its VBA project. Both return a builder with these methods:

| Method | Input | Result |
|---|---|---|
| `listSheets()` | none | Array of worksheet names |
| `addSheet(name)` | non-empty sheet name | Builder |
| `renameSheet(sheet, newName)` | existing sheet; new name unique without regard to case | Builder |
| `deleteSheet(sheet, options?)` | `{ allowRefErrors: true }` permits broken references | Builder |
| `setSheetVisibility(sheet, visibility)` | `visible`, `hidden` or `veryHidden` | Builder |
| `moveSheet(sheet, position)` | 1-based tab position | Builder |
| `copySheet(sheet, newName, options?)` | `{ position? }`, 1-based; default right after the source | Builder |
| `listNames()` | none | Array of `{ name, scope, refers_to, comment?, hidden, built_in }`; `scope` is a sheet name or `null` for the workbook |
| `defineName(name, refersTo, options?)` | formula such as `=Data!$B$2:$B$9`; `{ scope?, comment? }`, `scope` a sheet name | Builder |
| `deleteName(name, options?)` | `{ scope?, allowRefErrors? }`; `allowRefErrors: true` permits `#NAME?` | Builder |
| `insertRows(sheet, at, count?)`, `insertColumns(sheet, at, count?)` | 1-based row number or column number/letters; count defaults to 1 | Builder |
| `deleteRows(sheet, at, count?, options?)`, `deleteColumns(sheet, at, count?, options?)` | starting row or column, count defaults to 1; `{ allowRefErrors: true }` permits broken references | Builder |
| `setValues(sheet, address, matrix)` | rectangular 2D literal-value matrix | Builder |
| `setFormulas(sheet, address, matrix)` | rectangular 2D formula matrix | Builder |
| `getCells(sheet, address, options)` | A1 range; `formulas: false` is optional | `{ values, formulas? }` |
| `addTable(sheet, options)` | `{ range, hasHeaders?, name }` | Builder |
| `resizeTable(table, range)` | table name; A1 range on the table's sheet, header row unchanged | Builder |
| `setTableTotals(table, totals)` | `{ column: function \| { label } \| { formula } }`; `null` removes the totals row | Builder |
| `addChart(sheet, options)` | `{ type?, sourceRange, position?, title?, hasLegend?, yAxisNumberFormat?, comboLineSeries? }` | Builder |
| `listCharts(sheet)` | worksheet name | Array of chart IDs, types, titles, series references and axis format |
| `updateChart(sheet, chartId, changes)` | chart ID from `listCharts`; title, legend, value axis format or series operations | Builder |
| `export(output, options)` | distinct `.xlsx` path for a new workbook; same extension as the opened workbook; `{ overwrite?, recalculate? }` | Publication result |

Formatting, layout, conditional formatting, notes, pictures, hyperlinks, data validation, filters, sorting, sparklines, protection, print setup and document properties are covered in the sections below. Every method that changes the workbook returns the builder, so calls chain.

Chart types, data layouts, numeric preconditions and imported-series editing are defined in [charts.md](charts.md). Read it before creating radar, bubble, stock or 3D charts, or re-ranging an existing chart. `listCharts` reports plot topology, bubble size references, stock roles and fidelity limits; `inspect --kind chart` returns the same summary.

`setValues` keeps leading `=` text literal. Use `setFormulas` for formulas. Both validate the complete matrix before changing cells and follow the range-content ownership rules in [workbook-authoring-and-editing.md](workbook-authoring-and-editing.md). Populate cells before merging or protecting them, or creating table headers that own their content; ordinary table-body edits remain supported. `addTable` normalizes literal header cells to unique text matching the table’s column names and reports changes; formula headers refuse. See the same ownership reference for normalization and refusal details.

Expected input and ownership refusals are checked before live mutation, including composite charts, pictures, notes, conditional formats and protection changes. The builder stays usable after these refusals. Unexpected failures inside a guarded commit or serialization invalidate the session: `WORKBOOK_SESSION_FAILED` prevents later mutations and export. Reopen the unchanged source, or create a new builder. This is failure containment, not rollback of arbitrary external callbacks. Generic protected-sheet edits and OPC-signed package mutations follow the [shared ownership rules](workbook-authoring-and-editing.md#choose-the-preservation-lane).

`export` reloads the candidate and publishes transactionally, evaluating first only when `recalculate` is requested. Its result uses `schema_version: 9` and contains `output_path`, `source_path`, `engine`, `calculation`, `dependencies` (the caches the edit invalidated, as described in [data-and-formulas.md](data-and-formulas.md)), `formulas` (storage rewrites and unknown functions), `structural_edits` for rows and columns, `sheet_edits` for sheet operations, `table_edits` for table creation, resizes and totals, `name_edits` for defined names, `filter_edits` for filters, `sort_edits` for sorts and `layout_edits` for hidden rows and columns, outlines, page breaks and tab colours (each with its changed parts and invalidation; structural, sheet and name edits add any permitted broken references, a copy its new table names, a table creation its normalized headers, a resize its added columns and filled cells, a name its previous formula, a filter its hidden rows, a sort its moved rows, a layout edit how many lines it changed), checks, and warnings. See [workbook-authoring-and-editing.md](workbook-authoring-and-editing.md) for the refusal rules of each.

## Formatting and layout

| Method | Input |
|---|---|
| `setFormat(sheet, address, format)` | A1 range of at most 250000 cells; a format object |
| `getFormat(sheet, address)` | A1 range; returns the top-left cell's format in the same vocabulary |
| `setColumnWidth(sheet, columns, width)` | `"B"` or `"B:D"`; width in characters, 0 to 255 |
| `setRowHeight(sheet, rows, height)` | `3` or `"3:7"`; height in points, 0 to 409 |
| `freezePanes(sheet, { rows, columns })` | rows and columns to keep visible; both 0 unfreezes |
| `mergeCells(sheet, address)`, `unmergeCells(sheet, address)` | A1 range |

A format object may contain:

- `numberFormat`: an Excel format code such as `"#,##0.00"`, `"0.0%"` or `"yyyy-mm-dd"`.
- `font`: `{ bold, italic, underline, strike, size, color, name }`; `underline` is `true`, `false`, `"double"`, `"singleAccounting"` or `"doubleAccounting"`.
- `fill`: a solid colour, or `null` to remove the fill.
- `border`: `{ style, color }` for all four edges, `{ top, bottom, left, right }` for each, `null` on an edge to remove it. Styles are Excel's: `thin`, `medium`, `thick`, `dashed`, `dotted`, `double`, `hair` and the dash-dot variants.
- `horizontalAlignment`, `verticalAlignment`, `wrapText`.
- `locked`, `hidden`: take effect once the sheet is protected. Cells are locked by default, so set `locked: false` on the input cells users should still edit; `hidden` hides a cell's formula.

Colours are hex, `"#1F4E79"`. Only the properties given change; the cell keeps the rest of its existing style, including theme fonts from the source workbook. Merging refuses a range whose cells other than the top-left hold content, because Excel would hide it.

## Conditional formatting

`addConditionalFormat(sheet, address, rule)` adds a rule after the rules the sheet already has, so it never overrides one; `clearConditionalFormats(sheet, address)` removes the rules on exactly that range. A rule is one of:

| `type` | Other fields |
|---|---|
| `cellIs` | `operator` (`greaterThan`, `lessThan`, `equal`, `notEqual`, `greaterThanOrEqual`, `lessThanOrEqual`) with `value`, or `between`/`notBetween` with `values: [low, high]`; `format` |
| `expression` | `formula`, starting with `=` and written relative to the range's top-left cell; `format` |
| `containsText` | `text`; `format` |
| `duplicateValues`, `uniqueValues` | `format` |
| `top10` | `rank` (default 10), optional `percent` and `bottom`; `format` |
| `colorScale` | `colors`: two (low, high) or three (low, mid, high) |
| `dataBar` | optional `color` |

A rule's `format` uses the format vocabulary above, limited to what Excel applies conditionally: `font` colour, bold, italic, underline and strike, `fill`, `border` and `numberFormat`. An operand that is a number stays a number, text starting with `=` is a formula, and other text is compared as text.

## Notes and pictures

| Method | Input |
|---|---|
| `addNote(sheet, cell, text, { author? })` | one cell; text of at most 32767 characters, line breaks allowed |
| `addImage(sheet, { file \| data, cell, width?, height?, description? })` | a PNG, JPEG or GIF path or Buffer; the top-left cell; size in pixels |

A note is the classic cell note that every spreadsheet application shows on hover; a second note on the same cell replaces the first, and notes already in the workbook are kept. Excel's threaded comments are not written. A picture keeps its natural size, or its aspect ratio when only one of `width` and `height` is given; `description` becomes its alternative text. The file type is taken from the image's own signature, and the same image placed twice is stored once. Pictures are limited to 20 MiB and 16000000 pixels. Complete PNG/JPEG/GIF container framing is checked before insertion, including PNG checksums; original bytes and supported palette/16-bit/progressive variants are preserved. This is not a full JPEG/GIF entropy decoder or a guarantee that every viewer decodes every image. Use an independent image reader or inspect a benign rendered picture when visual validity matters. File inputs must be regular, nonsymlink files.

## Hyperlinks and data validation

| Method | Input |
|---|---|
| `addHyperlink(sheet, address, link)` | A1 cell or range; `link` is `{ url }` or `{ location }`, with optional `tooltip`, `text` and `style` |
| `removeHyperlink(sheet, address)` | A1 range; every link leaves its cells |
| `setDataValidation(sheet, address, rule)` | A1 range; a rule as below |
| `clearDataValidation(sheet, address)` | A1 range; its cells lose their rules |

`url` is an `http`, `https` or `mailto` address, stored as given. Any other scheme, such as `file:` or `javascript:`, is refused with `UNSUPPORTED_HYPERLINK_SCHEME`, and spaces in an address must be percent-encoded. `location` is a place in the workbook: a reference on an existing sheet, such as `Data!B5` or `'Q1 Sales'!A1:C3`, or a defined name visible from the link's sheet; anything else is refused with `HYPERLINK_TARGET_NOT_FOUND`. A cell holds one link, so a new link replaces the links on its cells, and an old link keeps its other cells. `text` writes the cell's text and needs a single cell; without it, an empty single cell shows the target.

Linked cells get Excel's built-in Hyperlink cell style: each keeps its own font, fill, border and number format, and the font is underlined in the theme's hyperlink colour. `style: false` leaves the format alone, and `removeHyperlink` takes the style off again. LibreOffice draws every linked cell as a link whatever its style, so a LibreOffice `preview` does not show whether the style was applied.

A rule is one of:

| `type` | Other fields |
|---|---|
| `list` | `values`, an array joined to at most 255 characters without double quotes, or `source`, a formula such as `=$H$1:$H$9` |
| `whole`, `decimal`, `date`, `time`, `textLength` | `operator` (`between` by default, `notBetween`, `equal`, `notEqual`, `greaterThan`, `lessThan`, `greaterThanOrEqual`, `lessThanOrEqual`) and `formula1`, plus `formula2` for `between` and `notBetween` |
| `custom` | `formula1`, true for a valid entry |

An operand is a number or a formula, with or without its leading `=`. Relative references are read from the range's top-left cell, as in Excel: `=B2>0` on `B2:B9` checks each cell against itself. The same rules apply in `repair`'s validation operation.

A cell holds one validation, so cells that had a rule lose it to the new one, and the old rule keeps its other cells. A rule that loses its top-left cell is split into rectangles, and its relative references move with each piece. A split that cannot keep them right, such as one of a rule over several areas, is refused with `VALIDATION_SPLIT_UNSUPPORTED`. Rules stored in the sheet's Excel 2010 extension list are left as they are, and an edit that overlaps one is refused with `VALIDATION_EXTENSION_OVERLAP`.

## Filters and sorting

| Method | Input |
|---|---|
| `setAutoFilter(sheet, address, criteria?)` | A1 range whose first row holds the headers; `criteria` is an array of column criteria, or none for filter buttons alone |
| `clearAutoFilter(sheet, address?)` | none for the sheet's filter; a table's range for that table's |
| `sortRange(sheet, address, keys, { hasHeaders? })` | A1 range; `keys` is an array of `{ column, descending? }`, or of columns, at most 64 |

A sheet has one filter of its own, so `setAutoFilter` replaces an earlier one; a table's range, headers included, filters that table instead, leaving its totals row out. A range that takes in only part of a table is refused with `TABLE_PART_UNSUPPORTED`. A column is a 1-based number within the range or a header text, matched without regard to case. Each column criterion is one of:

- `values`: cells showing one of these texts, numbers or booleans, matched without regard to case, with `blanks: true` to include empty cells as well; `blanks: true` alone keeps only empty cells.
- `conditions`: one or two `{ operator, value }`, joined by `join: "and"` (the default) or `"or"`. Operators are `equal`, `notEqual`, `greaterThan`, `greaterThanOrEqual`, `lessThan` and `lessThanOrEqual`. A number compares numbers; text compares text, where `equal` and `notEqual` take `*` and `?` wildcards, as in `"north*"`, and `~` escapes them. A cell of another kind, or an empty one, satisfies only `notEqual`.

The rows a filter rejects are hidden, decided from the values stored in the file, so the output shows the filtered view without an application. A criteria column holding a formula with no stored result is refused with `FILTER_VALUES_UNKNOWN`; export with `recalculate: true` first to store results. A values list is matched against what the cell displays, which the builder knows for text, TRUE and FALSE, errors, and numbers in the General format; any other number is refused with `FILTER_DISPLAY_UNKNOWN`, and `conditions` compare it by value instead. Setting or clearing a filter shows the rows the earlier criteria hid. Only `SUBTOTAL` and `AGGREGATE` results depend on hidden rows, so those lose their cached results. `clearAutoFilter` shows every row; a table keeps its filter buttons, and the sheet's filter is removed.

`sortRange` orders rows as Excel does: ascending puts numbers first, then text without regard to case, then FALSE and TRUE, then errors; descending reverses that; empty cells come last either way, and equal rows keep their order. Text compares in Unicode order, which can differ from Excel's for punctuation. A table's range or the sheet's filter range keeps its header row in place, and a table's totals row stays below; another range does so with `hasHeaders: true`. Cells move with their values, formulas and styles; a formula's references move with its row, as in a copy. Row heights, conditional formatting, validation and cells outside the range stay. Sorting a table or the sheet's filter records the sort in that filter, as Excel does.

A sort is refused when its rows hold something that cannot move with them: merged cells, shared, array or data-table formulas (`COUPLED_CELL_RANGE_UNSUPPORTED`); notes, hyperlinks, sparklines, pictures or charts anchored in the range, and pivot tables (`SORT_RANGE_UNSUPPORTED`); hidden rows (`SORT_HIDDEN_ROWS`: clear the filter, sort, then filter again); a key cell whose formula has no stored result (`SORT_VALUES_UNKNOWN`); and a formula whose references cannot move with it, such as one using `INDIRECT` or `OFFSET` (`SORT_FORMULA_UNSUPPORTED`). Formulas that read the sorted cells lose their cached results. Neither a filter nor a sort is applied to a protected sheet (`PROTECTED_WORKSHEET_UNSUPPORTED`).

## Hidden rows, outlines, page breaks and tab colour

| Method | Input |
|---|---|
| `setRowsHidden(sheet, rows, hidden?)` | `3` or `"3:7"`; `hidden` defaults to `true`, `false` shows the rows |
| `setColumnsHidden(sheet, columns, hidden?)` | `"B"` or `"B:D"`; `hidden` as above |
| `groupRows(sheet, rows, { collapsed? })`, `ungroupRows(sheet, rows)` | rows as above |
| `groupColumns(sheet, columns, { collapsed? })`, `ungroupColumns(sheet, columns)` | columns as above |
| `addPageBreak(sheet, { row } \| { column })`, `removePageBreak(sheet, { row } \| { column })` | the row number, from 2, or the column letter or number, from B, that starts the new page |
| `setTabColor(sheet, color)` | a hex colour such as `"#1F4E79"`, or `null` to remove it |

Grouping puts the lines one outline level deeper, so grouping inside a group nests it, up to Excel's 7 levels (`OUTLINE_LEVEL_LIMIT`); ungrouping takes them one level out. `collapsed: true` also hides the grouped lines and marks the summary line after them, or before them when the sheet puts summaries above or to the left. Ungrouping leaves hidden lines hidden; show them with `setRowsHidden` or `setColumnsHidden`. Adding a break that exists, or removing one that does not, changes nothing.

A column that had no width of its own is given the sheet's default width when it is hidden or grouped, so it keeps its width when shown again. That default is known when the sheet declares one or its Normal style uses Calibri 11; otherwise the edit is refused with `COLUMN_WIDTH_UNKNOWN`, and `setColumnWidth` on those columns first lets it through. At most 100000 rows can be hidden or grouped in one call.

Showing a row that a filter with criteria may have hidden is refused with `FILTERED_ROWS_UNSUPPORTED`: change or clear the filter instead. Hiding or showing rows clears the cached results of `SUBTOTAL` and `AGGREGATE`, and hiding columns those of `CELL`. On a protected sheet, hiding rows or columns needs `formatRows` or `formatColumns` allowed, grouping and page breaks are refused (`PROTECTED_WORKSHEET_UNSUPPORTED`), and the tab colour can still be set.

## Sparklines, protection, print setup and properties

| Method | Input |
|---|---|
| `addSparklines(sheet, { type?, data, location, color?, markers?, highPoint?, lowPoint?, negativePoints? })` | `type` is `line` (default), `column` or `winLoss`; `data` is a range on the same sheet; `location` is a row or column of cells |
| `protectSheet(sheet, { password?, allow? })`, `unprotectSheet(sheet)` | `password` of 1 to 15 printable ASCII characters; `allow` permits actions such as `sort`, `autoFilter`, `formatColumns`, `insertRows` or `deleteRows` |
| `setPageSetup(sheet, settings)` | `orientation`, `paperSize` (`a4`, `letter`, `legal`, `a3`, ... or an Excel code), `fitToWidth`/`fitToHeight` in pages (0 means as many as needed) or `scale`, `margins` in inches, `printArea`, `printTitleRows` (`"1:2"`), `printTitleColumns` (`"A:B"`), `header` and `footer` as `{ left, center, right }`, `gridLines`, `centerHorizontally`, `centerVertically` |
| `setProperties(properties)`, `getProperties()` | `title`, `subject`, `author`, `keywords`, `description`, `category`, `lastModifiedBy`, `company`, `manager`; `null` removes one |

Sparklines follow Excel's pairing: a column of N location cells takes N rows of `data`, a row of N cells takes N columns. Header and footer text may use `{page}`, `{pages}`, `{date}`, `{time}`, `{sheet}` and `{file}`; a literal `&` is kept as text. Sheet protection uses Excel's legacy password check, which stops accidental edits but is not security: anyone can remove it. Protection `allow` flags describe permissions in an opening spreadsheet application. Generic builder/CLI modifications still refuse a protected sheet until it is explicitly unprotected; only the documented layout operations honor their own exceptions.

LibreOffice imports these sparklines faithfully, but its PNG and PDF output draws them unreliably, so a missing or faint sparkline in a LibreOffice `preview` is not evidence of a broken file; report this Calc rendering limitation when their appearance matters; a missing preview is not proof of a native-object defect.

## Representative create and edit

```js
const draft = await createXlsxBuilder({ sheetName: "Data" });
draft
  .setValues("Data", "A1:C3", [["Category", "Amount", "Double"], ["A", 2, null], ["B", 3, null]])
  .setFormulas("Data", "C2:C3", [["=B2*2"], ["=B3*2"]])
  .addTable("Data", { range: "A1:C3", hasHeaders: true, name: "DataTable" })
  .addChart("Data", {
    type: "bar",
    sourceRange: "A1:B3",
    position: { from: "E1", to: "K12" },
    title: "Amounts",
    hasLegend: false,
  });
await draft.export("draft.xlsx");

const edited = await openXlsxBuilder("draft.xlsx");
edited.setValues("Data", "B2", [[5]]);
await edited.export("edited.xlsx");
```

## Limits and routing

This API deliberately exposes only the operations documented above. It does not expose the engine workbook object as a public contract. Use the CLI for bounded range edits, inspection, conversion, repair, render, and audits.
