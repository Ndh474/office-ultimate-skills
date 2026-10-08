# Task-script API

Import the copied `dist/pptx-ultimate.mjs` using an absolute file URL. Imports are silent and do not launch apps or write files. Do not import maintainer helpers. The module exports `apiVersion`, `version`, and these public operations:

```js
import path from 'node:path';
import {pathToFileURL} from 'node:url';
// skillDirectory and input are absolute local paths supplied by the caller.
const api = await import(pathToFileURL(path.join(skillDirectory, 'dist/pptx-ultimate.mjs')).href);
const view = await api.inspect({input, includeText: true});
```

- `doctor()` performs local schema/codec self-checks and native tool discovery.
- `nativeSmoke({outDir?})` runs the retained known-answer diagnostic described below.
- `schema('deck'|'edit')` returns the closed versioned request schema.
- `inspect({input,includeText?})` returns inventory, safe identities and coverage, including `presentation.canvas` dimensions in EMU/points when observed safely. Opt-in `text_body` provides regular-run coordinates/direct properties; recognized chart data is opt-in and still requires validation. Group/connector observations expose safe hierarchy/geometry and explicit profile status.
- `validate({input})` adds applicable schema and feature consistency checks.
- `create({spec,output,assetRoot?,overwrite?})` accepts a captured deck object; pictures require explicit assetRoot and safe descendants.
- `edit({input,changes,output,overwrite?})` accepts a source-bound changes object; replacement/add assets in API requests must use absolute local paths.
- `lintLayout({input,fonts:[{family,path}]})` produces read-only warning estimates for the explicit no-wrap regular ASCII profile; font paths must be direct absolute local files. Unmeasured text stays unchecked and no-warning is not a fit certificate.
- `contactSheet({input,outDir,columns?,thumbnailWidth?})` derives bounded navigation sheets from a verified PNG render directory; output must be fresh.
- `render({input,outDir,format?,includeHidden?,timeoutSeconds?,overwrite?})` runs the explicit native pipeline. Formats are pdf/png/both, default pdf; hidden slides default false, timeout 120 s, overwrite false.

Responses identify schema_version, operation and status; checks, warnings, limitations and operation-specific fields appear where applicable. Statuses are succeeded, refused or failed. Inspect `error.code` for a safe typed reason; do not parse raw application diagnostics. API callers must check status: receiving a resolved Promise does not mean the operation succeeded. The CLI emits one JSON response; exit 0 is success,1 refusal,2 an operational failure or the REQUEST_INVALID/SCHEMA_UNKNOWN response. Some bounded-request/source refusals therefore use exit 1; inspect the typed error code rather than classifying by prose.

Requests must be plain bounded JSON-like data: no accessors, sparse arrays, cycles, unknown fields or arbitrary native property bags. Capture returned source hashes and observed addresses rather than reconstructing them. Requests and source identities are fixed before asynchronous work; mutating a caller object afterward does not change the accepted request.

Use output-bound `identities`/`created_handles` from edits or inspect the newly published file before the next edit. Keep source/output distinct and preserve any existing destination unless overwrite is explicitly intended. A failed batch must not be retried as if earlier operations had already been published.

For a full example, follow [creation](creation.md), [editing](editing.md) and [rendering](rendering.md). Do not equate a native tool path in doctor with a live render pass or a syntax-valid schema object with supported feature semantics.

## Environment diagnostics

`await doctor()` diagnoses resources and native prerequisites without opening a document. `await nativeSmoke({outDir})` explicitly creates and checks a synthetic Impress sample; omit the argument or outDir for retained temporary evidence. The directory must be fresh. Both use `schema_version:1`, `operation`, `status` (`succeeded`, `refused`, `failed`), keyed `checks` and `limitations`; smoke adds artifact descriptors `{kind,path,bytes,sha256}` and evidence/report paths. Neither result is a universal native-capability or visual-quality certificate.
