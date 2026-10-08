# Setup and native evidence

## Requirements by operation

The self-contained `dist/xlsx-ultimate.mjs` CLI/API needs Node.js 22.13 or newer, with no install step for the skill itself.

Keep the complete skill folder when copying it. The repair schema and `dist/windows-job.cs` are required runtime resources.

| Operation | Additional tools |
|---|---|
| Read/write/repair XLSX or XLSM, native objects, CSV/TSV, stored-value analysis | None |
| Recalculate formulas, convert ODS | LibreOffice Calc |
| Validated whole-workbook PDF | LibreOffice Calc and Poppler `pdfinfo` |
| Validated single-sheet/range PNG | LibreOffice Calc, Poppler `pdfinfo` and `pdftoppm` |

Calculation evaluates a validated throwaway copy in LibreOffice Calc, then grafts only trustworthy computed results into the custom OOXML candidate. Calc never resaves the user's source. Rendering produces separate PDF/PNG artifacts; ODS conversion is an explicitly lossy format boundary. Whole-workbook PDF excludes hidden and veryHidden sheets, including sheets with explicit print areas. This print selection changes only the native working copy. An explicitly selected hidden sheet can still be shown through `preview`.

Use a maintained, patched [LibreOffice](https://www.libreoffice.org/download/) and [Poppler](https://poppler.freedesktop.org/) from official or trusted platform packaging. Executable discovery and a successful sample do not certify every feature or an entire release; a LibreOfficeDev/alpha version remains development-runtime evidence.

## Diagnose prerequisites

Run:

```bash
node <skill-root>/dist/xlsx-ultimate.mjs doctor
```

The diagnostic result separates `checks.engine`, detected identities in `checks.native`, and `checks.native.readiness.recalculate`, `.pdf` and `.png`. Each readiness value reports prerequisites, not verified application correctness. The output lists exact native tool versions/paths. A failed version command, timeout, interruption or ownership fault is a failed diagnostic with its error evidence; it is not reported as an absent tool. `render-pdf` may be available while `preview-png` is not; neither is required for Node-only operations.

LibreOffice is searched on PATH and at standard OS locations:

- Windows: `%ProgramFiles%\LibreOffice\program\soffice.com` or `soffice.exe`, including the `(x86)` installation roots
- macOS: `/Applications/LibreOffice.app/Contents/MacOS/soffice`
- Linux: common `/usr/bin`, `/usr/lib/libreoffice`, `/opt/libreoffice`, and Snap locations

Override individual executable locations with `XLSX_SOFFICE`, `XLSX_PDFINFO` or `XLSX_PDFTOPPM`. ODS conversion also accepts `--libreoffice-path <soffice>`. Overrides must identify the expected tool; merely exiting successfully for a version command is insufficient. No other application or rasterizer is used as a fallback.

Each native job owns a private writable profile/work directory and process scope. Timeout, output-limit and command cancellation end only owned processes. If cleanup cannot be confirmed, the error reports retained work directories; do not delete them while an owned process may still be using them. A private profile or process job is not a security sandbox.

On Windows, native operations require Windows 10 or newer on x64 and the existing .NET Framework64 C# compiler under the absolute Windows system root. The shipped, hash-checked helper is compiled in a private directory and assigns a suspended child to an invocation-owned Job Object before execution. Missing compiler/platform/job capabilities refuse before the tool is launched; no unowned fallback is used. Native process bounds are 1–600000 ms and at most 1 MiB captured output on Windows. POSIX timeouts cannot exceed 2147483647 ms, the timer limit; operation-specific limits can be stricter. Node-only workbook operations do not need the compiler.

## Run a known-answer sample

```bash
node <skill-root>/dist/xlsx-ultimate.mjs native-smoke --out-dir <fresh-directory>
```

This calculates a generated sample, compares expected results and validates PDF/PNG through Calc and Poppler. Evidence remains in a fresh directory, or a new temporary directory when `--out-dir` is omitted. An existing output directory refuses unchanged. The response gives `evidence_directory`, `report` and artifact paths with sizes and hashes. Inspect the rendered sample if appearance matters.

Checks apply to that sample and exact tool versions. Table results withheld by calculation policy are reported. UNIQUE is checked at its anchor because Calc may not expand a newly authored spill; test the full output range separately when required.

`doctor` and `native-smoke` return top-level JSON with `schema_version`, `operation`, `status`, `checks`, `limitations` and any `artifacts` or `error`. Their status is `succeeded`, `refused` or `failed`, with exit codes 0, 1 or 2 respectively; invalid usage is failed/2. JSON goes to stdout on failures too. Doctor can succeed while reporting unavailable prerequisites: native execution and acceptance remain `not_probed`. A smoke prerequisite refusal is distinct from an executed sample failure. Keep the returned evidence and limits when reporting either outcome.

## Native safety and validation

Before any native handoff, bounded package/XML/formula checks reject executable formula channels, external refresh/data connections, unsupported active/embedded content and incomplete scan coverage. ODS receives format-aware checks for scripts, events, encrypted content and externally loaded resources. Embedded ODS subdocuments are unsupported. XLSX-to-ODS can therefore refuse before publication when the generated ODS contains a chart object; an ODS containing such embedded objects also refuses as input. Plain safe ODS remains supported. Safe inert literal links differ from automatically fetched content. Native `INDIRECT` targets must be provably local literal references; computed or external targets are refused because their resource access cannot be established safely. Native SVG is restricted to understood passive primitives and local-fragment references; CSS/stylesheets, base-URL inheritance, animation, events and foreign content refuse. VML uses an inert note/shape subset. Declared image types must agree with their bytes; opaque or unsupported media refuses instead of being entrusted to native type sniffing. ODF table/style/text expressions use checked formula dialects; unrecognized executable or geometric expressions refuse. These restrictions apply to native handoff, not to ordinary preservation of unevaluated package parts. Read-only and unevaluated OOXML work remains available when a native operation is refused.

A macro-enabled source is never opened directly. The safe copy removes VBA and refuses macro sheets, ActiveX/OLE and possible VBA UDF dependencies. The original XLSM remains unchanged. Do not work around a refusal by opening the original in Calc.

PDF validation checks actual parse/page/geometry facts and absence of encryption/JavaScript. PNG validation includes full bounded decoding, chunk checksums and output dimensions. Requested dimensions are 64–10000 per axis and at most 16000000 pixels; previews preserve aspect ratio and stay inside the requested box. Rendering follows Calc → PDF → Poppler; receipts name actual tools and checks. Inspect the image when appearance matters; structural validation is not a visual review.
