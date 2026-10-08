# Requests and results

Import `executeDocx` from `dist/docx-ultimate.mjs`. Importing the module does not run the command line.

Available structured requests include:

```js
const result = await executeDocx({ schema_version: 2, operation: "capabilities" });
const diagnosis = await executeDocx({ schema_version: 2, operation: "doctor" });
const content = await executeDocx({ schema_version: 2, operation: "read", input: "report.docx", options: { view: "current", format: "text" } });
```

Requests use schema version 2. Unknown fields, unsupported versions and unavailable operations return a structured refusal or failure. There is no automatic reinterpretation of another request format.

Command results contain `schema_version`, `command`, `status` and `limitations`, with `result` or `error` when applicable. Status is `succeeded`, `refused` or `failed`. A handled command emits JSON to standard output; command exit codes are 0 for success, 1 for refusal and 2 for invalid usage or execution failure. Read the error code and limitations rather than treating the existence of output as success.

Native document processing, when available, uses LibreOffice Writer. There is no application selector or fallback. The current capability response reports native work as not probed; it does not launch LibreOffice.

`doctor` uses the diagnostic result family: `schema_version: 1`, `operation`, `status`, `checks`, `artifacts` and `limitations`, plus `error` when applicable. This diagnostic family is distinct from schema2 document-command results. Both use the same status and exit-code meanings. Resource or native execution faults fail diagnosis; ordinary missing prerequisites appear in a successful diagnosis with native execution and acceptance `not_probed`.

Native discovery requires stable LibreOffice 26.2.5 or 26.8.0 and newer, plus Poppler `pdfinfo`, `pdftoppm`, `pdftotext` and `pdffonts`. Prerelease builds do not qualify. `DOCX_ULTIMATE_SOFFICE`, `DOCX_ULTIMATE_PDFINFO` `DOCX_ULTIMATE_PDFTOPPM`, `DOCX_ULTIMATE_PDFTOTEXT` and `DOCX_ULTIMATE_PDFFONTS` may each name an explicit executable; an override is exclusive, with no silent fallback. Installing or changing system tools requires the user's authorization.

Use `describe` or `describe <operation>` to inspect supported request options. `run --request <local-json-file>` accepts the same structured request used by the API; the file must be valid JSON and no larger than 4 MiB. Read/search commands do not accept an output path. Format options affect the result payload, while the command still emits one structured JSON receipt.

`validate` returns `valid_within_declared_coverage` together with checked and unchecked parts/namespaces/branches. A failed declared check returns status `refused` with `VALIDATION_FAILED` and the bounded evidence. Validation does not certify native safety, layout or field freshness; unknown coverage is never silently promoted to valid.
