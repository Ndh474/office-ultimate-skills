---
name: pptx-ultimate
description: Create, inspect, edit, validate and render PPTX presentation artifacts using native OOXML content and LibreOffice Impress. Use when a .pptx is supplied or requested, including template adaptation, source-preserving edits and visual verification. Do not trigger for presentation prose or visual ideation alone, or cloud-slide work without a PPTX artifact.
---

# PPTX Ultimate

Represent the user's content and design faithfully as a PPTX. Use the bundled engine for native objects and source-preserving edits; use LibreOffice Impress for static rendering. Do not substitute a screenshot for a requested native table, chart, diagram group or text object.

## Choose the workflow

- New deck: read [creation](references/creation.md), obtain the current `schema deck`, create from an explicit JSON spec, then validate and visually review all slides.
- Read-only inventory or validation: use inspect/validate; request readable content only when needed. Do not start an edit or create an output for an inspect-only task.
- Edit an existing deck: inspect before choosing targets. Read [editing](references/editing.md), bind the batch to the actual source hash and expectations, and write a separate output. Preserve content/design the user did not ask to change.
- Verification or previews: read [rendering](references/rendering.md). Package/schema checks, native PDF/PNG generation and actual visual review are separate evidence.
- For advanced charts, merged tables, themes/masters, SVG or native grouped diagrams, read [advanced content](references/advanced.md) before choosing fields or editing geometry.
- Check [supported scope](references/scope.md) for specialized objects or import limitations. Use [API guidance](references/api.md) for task-script integration.

The entry is `dist/pptx-ultimate.mjs`. Run it with Node 22.13 or newer. The copied skill folder is complete; do not install consumer npm dependencies or import maintainer source helpers. `--help`, `--version`, `doctor`, `schema deck` and `schema edit` expose the current contract. Shell-quote local paths.

For native setup or troubleshooting, use `doctor` for prerequisites, then `native-smoke` when a real sample execution is needed. The smoke keeps a small synthetic PPTX/PDF/PNG evidence set in a fresh directory; `--out-dir` selects its location. It does not inspect or modify a user presentation and does not replace rendering and reviewing the requested output. Skip native diagnostics for a read-only package task unless its question actually concerns native execution.

## Preserve intent and evidence

Use source-bound observed identities, never a display name or an assumed slide index. Keep literal strings literal, especially leading zeros and leading equals in cells. Use structured text when paragraph/run formatting is intentional.

Honor refusal and unchecked coverage. Do not silently repair, flatten, strip extensions, execute embedded code, follow presentation links or switch applications to make a check pass. The engine accepts captured local assets only; separately acquiring new imagery must stay within the user’s task and permissions. Report the specific missing capability when it prevents the requested result.

Render with hidden slides included for new/global changes. For a local edit, inspect the affected rendered slides, including a hidden target, and verify the rest of the package stayed within the requested change. Open the actual produced images; successful process exit or image existence does not prove good layout.

Deliver the requested PPTX and any requested previews with a short account of what was verified and any remaining limitation. Never equate static rendering with animation/playback, full native UI editability or universal app compatibility.
