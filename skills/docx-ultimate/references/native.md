# Native rendering and evidence

LibreOffice Writer is the only native application. Eligible runtime profiles are Windows x64, Linux x64 and macOS x64/arm64. Eligibility and tool discovery are prerequisites, not platform acceptance.

```text
doctor
native-smoke --out-dir fresh-smoke-evidence
render report.docx --out-dir fresh-pages --format both --view current
```

`doctor` checks packaged resources and executable identities. Missing ordinary tools can appear in a successful diagnosis; actual execution, ownership or integrity faults fail. It does not open a document. Native work requires stable LibreOffice 26.2.5 or 26.8.0 and newer plus Poppler `pdfinfo`, `pdftotext`, `pdffonts` and, for page images, `pdftoppm`. Prerelease versions refuse. Windows process ownership also requires the stock x64 .NET Framework compiler. Do not install or alter system software without authorization.

`DOCX_ULTIMATE_SOFFICE`, `DOCX_ULTIMATE_PDFINFO`, `DOCX_ULTIMATE_PDFTOTEXT`, `DOCX_ULTIMATE_PDFFONTS` and `DOCX_ULTIMATE_PDFTOPPM` are exclusive executable-location overrides. They do not select another application or bypass version/safety checks.

`native-smoke` retains a benign two-page sample, report and completed artifacts. Success requires real page/text/font and raster-contrast known answers. Inspect both page images. This sample does not certify every feature, document or platform. A refused or failed probe retains available evidence and its original safe error.

Rendering admits a bounded passive Transitional package profile. Signed/macro/embedded/linked-resource/unknown structures, unsafe fields and unsupported review forms refuse before native handoff. Local image bytes and dimensions are checked. Private copies/profiles disable macros and external updates; they are lifecycle isolation, not an OS security sandbox. Native parser vulnerabilities are not ruled out.

Use `current` or `markup` explicitly. Markup requests visible tracked insertions/deletions and comments; original-view rendering refuses. Unproved move, property/paragraph-mark and table-revision profiles refuse. The PDF filter requests the view without accepting/rejecting revisions; inspect actual output because a filter override can fail silently.

Outputs are PDF, PNG pages, or both, in a fresh directory. Page geometry is measured individually, including mixed sections. Page PNGs are 96 DPI with a 25-million-pixel per-page and 500-million-pixel aggregate proof bound. Native jobs have a bounded complete deadline; output count/bytes, XML, image and text extraction are capped. Existing output directories refuse unchanged. Failure may retain owned recovery directories; never treat their existence as success.

Review all pages for clipping, missing glyphs, substitutions, table/header/footnote boundaries and downstream reflow. Recorded PDF fonts are observations, not proof of every glyph. Rendering never writes a natively rewritten DOCX back over the source. PDF fields can appear updated while stored DOCX caches remain stale.
