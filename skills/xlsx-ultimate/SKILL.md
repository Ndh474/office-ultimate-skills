---
name: xlsx-ultimate
description: Create, inspect, modify, analyze, convert, or verify XLSX artifacts, including existing XLSM workbooks. Use when these workbook files are an input or requested output, for package-preserving edits, typed cells, formulas, native structures, rendering, or CSV, TSV and ODS exchange. Do not trigger for tabular prose or work solely inside a cloud spreadsheet service.
---

# XLSX Ultimate

Treat business meaning, source data, assumptions, and required calculations as inputs. This skill owns their faithful XLSX representation, preservation of the existing workbook, and evidence about the resulting file.

## Establish the operation contract

Resolve the operation, paths, target sheets or ranges, controlling template, native structures, calculation expectations, and the application the result must work in. Preserve data types and distinctions that affect the stored result.

For an existing XLSX or XLSM, preserve content, formulas, formatting, behavior, and review state outside the requested change. Inspect only relevant ranges, dependencies, and objects. A template controls established mechanics to the requested match level, not its old data.

Run `node <skill-root>/dist/xlsx-ultimate.mjs <command> ...`. The core CLI/API needs Node.js 22.13 or newer and no install step. Keep the complete skill folder so its schema and distribution resources remain available. `--help` works in any argument position and lists supported options and representative examples.

Output is JSON by default; `--human` prints a short summary. Read the JSON: the receipt carries the checks, changed parts, calculation state, and limitations that the summary omits.

Reading, writing, repairing, converting to CSV or TSV, auditing, and authoring formatting and native objects need nothing but Node. Evaluating formulas and ODS conversion need LibreOffice Calc. Validated PDF additionally needs Poppler pdfinfo; PNG also needs pdftoppm. Run `doctor` to inspect this machine’s operation-specific prerequisites; read [setup.md](references/setup.md) for tool locations and native evidence. Keep task files outside the skill.

Use `native-smoke` when you need a known-answer calculation and render check on this machine. It retains sample evidence and reports its limits; it does not verify your workbook.

## Choose the least destructive capable path

| Need                                        | Path                                                              |
| ------------------------------------------- | ----------------------------------------------------------------- |
| Read bounded workbook state                 | `inspect` or `range get`                                          |
| Write literals or formulas                  | Targeted `range set --values` or `--formulas`                     |
| Insert or delete rows or columns            | Guarded `rows` or `columns` command, or the builder methods       |
| Rename, delete, hide, move, or copy a sheet | Guarded `sheet` command, or the builder methods                   |
| List, define, or delete a defined name      | Guarded `names` command, or the builder methods                   |
| Resize a table or set its totals row        | Builder `resizeTable` or `setTableTotals`                         |
| Clear, copy, repair, or edit objects        | Matching guarded command with exact target and expected state     |
| Add formatting or native objects            | Stable builder in [authoring-api.md](references/authoring-api.md) |
| Create, inspect or edit charts               | Builder methods and [chart contracts](references/charts.md)        |
| Convert, render, compare, or audit          | Matching conversion or evidence lane                              |

Read [runtime-and-operation-routing.md](references/runtime-and-operation-routing.md) when the need is not an obvious range edit, or when choosing between a command and the builder.

XLSX/XLSM edits preserve package parts outside the operation’s ownership; unsupported edits refuse rather than reconstructing the workbook. A refusal is a stop signal: it names the structure or part at risk. Do not work around it by rebuilding the workbook with another tool or flattening native objects. Read [workbook-authoring-and-editing.md](references/workbook-authoring-and-editing.md) for creation, existing-file edits, repairs, and structural ownership.

An existing `.xlsm` can be read and edited with the same workbook operations as `.xlsx`. Keep the output `.xlsm`: the macro project is preserved byte for byte, and calculation or rendering opens only a temporary `.xlsx` copy with VBA removed. Creating `.xlsm` or converting it to `.xlsx` is unsupported. Read [interop-and-conversion.md](references/interop-and-conversion.md) before extracting it to another format.

## Preserve artifact fidelity

- Keep the source immutable and write to a distinct output.
- Prefer targeted part edits over whole-workbook import/export.
- Preserve IDs, names, relationships, native objects, comments, hidden/protected state, calculation settings, validations, formats, filters, print settings, and unknown parts outside the operation.
- Preserve styles and number formats. Add cells or objects using the matching table, row, column, or template role.
- Keep formulas, tables, charts, validation, and filters native when behavior or editability matters.
- Treat merged cells, array/shared formulas, data tables, calculated columns, headers, totals, and other coupled ranges as owned structures. Use a supported structural operation or refuse.

## Keep calculation evidence distinct

Literal writes include strings beginning with `=`; only formula operations create formulas. Delimited imports remain literal unless coercion is explicit. Preserve identifiers, date system, precision, blanks, and errors.

Formula text, static references, stored cell or chart caches, and freshly evaluated results are different evidence. Do not infer recalculation or dependency health from an old cache.

For array formulas, check the full output range, including spill cells beyond the anchor, after recalculation.

Reads report stored values unless `--recalculate` is passed, and say which. `range get` and `inspect` also report date-format evidence beside raw numeric values; use the ISO value for unambiguous dates and check unsupported-format status. A write clears the cached result of every formula that can depend on the edit and marks the workbook to recalculate on open, or, with `--recalculate`, stores fresh results; the receipt says when coverage was partial. Fresh results come from Calc, and the receipt names the exact runtime. Results not established as trustworthy for that version are withheld rather than stored. Native evidence is operation-specific and does not establish Excel equivalence. Read [data-and-formulas.md](references/data-and-formulas.md) for the flags, coverage and receipt fields, and for formula, dependency, profiling, or reconciliation work.

## Verify only affected behavior

Reuse writer candidate checks and exact target readback. Do not repeat equivalent evidence on an unchanged output.

- Local edits need target preconditions and intended stored values or formulas; inspect dependencies only when downstream impact matters.
- Structural or guarded edits need evidence for owned identities, relationships, calculation state, and unowned features.
- New workbooks need inspection of requested sheets, types, formulas, tables, charts, and representative outputs.
- Render only when formatting, charts, print layout, widths, hidden ranges, glyphs, or number formats affect acceptance.
- Run specialized audits only for an actionable request or detected risk.

Read [quality-and-risk-gates.md](references/quality-and-risk-gates.md) for non-trivial verification and [interop-and-conversion.md](references/interop-and-conversion.md) before crossing a format or application boundary. Local evidence does not prove how another application behaves.

## Deliver the artifact

Return the XLSX or findings with the output path, material changes, decision-relevant checks, and limitations. Preserve material retained-file warnings and any interrupted-publication outcome from the [verification guidance](references/quality-and-risk-gates.md#publication-and-interruption). State whether results were evaluated, and by what, because a reader that does not recalculate can show missing or stale cached results. Do not deliver internal diagnostics, temporary renders, or task scripts unless requested or intended as evidence.
