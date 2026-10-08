# Office Ultimate Skills

Agent skills for creating, inspecting, editing, validating, and rendering Word documents, Excel spreadsheets, and PowerPoint presentations.

Each skill includes instructions, a bundled runtime, and reference documentation. Use them with an AI agent that supports [Agent Skills](https://agentskills.io) and can run local commands.

## Skills

| Skill | What you can do |
| --- | --- |
| [docx-ultimate](skills/docx-ultimate/SKILL.md) | Create and edit DOCX documents, work with tables and templates, search content, and render pages for review. |
| [xlsx-ultimate](skills/xlsx-ultimate/SKILL.md) | Create and edit XLSX workbooks, work with formulas, formatting, tables, and charts, and export data or previews. |
| [pptx-ultimate](skills/pptx-ultimate/SKILL.md) | Create and edit PPTX presentations, adapt existing decks, work with native slide objects, and render slides for review. |

## Install

Use the [skills CLI](https://github.com/vercel-labs/skills) to choose the skills and agent you want to install for:

```bash
npx skills add Ndh474/office-ultimate-skills
```

Or install an individual skill:

```bash
npx skills add Ndh474/office-ultimate-skills --skill docx-ultimate
npx skills add Ndh474/office-ultimate-skills --skill xlsx-ultimate
npx skills add Ndh474/office-ultimate-skills --skill pptx-ultimate
```

Installation defaults to the current project. Add `-g` to install for your user account across projects. Use `--list` to see available skills without installing.

## Requirements

- **Node.js 22.13 or newer** to run the bundled runtimes. No separate `npm install` is needed inside the skill folders.
- **LibreOffice** for document and slide rendering, spreadsheet formula evaluation, and ODS conversion.
- **Poppler** for PDF verification and PNG previews.
- **Fonts used by your files** for consistent rendering.

Reading, creating, and editing files do not require LibreOffice unless the requested operation also needs calculation, rendering, or conversion through it. Keep each installed skill folder intact so its runtime and supporting resources remain together.

Ask your agent to run the relevant skill's `doctor` command before calculation or rendering. It reports operation-specific prerequisites and detected tools; each skill's documentation covers supported versions and platform requirements.

## Use

Give your agent the source files or content, the result you want, and any formatting or preservation requirements. You can name the skill explicitly:

**Word documents**

> Use docx-ultimate to turn these notes into a DOCX report with headings and a summary table. Save a separate output file and render the pages for review.

**Excel spreadsheets**

> Use xlsx-ultimate to create a monthly budget workbook from this CSV, with category totals, formulas, and a summary chart. Recalculate the formulas and check the results.

**PowerPoint presentations**

> Use pptx-ultimate to update the figures in this presentation while preserving its theme and layout. Save a new copy and render the changed slides for review.

## Working with existing files

The skills are designed to preserve content and structures outside the requested change, save edits separately from the source, and report unsupported operations. Supported text, tables, charts, and other objects remain native to their file format.

Validation, calculation, and visual review check different things. A valid file still needs layout review when appearance matters. LibreOffice supplies rendering and spreadsheet calculation; results can differ from Microsoft Office, and static slide previews do not verify animations or playback.

See each skill's linked documentation for supported operations, limitations, and detailed usage.
