# Runtime and operation routing

Discover exact flags from `node <skill-root>/dist/xlsx-ultimate.mjs --help` and `<command> --help`, which accept `--help` in any position and list supported options and representative examples. For `repair`, the operation schema is in `schemas/repair-operations.schema.json` and `repair --describe`.

## Runtime and entrypoints

The built-in `ooxml` engine edits the package in place; LibreOffice calculates through a safe separate-process boundary; PDF/PNG evidence additionally uses Poppler. Read [setup.md](setup.md) for native prerequisites. Run `doctor` to discover tools and operation-specific readiness before assuming a native operation can run.

| Need | Preferred entrypoint | Decision boundary |
|---|---|---|
| New minimal workbook | `create` | Add substantive content with a deliberate builder or subsequent edits. |
| Read structure or a bounded region | `inspect`, `range get` | Read values and formulas as different evidence. `range get` reports the values stored in the package and labels them unevaluated; `--recalculate` evaluates in Calc instead, without changing the package. |
| Set values or formulas in a region | `range set` | Targeted worksheet-part lane. Untouched package parts stay byte-identical. It refuses coupled ranges such as merged cells, array/shared formulas, protected sheets, and table headers or totals. |
| Clear or copy a region | `range clear`, `range copy` | Rewrites only the parts it edits. Choose the copy mode explicitly; a formula whose references cannot be translated safely is refused. |
| Insert or delete whole rows or columns | `rows insert|delete`, `columns insert|delete`, or matching builder methods | Shifts cells, references and supported objects across the workbook. A delete that would create `#REF!` is refused unless explicitly allowed. Read [workbook-authoring-and-editing.md](workbook-authoring-and-editing.md) before applying it. |
| Rename, delete, hide, move, or copy a sheet | `sheet rename|delete|hide|unhide|move|copy`, or matching builder methods | Rewrites references to the sheet across the workbook. A delete that would create `#REF!` is refused unless explicitly allowed. Read [workbook-authoring-and-editing.md](workbook-authoring-and-editing.md) before applying it. |
| List, define, or delete a defined name | `names list|set|delete`, or matching builder methods | Workbook or sheet scope. A delete that would leave `#NAME?` is refused unless explicitly allowed. Read [workbook-authoring-and-editing.md](workbook-authoring-and-editing.md) for the naming rules. |
| Correct a known cell or validation rule | `repair` | Supply the expected prior state; do not use repair as an unreviewed cleanup sweep. |
| Preview a sheet/range or render a PDF | `preview`, `render` | Inspect the produced image or PDF when presentation matters. `preview` produces one page: `--range` scaled to fit, or the first printed page of `--sheet` (default: the first sheet); use `render` for every page. |
| CSV, TSV, or ODS exchange | `convert` | Choose typed/date/formula policy explicitly and assess format loss. |
| Package, design, provenance, accessibility, or security review | `audit`, `security` | Run the mode that answers the current question, not every mode by habit. |
| Compare two workbooks or rendered PNGs | `interop diff`, `interop compare`, `visual-diff` | `interop diff` shows stored cell changes; `interop compare` shows package-part changes. Neither establishes application fidelity. |

The CLI is not a universal authoring API. For formatting, tables, charts, hyperlinks, data validation, filters, sorting, hidden rows and columns, outlines, page breaks or other native objects, in a new workbook or an existing one, use the stable builder in [authoring-api.md](authoring-api.md). Read [charts.md](charts.md) for supported chart families, source layouts and editing limits. For stored range profiling, reconciliation and formula inventory, use `analyze` as described in [data-and-formulas.md](data-and-formulas.md). Keep outputs in the task workspace, not in the skill directory.

When neither a command nor a builder method covers the requested feature, say so. Do not edit the package's ZIP or XML by hand as a workaround.

`capabilities` reports the current operation registry. It is useful for choosing a path when a requested feature is unclear; it is not a substitute for reading the input workbook or running the selected operation.

`doctor` diagnoses executable identities and separate calculation/PDF/PNG prerequisites. `native-smoke` runs a known-answer sample through Calc and Poppler and retains its evidence. Discovery does not establish native behavior; a successful sample does not certify a different workbook.
