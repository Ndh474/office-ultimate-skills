# Native charts

Use the [builder API](authoring-api.md) to create or edit charts. Inspect the source cells and the chart's type, series and axes before editing an imported chart. A native chart remains connected to its cells; a successful PDF export alone does not establish that its geometry or data roles are correct.

## Create a chart

`addChart(sheet, { type, sourceRange, position?, title?, hasLegend?, yAxisNumberFormat?, comboLineSeries? })` uses a rectangular source with a header row. `position` has `from` and `to` cell addresses. Unsupported options refuse; camera, shape and bubble shading controls are not exposed.

| Types | Source columns, from left to right |
|---|---|
| `bar`, `column`, `line`, `pie`, `area`, `doughnut`; `bar-stacked`, `column-stacked`, `area-stacked`; `bar-percent-stacked`, `column-percent-stacked`, `area-percent-stacked` | Categories, then one or more value series |
| `scatter` | Shared X values, then one or more Y series |
| `combo` | Categories, then at least two value series; columns plus secondary-axis lines |
| `radar`, `radar-marker`, `radar-filled` | Categories, then one or more value series |
| `bubble` | Exactly X, Y, Size; one series |
| `stock-hlc` | Exactly Category, High, Low, Close |
| `stock-ohlc` | Exactly Category, Open, High, Low, Close |
| `column3d`, `column3d-stacked`, `column3d-percent-stacked`, `bar3d`, `line3d`, `area3d`, `area3d-stacked`, `area3d-percent-stacked` | Categories, then one or more value series |
| `pie3d` | Exactly Category, Value; one series |

Radar, bubble, stock and the named 3D profiles require finite numeric literal cells in their numeric channels when created or re-ranged. Blanks, text, booleans and formulas in those channels refuse, including formulas with stored results. Bubble validates X and Y as well as Size; Size must be positive. Stock requires Low ≤ Open/Close ≤ High in each row. `pie3d` requires nonnegative values with a positive total. Category labels can be text or numbers. The existing 2D types retain their ordinary referenced-cell contract.

For `combo`, the second value series is a line by default. `comboLineSeries` selects one-based value-series indexes, such as `[2, 3]`; at least one series must remain on columns. This option does not apply to radar, bubble, stock or 3D profiles.

`column3d` uses depth to separate series. `bar3d` is horizontal clustered bars. The stacked and percent-stacked types use their named grouping; `line3d` draws ribbons. The profiles have fixed views and appropriate axis topology. Bubble size is not a spatial Z coordinate. Stock volume combinations, surface/contour creation, arbitrary 3D variants and bubble 3D appearance are unsupported.

Example: a bubble source `A1:C4` contains headers X, Y, Size followed by three numeric rows. Create it with `builder.addChart("Data", { type: "bubble", sourceRange: "A1:C4", title: "Measurements" })`. Do not place two Y series in those columns: the third column controls bubble area.

## Inspect and edit an existing chart

`listCharts(sheet)` and `inspect --kind chart` report chart IDs, recognized types, plot elements/grouping/axis IDs, series references and fidelity limits. Bubble series include `size_ref`; stock series include their ordered `role`. An imported type can be recognized even when creation or a particular edit is unsupported. Preserve unknown parts and extensions.

`updateChart(sheet, id, { title, legend, valueAxisNumberFormat })` edits those owned elements; the axis format targets the first value axis. Type conversion is refused. For the ordinary 2D types, series edits retain the following forms:

- `re-range`: `{ action: "re-range", index, categoryRange?, valueRange?, nameCell?, sourceSheet? }`
- `remove`: `{ action: "remove", index }`, keeping at least one series
- `add`: `{ action: "add", index, categoryRange, valueRange, nameCell, sourceSheet? }`, with an unused index

Use each series' reported index. `plotIndex` selects a plot in a supported combination chart. A category/value range is one column; paired data channels must have equal row counts.

For radar, bubble, stock and the named 3D profiles, pass a complete replacement binding for every existing series in one single-plot chart. Each operation must be `re-range` and supply both `categoryRange` and `valueRange`; all series share the same category range and source sheet. Bubble also requires `sizeRange`. Stock's existing High/Low/Close or Open/High/Low/Close roles stay in order. These profiles refuse add/remove, incomplete bindings and edits inside a combination chart. Their replacement numeric data obeys the creation rules above. `nameCell`, if supplied, is one cell.

Example: replace a bubble's channels together with `builder.updateChart("Data", id, { series: [{ action: "re-range", index: 0, categoryRange: "E2:E4", valueRange: "F2:F4", sizeRange: "G2:G4" }] })`. Supplying only X and Y refuses because the size channel would remain coupled to different data.

Expected input refusals leave the builder usable. Other chart XML survives a supported edit; edited reference caches are removed. Ordinary source-cell edits still follow worksheet ownership and dependency rules, so inspect the chart again if later changes alter its input meaning. See [chart-cache evidence](data-and-formulas.md#chart-cache-evidence) before relying on stored chart data.

## Verify the visual result

Render the affected sheet with `preview`, then inspect the requested chart: category order, labels, roles, relative sizes, axis scales, stacking and depth. Check the numeric cells independently. Recalculation and a structurally valid PDF are different evidence from chart fidelity.

Imported surface/contour parts can be preserved, but Calc may render them as columns. Bubble 3D appearance can be flattened. Read `fidelity_limits` from chart inspection and do not treat those approximations as faithful previews. Native save-through can also normalize styles or flags; use targeted package edits to preserve unrelated chart content. Full spill coverage or fresh serialized chart caches must not be inferred from an apparently plausible image.
