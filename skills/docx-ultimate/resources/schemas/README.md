# WordprocessingML Transitional schemas

The skill validates WordprocessingML and document-property parts against these schemas, and derives from them the order in which child elements must appear. The WML set is the transitive `schemaLocation` closure of `wml.xsd`; the metadata set adds core, extended/custom properties and their Dublin Core/variant-type dependencies. The VML schema closure is included for declared VML grammar in WordprocessingML lax hosts. SpreadsheetML and PresentationML remain outside this set; VML Excel/PowerPoint extension vocabularies are dependencies of the official VML schema, not support for spreadsheet/presentation packages.

## Provenance

| Files | Source |
|---|---|
| `wml.xsd`, `dml-*.xsd`, `shared-*.xsd`, `vml-*.xsd` | ECMA-376 Part 4, 5th edition (December 2016), `OfficeOpenXML-XMLSchema-Transitional.zip`, unmodified |
| `xml.xsd`, `root-wml.xsd` | Written for this skill; not part of the ECMA distribution |
| `opc-coreProperties.xsd` | ECMA-376 Part 2, 5th edition (December 2021), `OpenPackagingConventions-XMLSchema.zip`, unmodified |
| `dc.xsd`, `dcterms.xsd`, `dcmitype.xsd` | Dublin Core's 2003-04-02 qualified XML schema distribution, unmodified, from `https://www.dublincore.org/schemas/xmls/qdc/2003/04/02/` |
| `root-metadata.xsd` | Authored compilation entry; imports local dependencies before the unmodified schemas' remote/bare imports |

Word writes Transitional OOXML by default, which is why the set comes from Part 4. Strict documents are refused before validation, so nothing here needs to cover them.

## Why the two authored files exist

`wml.xsd` and `shared-math.xsd` both import the `http://www.w3.org/XML/1998/namespace` namespace without a `schemaLocation`, so a schema processor has nothing to resolve `xml:space`, `xml:lang`, `xml:base` or `xml:id` against. Fetching the W3C copy at validation time would make validation depend on the network, so:

- `xml.xsd` declares those four attributes locally;
- `root-wml.xsd` is the compilation entry point. It imports `xml.xsd` first and the official VML closure, so the namespace is already in the schema set when the bare import in `wml.xsd` is reached.

The ECMA files are used exactly as published. Do not edit them; a refresh replaces them wholesale from the same archive.

The three `shared-documentProperties*.xsd` files use the same unmodified Part 4 source as the other shared schemas. `root-metadata.xsd` seeds the local XML and Dublin Core namespaces, then imports OPC core and extended/custom properties. Official UTF-8 BOMs are retained; loaders decode them rather than rewriting the source. Metadata grammar coverage does not prove custom-property PID uniqueness, relationship graph correctness, native application behavior or accuracy of stored derived counters.

## What these schemas do not cover

They describe WordprocessingML and the DrawingML and math it uses. Markup Compatibility (ECMA-376 Part 3) and Word's extension namespaces (w14, w15, w16 and later) are not declared here. Validation therefore runs on a temporary projection of each part, with ignorable extensions removed and alternate content resolved, and reports those namespaces as not checked. The projection is never written back.
