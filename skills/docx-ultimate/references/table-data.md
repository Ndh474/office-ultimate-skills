# Native tables and typed data

`table.import` is a checked authoring operation. It imports local CSV/TSV or cached XLSX values into a native table with explicit DXA widths. Use `read-table-data <input> --format <csv|tsv|xlsx>` to inspect typed data first. XLSX options include an explicit sheet/range and missing-cache policy; use `describe read-table-data` and its table-data contract for exact fields.

Appending or importing a table directly after another table inserts an empty paragraph between them. This keeps native applications from merging their grids and changing their widths; existing table content is preserved.

Preserve the typed receipt: string, number, boolean, date, blank, error, formula cache and unrepresentable date remain distinct. Formula caches are stored observations, not calculated or certified current values. Missing formula caches refuse by default; explicit `missingCache: "preserve"` displays `[formula cache missing]`. Serial 60 in the 1900 date system is tagged unrepresentable rather than silently treated as a real calendar date. The 1904 date system and fractional date/time values are handled explicitly.

```text
read-table-data values.csv --format csv
read-table-data values.xlsx --format xlsx --sheet Data --range A1:D20
export-table report.docx --out table.csv --target table-locator.json --format csv
```

Obtain a table locator from `read` or `inspect`. Export is stored-text CSV/TSV only; it does not infer original numeric/date/formula types. Fields, review and opaque content outside the proved export grammar refuse. Merged cells export anchor text with empty continuation cells. Cell line breaks are native LF; CSV/TSV record line endings are separately selectable.

Quoting preserves delimiters, quotes, newlines and empty strings. It does not prevent spreadsheet applications interpreting formulas or changing types. Formula-looking cells are reported without silently rewriting their text. No spreadsheet application is opened. XLSX export, spreadsheet formatting transfer and recalculation are unavailable.
