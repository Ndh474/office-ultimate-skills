# Create a presentation

Use `node <skill>/dist/pptx-ultimate.mjs schema deck` for the current closed JSON contract. Treat schema output as a request shape, not permission to use arbitrary style keys in every context. Unknown fields and unsupported semantics refuse.

Run:

```sh
node <skill>/dist/pptx-ultimate.mjs create deck.json output.pptx
node <skill>/dist/pptx-ultimate.mjs validate output.pptx
node <skill>/dist/pptx-ultimate.mjs render output.pptx --out-dir preview --format both --include-hidden
```

Quote local paths. Output must differ from the spec and every asset. An existing output refuses unless `--overwrite` is explicit; the prior file survives handled failure. Use a new dedicated render directory, not an arbitrary project folder.

## Model the requested content

A deck has `schema_version: 1`, `presentation`, `assets`, `layouts` and nonempty `slides`. The presentation can set point dimensions, theme and explicit metadata. Default canvas is 960×540 points. Coordinates and box extents are points; image sizes are pixels. There is no automatic fit, shrink or overflow repair.

Use unique semantic `id` handles for layouts, slides and objects. Optional display names may repeat. A slide's `layout` selects its handle; omission uses the generated blank layout. Slide order is array order. `hidden: true` persists hidden state.

Text is paragraphs with ordered runs. A run has either `text` or `break: true`. Use separate paragraphs for paragraph boundaries and explicit break runs for line breaks; raw CR/LF inside run text refuse. Paragraph lists are `none`, `bullet` or `number`, with supported levels 0–8. New explicit lists reserve marker spacing according to the paragraph text size; inherited lists retain layout defaults. Fonts, sizes and color must be deliberate; package success does not prove the font exists on the render host.

For characters outside the selected font's coverage, choose an appropriate installed font on the affected runs rather than dropping or substituting the characters. Keep the surrounding text's intended font. Emoji/color-font support depends on the installed font and native renderer; a valid font-family string does not prove coverage. Review the actual glyphs and line fit after changing fonts, including note text when relevant.

Reusable layout placeholders declare an exact role/index slot and owning geometry. Slide placeholders bind the selected layout's role/index and may omit geometry or style to inherit. Local overrides affect the slide only. Never guess a slot from its display name. Prompt run formatting is not a shared inherited default; the owning placeholder's object and paragraph/list defaults define native levels.

Optional slide `notes` are structured paragraphs. Omission means no notes part; an empty array deliberately creates an empty notes body. Do not put speaker-only content on a visible slide as a substitute.

## Choose native objects

- Text, rectangle, roundedRectangle, ellipse and line use native OOXML objects. A line may have one zero extent, not both; text/fill is not a line property.
- Pictures refer to declared PNG/JPEG or supported static SVG asset IDs. CLI asset paths resolve beneath the JSON file's directory. Preserve original image bytes. `contain`, `cover` and `stretch` have explicit point-box semantics; SVG retains its vector and generated PNG fallback under the [bounded profile](advanced.md). Crop fractions trim source edges first. Alt text describes the image rather than duplicating a filename.
- Tables use rectangular arrays of literal strings. Leading zeros, equals and whitespace stay literal. Optional row/column dimensions sum to the box; sparse cell-style overrides are zero-based. Rectangular merges, row/column styling and per-edge borders follow [advanced content](advanced.md); formulas are not evaluated.
- Basic charts are clustered column/bar, nonstacked line with optional markers, or single-series pie. Supply complete numeric values and nonempty string categories/series names. Every chart owns a formula-free embedded workbook. Optional title/legend/series colors are chart fields, not generic shape styling. Missing/null/sparse points, formulas and unsupported plot families refuse.
- For stack/percentage/area/doughnut, numeric XY scatter/bubble, radar, nested groups and attached straight/elbow connectors, use [advanced content](advanced.md). Their fields and coordinate semantics differ; do not simulate them by mislabeling a basic chart or loose line.
- Ordinary text/shape/picture links may be HTTP, HTTPS, mailto, or `slide:<semantic-slide-id>`. The engine records them without following them.

The bounded profile and resource ceilings are in [scope](scope.md). For example, PNG/JPEG orientation and format refusals are not an invitation to silently convert the user's source. Acquire or transform a replacement only when the task permits it.

## Finish with evidence

Validate the exact published PPTX, then render all slides including hidden ones. Open every generated PNG. Check text clipping, missing objects, intentional space, image crop, native table readability, chart values/legends and inherited styling. Correct the source specification and regenerate if necessary. Preserve the user's requested native object type instead of hiding a failed feature in a screenshot.
