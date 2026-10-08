# Supported profile and limits

The skill authors and edits native PPTX package content with a custom OOXML engine. LibreOffice Impress is its sole application target for static rendering. Core create/edit/inspect/validate work without a native app. Use the bundled public CLI/API and its reported request schemas.

## Supported native profile

Supported authoring includes explicit themes, reusable layouts, native placeholders, structured text/lists, rectangle/roundedRectangle/ellipse/line shapes, PNG/JPEG and bounded static SVG with generated PNG fallback, inert ordinary links, literal rectangular tables with bounded merges, clustered column/bar, nonstacked line and single-series pie charts with literal embedded workbooks, notes, hidden slides and explicit metadata. The additional bounded profiles are stacked/percent column-bar and area, single-ring doughnut, marker-only numeric XY scatter, radar/bubble, bounded column+line/secondary axes, chart presentation controls, nested axis-aligned groups and bounded straight/elbow connectors with arrows and cross-group attachments. See [advanced content](advanced.md) for their exact contracts. Source-bound edits include precise literal-run changes and controlled diagram/slide/reference/section changes.

Generic inspection/validation may preserve or inventory a broader input than can be edited or rendered. `valid` means observed checks found no failure; review every coverage field. Unknown extension branches, unsupported table/chart structures and unrecognized inner packages are not silently certified. Unowned payload preservation is distinct from understanding an object well enough to mutate it.

Deferred: SmartArt construction, arbitrary group transforms/reparenting, 3-D and arbitrary mixed/axis chart profiles, obstacle-avoiding connector routing/retargeting, formula workbooks/live data, arbitrary animations/Morph/triggers, audio/video playback, modern review editing, macros/ActiveX/OLE, signature mutation, general Strict conversion and cloud-slide services. Do not emulate a requested native feature with an image or strip unsupported data to obtain a pass.

## Practical bounds

Use `doctor` for exact current resource ceilings and `schema deck`/`schema edit` for closed fields. Important semantic limits include:

- Canvas dimensions 72–4032 points; ordinary positive boxes, with a bounded line zero-extent exception. No automatic shrink/fit.
- Raster PNG/JPEG: 64 MiB encoded/image, 25 million decoded pixels, 256 MiB captured image bytes per request. Nonidentity EXIF orientation refuses. No remote fetch, path traversal or descendant symlinks in asset capture. SVG uses the narrower static grammar and rasterizer bounds in [advanced content](advanced.md); EMF remains opaque/native-unsupported.
- Up to 10,000 cells per native table; literal strings, explicit rectangular merges with empty covered cells; no formula coercion.
- Up to 10,000 plotted points/chart, 100 series and 32,767 characters/label. Complete finite numeric arrays only. Pie values nonnegative with a positive total. Chart labels/titles containing raw CR or `_xHHHH_`-shaped SpreadsheetML escape spellings refuse; do not silently normalize them to force acceptance. Stacked/area/doughnut/scatter/radar/bubble chart profiles accept zero or numeric magnitude 1e-12–1e12; stack/percentage data is nonnegative, and each percentage category needs a positive total.
- Diagrams: at most 8 group levels and 10,000 native nodes per drawing part; cumulative scale 1/64–64 per axis and full composed corners within ±4032 points. These supplement request/package limits. Unsupported or omitted diagram coverage cannot satisfy creation, affected-edit or native handoff checks.
- Recognized chart workbooks are formula-free and bounded independently: 20 entries, 32 MiB each and 128 MiB cumulative expanded bytes. They do not activate a spreadsheet engine or recursive arbitrary-package traversal.
- Native output at 96 DPI, at most 1,000 pages, 25 million pixels/image and 512 MiB per managed set. Default native-stage timeout 120 s; explicit 1–600 s. These are not whole-job or total-process RSS guarantees.

## Runtime prerequisites and evidence

Native adapters implement macOS x64/arm64, Linux x64 and Windows 10+ x64. Windows also needs the stock .NET Framework compiler for its process-ownership helper. Package operations remain independent of native tools. See [rendering](rendering.md) for executable discovery and each adapter's cleanup boundary.

Run `doctor` on the actual host to inspect prerequisites and identities. Native execution remains `not_probed` until a real operation runs: use `native-smoke` for a narrow known-answer check, then render and inspect the requested presentation. An implemented platform or successful version probe is not certification of app initialization, fonts, filesystem behavior, arbitrary documents or future versions.

Choose installed fonts that cover the required characters; default fallback may omit glyphs or change their appearance. Check actual glyphs and line fit after rendering. No font is promised to cover arbitrary Unicode. See [creation](creation.md).

## Assessment and retained compatibility deviations

Read `assessment.scope` with `assessment.grade`. `clean` requires complete applicable coverage; graph-only inspect does not establish XSD conformance. `severe` records failed applicable checks; null means insufficient coverage to grade. A named `compatibility_warning` does not grant general editing or native handoff.

Strict validate retains its failed status for signed chart axis IDs and unused content-type declarations. Explicit `compatibilityPolicy: "preserve-known"` permits only checked disjoint shape text/run or alt-text operations while preserving those source defects and their dependency payloads. Unknown/ambiguous defects refuse. Recognized creationId metadata has a separate bounded grammar; its exact bytes and namespace meaning survive local edits. Duplicate/identity-changing metadata edits remain unsupported. Unknown extensions are never stripped to obtain a pass.
