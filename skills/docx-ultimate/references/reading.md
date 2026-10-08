# Stored content and exact targeting

Use the bundled executable with these commands:

```text
read report.docx --view current --format text
inspect report.docx --format json
search report.docx --pattern "exact phrase"
search report.docx --pattern "item\\s+[0-9]+" --regex --flags u
```

The views are `current`, `original` and `markup`. They describe the selected stored text model, not application pagination or a rendered review view. Move, paragraph-mark and table-row revision forms without a proved text interpretation are explicitly incomplete; never interpret an incomplete view as the full document. Unknown embedded content and orphan stories appear in coverage or diagnostics rather than disappearing silently.

Read formats are `json`, `ndjson`, `text`, `markdown` and `outline`. NDJSON contains typed items; text and Markdown are convenience projections, not a lossless document conversion. Equation items expose stored OMML tokens without claiming full mathematical meaning or native editability. Field results remain stored caches with freshness `not_evaluated`.

Check `coverage.complete`, `coverage.truncated`, field/outline coverage and `diagnostics`. Bounded output may be truncated or refused. Use the command's `describe` result for configurable ceilings. Do not request a higher limit to bypass a refused structural or review boundary.

Search uses exact Unicode text without normalization. Regex requires the Unicode `u` flag and runs in a bounded, terminable worker. Ranges use Unicode code points. Each match carries a schema2 source locator bound to the input digest, physical story, original path, node fingerprint and exact expected text/count. Preserve the locator as returned; a path or repeated label alone is not edit authority. Cached-field and review ranges can be read-only.
