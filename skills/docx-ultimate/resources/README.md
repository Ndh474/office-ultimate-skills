# Resource provenance

Resources are adjacent portable inputs, never a reason to look for an ancestor dependency installation. `vendor-provenance.json` records their origin; all paths in it are relative to this resources directory.

`source_resources` names a resource root, its provenance record and source notes. The schema record identifies immutable source bytes and their upstream origin.

`npm_resources` records exact, unmodified files copied from locked runtime dependencies. Each record has exactly `path`, `package`, `package_file`, and `sha256`. For example, a validator asset can name `validator/xmllint.wasm`, package `xmllint-wasm` and upstream `xmllint.wasm`, with its actual SHA-256. List the JavaScript factory separately. Do not substitute a transformed file under an upstream hash.

Assembly checks each copied resource against both its declared hash and the same file in the installed locked package, checks the installed version, and captures that package's complete distributed license/notice files. This resource use is recorded separately from modules reached by the bundler. A different dependency or transitive asset needs its own explicit record; it does not inherit another package's provenance.
