# Native process resources

`windows-job.cs` is a locally maintained derivative of the repository's reviewed Windows Job Object helper. Its source identity and origin are recorded in `provenance.json` and checked by the active runtime before compilation with the stock x64 .NET Framework compiler. A copied distribution carries this resource directly; no sibling skill or source package lookup occurs.

The historical C# class name is internal and does not select an application. Only LibreOffice Writer is a DOCX native target. The protocol3 derivative preserves primary failures separately from cleanup uncertainty and reports process_started only after successful ResumeThread; an empty job alone is not evidence of application start. Shared helper ancestry does not establish DOCX Windows acceptance: actual lifecycle, cancellation, Unicode paths, known-answer document and visual tests remain separate platform gates.

The minimum LibreOffice discovery floor is stable 26.2.5 or 26.8.0 and newer. This floor follows the official October 2026 security advisories at https://www.libreoffice.org/security/ and must be rechecked for each release acceptance. Version discovery never certifies safe input, execution or document fidelity.
