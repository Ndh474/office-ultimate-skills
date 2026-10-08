# Advanced native content

Choose these profiles for the user's actual data or diagram structure. They remain native objects with source-bound edits; they do not imply all PPTX features or all Impress behavior. Read the current schemas and validate/render the exact output.

## Categorical composition charts

Keep string `categories` and complete series `{name,values,color?}` arrays.

- `column`/`bar`: `grouping` is `clustered` (default), `stacked` or `percentStacked`.
- `area`: `grouping` is `stacked` (default) or `percentStacked`; overlapping standard area is not this profile.
- `doughnut`: one nonnegative series with a positive total; `holeSize` may be omitted (default 50) or explicitly 50. Other values refuse. The native profile supports a fixed 50% hole; legal imported non-50 doughnuts remain unchecked and dependent edits/rendering refuse rather than silently changing them. Zeros stay in the data/legend when no slice is visible. New doughnuts show category names in the legend. Data edits retain the observed color-variation property, including older supported series-name legends.

New stack/area profiles require nonnegative data; percentage categories each need a positive total. Keep the raw values, rather than converting them to percentages before passing them to the engine. For example, values `[2,8]` and `[6,4]` produce category totals 8 and 12; a percentage plot shows their shares of those totals.

```json
{"id":"share","kind":"chart","chartType":"column","grouping":"percentStacked","box":{"x":40,"y":60,"width":640,"height":300},"categories":["A","B"],"series":[{"name":"Blue","values":[2,8]},{"name":"Orange","values":[6,4]}]}
```

Use a stacked chart for cumulative magnitude and a percentage chart for relative composition. They answer different questions. Data edits retain family, grouping, hole, title, legend and recognized colors while updating caches and literal workbook cells together. Grouping/hole fields on an inapplicable family refuse.

## Numeric XY scatter

Use `chartType: "scatter"` for actual numeric X spacing. There are no categories. Each series has its own nonempty `points: [{x,y}]`; series lengths may differ. The engine preserves order, repeated X and negative coordinates. Markers default to7-point circles; the supported explicit symbols/sizes are described below. Connecting/smoothed scatter lines are not supported.

```json
{"id":"xy","kind":"chart","chartType":"scatter","box":{"x":40,"y":60,"width":640,"height":300},"series":[{"name":"Measurements","points":[{"x":0,"y":2},{"x":1,"y":-1},{"x":10,"y":3}]}]}
```

The 1→10 horizontal distance must be nine times 0→1 before axis/display rounding. Do not pass numeric labels to a categorical line chart and call it scatter. The native model uses two numeric axes and independent X/Y ranges. `chart.data` for scatter contains only its replacement `series` with `points`; the categorical categories/values form is a different contract and refuses on a scatter source.

New chart profiles accept finite zero or magnitude 1e-12–1e12, subject to the 10,000-point/100-series and global request/workbook bounds. This is a practical supported range, not the full numeric capacity of PPTX. A single/repeated coordinate uses native auto scaling; visual review still needs to establish useful axis labels. Bubble and the bounded column+line combination are described below. Arbitrary plot mixes, marker shapes and axis modes remain unsupported.

## Groups and their coordinates

Use a `group` when child objects form a reusable diagram/card. Required `box` is in the parent's coordinates; required `coordinateSpace` defines the group's child coordinates, in points. `objects` contains native text, ordinary shapes, pictures, bounded straight/elbow connectors or nested groups. It cannot contain tables, charts or placeholders in this profile. IDs remain unique across the request; names may repeat.

For one axis, a child's position maps through: `group position + (child position − child-space origin) × group size / child-space size`. The engine composes this through nested groups using serialized EMU geometry. Do not pre-flatten child positions into slide coordinates or duplicate inherited scaling in the child boxes. Group movement/resizing changes only its outer box; its coordinateSpace stays unchanged.

```json
{"id":"card","kind":"group","box":{"x":80,"y":60,"width":400,"height":200},"coordinateSpace":{"x":0,"y":0,"width":200,"height":100},"objects":[{"id":"label","kind":"text","box":{"x":10,"y":10,"width":180,"height":40},"paragraphs":[{"runs":[{"text":"Native grouped content"}]}]}]}
```

Group transforms are positive and axis-aligned. Max depth is 8, with 10,000 native nodes per part, cumulative per-axis scale 1/64–64 and full composed corners within ±4032 points. Zero-after-rounding child extents, rotation/flips, unsupported grouped families and unchecked imported transformations refuse dependent changes. Inspect parent IDs/depth and local/world box evidence; do not select by a repeated display name.

## Attached connectors

A `connector` derives its geometry from two named shapes in the same slide. It may appear before them in the object array. Each endpoint is `{object: "<semantic-id>", site: "top|left|bottom|right"}`. New connectors belong to the endpoints’ lowest common container; the simple example below uses sibling shapes in one container. Supported endpoint shapes are rectangle, roundedRectangle and ellipse.

```json
{"id":"edge","kind":"connector","from":{"object":"source","site":"right"},"to":{"object":"destination","site":"left"},"style":{"lineColor":"334455","lineWidth":1.5}}
```

For this sibling example, put the record in the same objects array as its endpoints. It is a native attachment, not a free line that happens to overlap their boxes. The engine uses preset-specific connection sites, including ellipse's different site indices, and explicit orientation for reverse diagonals. It does not route around obstacles.

To move an endpoint, edit that shape's `object.geometry` with its observed hash. Affected connector geometry is recomputed atomically. Moving/resizing a whole group carries its internal attachments. Direct geometry edits on an attached connector refuse because the endpoints own its geometry. Self connections, unsupported endpoint presets, zero-length paths and retargeting remain outside this contract. Cross-group routing and arrow fields follow the bounded rules below. An imported richer connector may be preserved untouched while its dependent edit/render remains unchecked/refused.

## Verify the intended result

Inspect and validate before native handoff. Parsed chart observations are not proof of cache/workbook agreement; group boxes are not proof of text fit. Render before and after edits and open the produced images. Check raw-value versus percentage meaning, numeric X spacing, hole/legend, nested placement, cropped images, text fit and actual endpoint contact. Keep the source and unrelated content unchanged. If a requested unsupported feature matters, report it instead of flattening or silently changing the user's design.

## Text fit and hyperlink appearance

Text edits preserve explicit sizing; they do not silently resize a shape or fit text. A longer or bold replacement can wrap or overflow. Choose a deliberate fontSize or geometry change and render the result. Linked-run RGB overrides refuse in the maintained profile because Impress uses its hyperlink theme color; linked text and other supported styles can still be edited while retaining the link.

## Chart labels, axes and stable series

`presentation.dataLabels` is false or a bounded selection of value/category/series labels; percent applies only to pie/doughnut. `presentation.axes` uses category/value for categorical charts and x/y for scatter/bubble. Axis titles, major/minor gridline RGB/width and number formats General, 0, 0.00, #,##0, 0%, 0.0% and 0.0E+00 are supported. Use 0.0E+00 for compact scientific labels at very small or large magnitudes. Formatting never changes numeric values. Use the schema for family-specific fields; invalid label positions refuse. Line/scatter/radar markers are none/circle/square/diamond, size2–20 points; a none marker has no size. Review label crowding and actual markers. For combo charts whose column and line labels meet, choose separate positions per plot, for example center on columns and above on lines. The renderer does not automatically avoid label collisions.

`chart.style` patches title, legend, presentation and selected series by nativeId. Patch title:null removes a title; omitted values preserve it. Explicit false disables labels/gridlines/legend where defined. Inspect exposes chart.series_ids without readable content; chart.data with advanced presentation requires each observed nativeId. Unknown IDs and ambiguous ordinal updates refuse. Reinspect the output for each new source-bound edit; duplicate series names are not identities.

Radar uses at least3 shared categories and nonnegative bounded values; radarStyle is standard or marker. Filled/negative radar refuses. Bubble uses independent points {x,y,size}, positive size and fixed area scaling; sizes1/4/9 describe areas, not diameters. Bubble data labels must be omitted or false. Enabled label objects refuse with CHART_BUBBLE_LABEL_UNSUPPORTED because the selected native profile does not reliably preserve their numeric meaning. Marker overrides do not apply to bubble. Both families keep cache, exact ranges and literal workbook cells synchronized.

## Merged and styled tables

`merges:[{row,column,rowSpan,columnSpan}]` uses disjoint zero-based rectangles with one origin and area greater than1. Every covered source cell must be exactly an empty string; whitespace is content and refuses merging. RowStyles and columnStyles are sparse index/style records; cellStyles address origins only. Style precedence is table→row→column→cell, evaluated at the merge origin. The origin's resolved style applies across its merged rectangle.

Table-specific borders have top/right/bottom/left values false or {color,width}. False hides that edge; per-edge width0 is a native hairline. Existing uniform lineWidth0 retains its no-fill meaning. One requested shared edge is mirrored; conflicting explicit edges, including along a whole merged side, refuse. Unchanged imported border asymmetry is preserved; ambiguous partial style changes refuse. Per-cell horizontal/vertical alignment remains native and independently checked.

## Column+line and secondary axes

`chartType: "combo"` uses common categories and exactly two named plot families: plots:[{type:"column",axisGroup:"primary"},{type:"line",axisGroup:"primary"|"secondary"}]. Each series names its plot and keeps a chart-local nativeId. Both plots must remain nonempty. A displayed legend requires all column series before all line series; reorder within a plot is allowed. Interleaved data order requires explicit legend:false; enabling its legend refuses rather than silently rearranging series. The shared-primary profile uses one unique category/value axis pair. The secondary profile uses a second reciprocal pair with a hidden categorical axis and right value axis. Inspect reports actual axis_groups and plot_keys; names are not IDs.

Plot-specific labels belong to each plot's presentation.dataLabels. Top-level combo presentation accepts axis styles only, including secondaryCategory/secondaryValue when present. `chart.style` can patch labels through plots:[{type,presentation}], but cannot silently move an existing series or plot onto another axis group.

To add a series in chart.data, use newId plus its name/data and, for combo, plot. Existing series use observed nativeId; unknown nativeId never allocates. New series may supply color/marker. The receipt maps new_id to native_id in applied.created_series; reinspect before the next external edit. Request newId is not a persisted name or a later-operation alias. Removed identities are not reused within the same batch.

## Presentation sections

A deck may declare sections:[{id,name,slides:[semanticSlideIds]}]. Each section is a contiguous nonempty group; unsectioned slides are allowed. Slide membership and section GUIDs must be unique. Inspect presentation.sections for source-bound addresses, current hashes and native slide IDs; names require includeText.

section.create targets the presentation; section.rename/remove/membership target an observed section. All require the current target hash. Removing a section leaves its slides. Duplicate inherits source membership; removing the last member removes its empty section. Reordering preserves membership and refuses if it would split a section. slide.add defaults to unsectioned; an explicit observed section must remain contiguous at the appended position. Unknown section extensions or legal noncontiguous imported profiles remain preserved/unchecked and refuse dependent section/topology edits. A PDF cannot establish editable section membership; native save/reopen or model evidence is required for that claim.

## Theme references and shared design resources

`presentation.theme.definition` accepts a reloadable versioned definition of the 12 color roles and major/minor Latin, East Asian and complex-script fonts. Inspect with `includeText` to read it. A definition replaces the legacy palette/font input; do not combine both forms. The engine does not fetch themes or fonts.

Color fields accept literal RGB or `{scheme:"accent1"}` and other declared roles. Font fields accept a literal family or `{theme:"major"}` / `{theme:"minor"}`. `presentation.theme.inheritDefaults:true` deliberately writes theme-linked default colors/fonts. Ordinary text still has explicit size and layout: this option does not make every property inherit from the master. Literal overrides stay literal. `slide.add` can likewise request `inheritDefaults:true` for its new objects.

Inspect exposes source-bound themes, masters, layouts and their native objects. `theme.apply` takes an observed theme address, current SHA and a full `definition`. It changes that resource only; native references pick up the change and literal formatting stays fixed. Inspect the returned impact list, including hidden slides, and render every affected slide. Theme overrides or unresolved required resource semantics refuse rather than being flattened.

Master/layout objects use their observed addresses with supported object/text, picture, table or chart operations. Shared resource edits always require the current object SHA. Their impact lists follow actual inheritance relationships; a shared theme and a shared master are different scopes. `master.defaults` selects one `role` (`title`, `body`, `other`), zero-based `level` 0–8 and supported text `style`. Direct formatting on layouts/slides/runs remains intact and may override that default. No generic master XML replacement, implicit rebinding or global RGB replacement is provided.

## Paint and connector routing

Shapes/text boxes support a solid color, `fill:false`, or a bounded linear fill `{type:"linear",angle,stops:[{offset,color,opacity?},...]}`. Use 2–16 strictly ordered stops from exactly 0 to 1 and an angle in [0,360). `fillOpacity` multiplies explicit gradient stops once; solid `fillOpacity` and `lineOpacity` are absolute 0–1 values. A color-only edit preserves supported existing alpha. Opacity-only changes to an existing gradient refuse; supply its complete intended stops.

`shadow` is one outer shadow with color, opacity, blur/distance in points and angle in degrees. Defaults: opacity 0.25, blur 4 points, distance 2 points, angle 45°; blur/distance are 0–100 points and angle is [0,360). `shadow:false` removes that owned effect. Unsupported effect DAGs and unresolved inherited effects refuse. Unrequested direct native effect siblings remain preserved; this is not a general effect editor.

Attached connectors support `routing:{type:"straight"}` or bounded `elbow` routing. Elbows use a monotone two/three-segment orthogonal path derived from cardinal sites; the three-leg bend is the midpoint. Omit `bend` or set exactly 0.5. Other authored fractions refuse; imported non-midpoint routes remain unchecked and are never clamped or repaired. A detour, touching/zero leg, unsupported transform or impossible endpoint tangent refuses. This is not obstacle avoidance. Internal quarter-turn transforms belong to the connector representation; rotated/flipped endpoint shapes/groups remain unsupported.

`style.headEnd` names the start/from arrow and `tailEnd` the end/to arrow. Types are none/triangle/stealth/diamond/oval/arrow; width/length are sm/med/lg. `object.style` can patch these ends while preserving other supported stroke properties. Selected imported non-none arrow markup requires explicit native width/length values; richer or unresolved arrow defaults remain unchecked. Cross-group endpoints require positive bounded axis-aligned transforms within one slide. Put a newly authored connector in the endpoints' lowest common container. Moving one endpoint ancestor recomputes crossing edges; moving their common ancestor carries internal geometry once. The engine preserves native IDs/sites and reports serialized geometry; native contact/save-reopen still needs actual output review.

## SVG and opaque EMF pictures

Captured local SVG can author a native SVG image plus a generated PNG fallback. Both are retained and deduplicated by bytes. The closed SVG grammar covers static shapes, paths, transforms, literal fills/strokes and local linear gradients. It refuses text/fonts, CSS/style, images/use, scripts/events, animation, external/data resources, filters and unknown elements. Do not assume a general SVG file fits it.

The bundled renderer has no network/system-font inputs, a pinned 256 MiB WASM maximum and a 15-second worker deadline. Individual SVG: 1 MiB, dimensions up to 4096, 4 million pixels; validation additionally limits 64 SVG payloads and 32 million aggregate SVG pixels. Geometry/node/path/transform limits may refuse earlier. Generated fallbacks count toward request asset-byte limits. Verify actual native crop/scale/alpha appearance; successful fallback generation alone does not prove which representation a presentation application displayed.

A paired imported SVG must pass the same grammar and exact deterministic fallback check for native admission. A different or stale fallback remains unchecked. Safe dormant SVG payloads can remain checked without inventing a picture binding. Unknown SVG can be preserved unchanged by a disjoint package edit where its operation closure permits; it is never handed to a renderer merely because relationships contain no external target. Embedded SVG content has its own resource surface.

EMF remains opaque. Inspection reports bounded header/record-envelope facts and an explicit `native_supported:false`; those facts are not a decoder or safety certificate. Disjoint package edits preserve bytes where their dependency closure allows. EMF+, escapes/comments/embedded payloads, GDI object/state/path semantics and native rendering are not supported by the current EMF profile. Do not convert, strip, or native-render an opaque image to obtain a pass.
