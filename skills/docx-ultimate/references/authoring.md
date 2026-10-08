# Authoring and precise edits

Commands share the same checked operation registry as the API and builders. Use `describe` for the finite contracts of all currently available operations. The contract vocabulary is descriptive; runtime validation remains authoritative.

A batch is `{ "schema_version": 2, "operations": [...] }`. Each operation has `op`, an exact `expect: { "count": 1 }`, operation-specific `value`, optional `target`, and optional unique output name `as`.

```json
{
  "schema_version": 2,
  "operations": [
    { "op": "paragraph.append", "value": { "text": "Report" }, "expect": { "count": 1 }, "as": "title" },
    { "op": "run.append", "target": { "type": "output-reference", "name": "title", "kind": "paragraph" }, "value": { "text": " — draft", "format": { "italic": true } }, "expect": { "count": 1 } }
  ]
}
```

```text
create --out report.docx --batch batch.json
edit source.docx --out revised.docx --batch edits.json
markdown report.md --out report.docx --base-dir assets
```

Use distinct input/output files. Existing output files require explicit `--overwrite`; source and protected asset aliases still refuse. DOCX/DOTX output extensions must match package kind. There is no in-place edit mode.

For a source text edit, take the search result's locator unchanged and include its exact text expectation. Source targets are resolved before the first mutation. Overlapping source writes refuse; ordered edits of newly created objects require a proved transition. Ordinary refusal occurs before mutation; a failed mutation prevents publication.

Supported families include paragraphs/runs/headings, named styles, numbering/restarts, selected formatting, bounded local fields, properties/settings, native table grids, inline images, section geometry/header/footer variants, bookmarks/links/notes and scalar templates. `describe <operation>` gives precise selectors and limits. Shared numbering changes require a fresh identifier and explicit paragraph assignment. Editing restrictions are ordinary document settings, not encryption or a password-security promise.

Creation normalizes CRLF, CR and LF text line endings into native break elements. Existing unowned XML text is preserved. Markdown supports a bounded CommonMark profile with tables, strikethrough and local raster assets. Unsupported HTML or lossy constructs produce diagnostics; creation refuses those diagnostics instead of silently discarding content.

Scalar templates retain unrelated content. Expected counts, unknown keys, locked controls, bound/repeated controls and unproved field/review regions are checked. Repeated/conditional/bound templates and blank-scaffold generation are not available.

The importable helpers are `createDocxBuilder`, `openDocxBuilder`, `docxOutputReference`, `compileMarkdown` and `createMarkdownDocx`. A builder collects operations with `add`, `batch` or convenience methods, then uses `build({ output, overwrite })`. Helper option errors throw typed errors; `executeDocx` returns structured receipts. Builders do not expose mutable sessions or raw-XML escape hatches. Read the published output again to obtain locators for a later edit; handles from one builder do not authorize another session.
