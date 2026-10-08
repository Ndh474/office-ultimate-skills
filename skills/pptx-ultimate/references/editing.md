# Edit an existing presentation

Start with `inspect input.pptx`; add `--include-text` only when content is needed. Default reports expose safe numeric/opaque identities, not deck text or raw XML. Canvas dimensions are reported in `presentation.canvas` in both EMU and points; check its observation status. Use `schema edit` for the closed request contract.

```sh
node <skill>/dist/pptx-ultimate.mjs inspect input.pptx --include-text
node <skill>/dist/pptx-ultimate.mjs edit input.pptx changes.json output.pptx
node <skill>/dist/pptx-ultimate.mjs validate output.pptx
```

Keep the source unchanged and write a separate output. Do not reconstruct the whole presentation merely to change a supported existing object. If the request changes only local content, preserve the source's unrequested design and inherited styles. For template adaptation, identify obsolete example content explicitly; a copied layout does not justify retaining obsolete names, figures or period labels.

## Bind identities and expectations

A changes file contains `schema_version: 1`, the actual `source_sha256`, and ordered `operations`. Use addresses emitted by inspect; they include the source hash and native IDs. A display name or slide number is not a selector. Layout observations supply source-bound addresses and supported role/index slots.

Every operation includes `type`, `target`, a nonempty `expect`, and `data`. Expectations are exact and apply to the current state after earlier operations. Available facts depend on the operation: object XML hash, current text, counts, visibility/link value, image byte hash, or chart data identity. Do not supply facts that the selected operation cannot verify.

For `chart.data` and `chart.style`, use the inspected `chart.data_sha256` as `expect.value`; a frame hash does not bind its chart/workbook contents. For `table.cells`, bind the object hash or supply an exact `expect` string on every selected cell. Coordinates are zero-based; values remain strings.

New objects/slides can use `handle:<declared-id>` later in the same batch. Duplication additionally exposes `handle:<new-slide-id>/<original-shape-id>`. Deleted aliases cannot be reused. Final `identities` and `created_handles` bind the published output hash, usable for a subsequent request. Applied-operation records describe the original batch; re-inspect when in doubt.

## Supported changes

- Literal text replacement preserves unambiguous uniform formatting. Mixed rich formatting refuses rather than guessing; explicit structured paragraphs let the caller intentionally replace that content. Placeholder fill keeps shared layout/master design unchanged.
- For a local change inside mixed formatting, use `text.runs` rather than replacing all paragraphs. Inspect with `--include-text`, choose the reported zero-based paragraph/regular-run coordinates in `text_body`, and require the current object `expect.sha256`. Break tokens are not runs. Each `data.runs` item supplies `paragraph`, `run`, and `text` and/or supported character `style` fields. Unselected runs, paragraph defaults, breaks and existing run links stay intact. Coordinates must be unique in one operation; dynamic fields and raw CR/LF in a literal run refuse. Direct properties in inspection do not resolve inheritance. RGB color deliberately replaces the selected run fill; do not use that override when a gradient or opacity transform must stay unchanged. Linked runs use the application hyperlink color in the maintained Impress profile: explicit RGB overrides refuse with `EDIT_LINK_COLOR_UNSUPPORTED`, including whole-body formatting that would reach a linked run. Text, font and other supported character edits may preserve the link. Opt-in run observations identify this restriction. Do not remove a link or recolor the shared theme merely to bypass it. If observation is unchecked (including its 10,000-node reporting bound), do not invent a run selector.
- Text formatting, ordinary object geometry/style, alt text and inert links have selected native-object contracts. An inherited property is not flattened unless the requested local override requires it.
- Picture replacement isolates one consumer, preserving a shared original image for other objects. CLI replacement assets are under the changes-file directory; API paths must be absolute. A changed aspect ratio requires explicit fit. Unsupported tiled/compound fill profiles refuse.
- Literal table-cell batches preserve untouched cells and supported formatting. Chart-data replacement synchronizes caches/ranges and its owned literal workbook together. Categorical sources use categories/values; scatter/bubble use numeric points. Advanced series require observed `nativeId` values, including reorder; do not infer identity from names or list positions. `chart.style` patches presentation while preserving data. See [advanced content](advanced.md). Inspection exposes recognized data only with `--include-text` and marks it as requiring validation, not certified workbook agreement. Shared or unsupported chart data cannot be edited by approximation.
- Group/connector geometry has dependency rules in [advanced content](advanced.md). Move an attached endpoint or group, not the connector line itself. Do not flatten a group to get around an unsupported imported transform.
- Notes, slide add/duplicate/remove/reorder/visibility are controlled graph changes. Add uses an observed source layout; omitting it requires exactly one blank layout. Duplication preserves design/media references and clones checked owned notes/chart data. Incoming slide links and unsupported topology may require refusal.

For example, after observing the exact object and its second regular run:

```json
{"type":"text.runs","target":"<observed address>","expect":{"sha256":"<current object hash>"},"data":{"runs":[{"paragraph":0,"run":1,"text":"Updated value","style":{"bold":true}}]}}
```

This changes one run. It is not a request to restyle the entire text body. A later operation on the same object must use expectations for its updated state; use a new inspect/edit round when that hash is not otherwise available.

Any failed operation prevents publication of the batch, including a later failure following earlier successful mutations. A no-op keeps exact source ZIP bytes; ordinary edits preserve unowned part payloads, not historical compression bytes. Untouched unsupported content may survive, but editing it or rendering it can be refused. Do not remove an extension, review object, timing or signature to bypass that boundary.

After editing, validate, inspect the intended change and verify declared changed parts. Render the relevant slides including a hidden target; review all pages when a shared/global property changed. See [rendering](rendering.md).

## Known metadata and compatibility editing

Use strict validation first. The optional changes field `compatibilityPolicy: "preserve-known"` admits only named proven source deviations for disjoint `text.runs`, `text.replace` or `object.altText` operations. Default is `strict`. The result keeps original strict-validity facts and preserved deviation identities; a successful local edit is not whole-deck validation or native safety. Inspect may provide safe source-bound local observations while returning refused for a recognized graph deviation. Follow its required policy; do not invent targets for ambiguous/unsafe parts. Native rendering still uses its separate strict gate.

A recognized nonvisual creationId may remain during supported local edits. Its exact extension bytes and inherited namespace context stay protected. Unsupported metadata errors identify a safe structural location and next step; never delete the extension as a workaround. Identity-changing duplication remains refused.

For merged tables, inspect `table.structure` for origin/continuation coordinates. `table.cells` updates origins or ordinary cells; continuation edits refuse with the origin coordinate. `table.style` accepts table/row/column/cell style patches and requires the current object SHA256. Empty/whitespace covered content is not silently combined, and contradictory requested shared edges refuse before publication.
