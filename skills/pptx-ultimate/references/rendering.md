# Render and visually verify

The pipeline is LibreOffice Impress → PDF → optional Poppler PNG. It never falls back to PowerPoint/Keynote, saves over the source, follows presentation links or claims animation/playback fidelity. Use `doctor` to identify tools and codec/schema self-checks. Executable discovery does not establish that the app can initialize or render this deck; only the requested real operation supplies that evidence.

```sh
node <skill>/dist/pptx-ultimate.mjs doctor
node <skill>/dist/pptx-ultimate.mjs render deck.pptx --out-dir preview --format both --include-hidden
```

Verified PDF requires LibreOffice plus `pdfinfo`. PNG additionally needs `pdftoppm` and the shipped image decoder. Explicit executable overrides are `PPTX_ULTIMATE_SOFFICE`, `PPTX_ULTIMATE_PDFINFO` and `PPTX_ULTIMATE_PDFTOPPM`. An explicit override is exclusive: an invalid path does not silently fall back.

On macOS, the native adapter supports Intel and Apple Silicon Node processes. It checks PATH first, then `/Applications/LibreOffice.app/Contents/MacOS/soffice` and the caller's `~/Applications/LibreOffice.app/Contents/MacOS/soffice`. Poppler also checks `/opt/homebrew/bin` and `/usr/local/bin`; these paths do not imply the tools are installed. Use a LibreOffice build compatible with the actual Mac. The pipeline calls the app-bundle executable directly with `--headless` and a private profile, leaving backend selection to macOS. If the OS blocks launching it, report the failure rather than bypassing Gatekeeper, quarantine or system policy.

On Windows, select the console launcher `soffice.com`. The Windows 10+ x64 adapter also requires the stock .NET Framework compiler (`csc.exe`); it compiles the shipped, integrity-checked process-ownership helper in a private temporary directory. A missing compiler or denied host capability is an explicit failure, not a reason to disable a security policy. Core package operations remain usable.

Use a maintained patched LibreOffice release. The runtime's minimum-version check is not a permanent advisory guarantee; consult [current official advisories](https://www.libreoffice.org/security/) when establishing a new accepted installation. Consult [scope](scope.md) for the implemented platform profiles and per-host verification requirements.

Output is a dedicated managed directory containing `render.json`, optional `presentation.pdf`, and `page-0001.png` onward. Without `--include-hidden`, only visible slides appear. Use the manifest's explicit source-slide/page mapping, never guess page identity from page count or a filename. Identical slides may legitimately have identical pixels.

Default output is no-clobber. `--overwrite` accepts only an intact prior render set owned by this engine, not an arbitrary or empty user folder. Handled failures retain the previous set; a shorter successful rerender leaves no stale pages. This does not promise crash-atomic durability or protection against a hostile concurrent writer. Keep source files outside the render destination.

Before native launch, supported schema/semantic/safety checks must pass. Active content, unchecked extensions or unsupported embedded data refuse handoff. The engine uses a disposable copy and private profile/cache; it does not weaken the user's macro policy or attach to an existing office session. Timeout/cancellation targets only its owned work. Linux/macOS commands own detached POSIX process groups; descendants that create another session and abrupt caller termination are outside that group-cleanup guarantee. Windows uses the helper's owned Job Object. If cleanup cannot be confirmed, preserve the reported recovery directory and explain the failure rather than claiming success. Abrupt termination of the calling process can also leave private temporary artifacts. Confirmed process-tree termination does not itself mean filesystem cleanup completed. Clean only exact task-owned paths after confirming their processes have stopped; never wildcard-remove another application's temporary/session data.

## Actual visual review

The pipeline checks PDF facts and complete PNG decoding, not design quality. Open the produced images and review every relevant page. For a new deck or shared style change, include all hidden slides. Look for:

- missing or clipped native text, shapes, tables, images or chart series;
- line wrapping, glyph substitution, bullet/numbering and inherited style changes;
- image crop/transparency, table literal values/readability and chart labels/legends;
- stale, repeated-first or incorrectly mapped pages.

Report structural/schema validation, successful native rendering and actual visual inspection separately. Font substitutions, unchecked content or an unavailable app remain explicit limitations. A screenshot cannot certify UI editability or other application behavior.

## Diagnose the current environment

`doctor` reports prerequisites, resource self-checks and executable identities. A successful diagnostic is not a document-render pass. If setup or a failed native operation needs a known-answer check, run:

```sh
node <skill>/dist/pptx-ultimate.mjs native-smoke --out-dir <fresh-evidence-directory>
```

Omit `--out-dir` to retain a new temporary evidence directory. Existing directories refuse; there is no provider selector, overwrite or keep flag. The command creates a native sample, renders PDF and PNG, and checks page geometry, selected solid-color pixels and unchanged source bytes. Its JSON uses `schema_version`, `operation`, `status`, `checks`, `artifacts`, `evidence_directory`, `report` and `limitations`. Missing prerequisites refuse; execution/sample failures fail. Completed artifacts and a failure report remain available for diagnosis.

A passing sample establishes only those checks on that environment. It does not certify fonts, arbitrary content, every feature or the user's final layout. Open the actual requested presentation's rendered pages before delivery.

## Contact sheets for selection

Use `contact-sheet <render-directory> --out-dir <fresh-directory>` after a PNG/both render. Optional --columns 1–8 and --thumbnail-width 160–480 control layout. The command verifies the complete render-set inventory/hashes/mapping and produces paginated PNG sheets plus contact-sheet.json with tile→slide identities and hidden labels. It retains original full-size pages and never launches a native app.

A contact sheet is a reduced navigation aid. Manifest consistency is not signed provenance or new native acceptance. Review full pages for typography and content. Input limits are1000pages,8million pixels/page and256MiB aggregate; output sheets use at most32tiles/8million pixels each. Existing output directories refuse.

## Warning-only layout estimation

`lint-layout <input.pptx> --fonts <font-map.json>` reads a JSON array of {family,path} font mappings. CLI paths resolve relative to that mapping file; API font paths must be absolute. Presentation/fonts must be direct canonical regular-file paths. The command rechecks captured bytes and never changes the deck.

The measured profile is deliberately narrow: top-level rectangular text, one paragraph/regular run, explicit regular font/size/insets/left alignment, ASCII LTR, no-wrap and no autofit. Existing square-wrapped authoring defaults, mixed runs, inheritance, rotation, placeholders and complex scripts are unchecked. Supply a matching regular static TTF; there is no system-font search or silent fallback.

Direct glyph advances and visible ink bounds are reported separately. Whitespace advance is not painted ink; glyph overhang can exceed its advance. The method omits kerning/shaping and native baseline placement. Warnings mean potential overflow; absence of warnings never establishes a fit. Native PDF/image review remains the final layout evidence. Do not shrink text or enable autofit just to clear a warning.
