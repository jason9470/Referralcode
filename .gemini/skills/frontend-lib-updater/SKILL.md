---
name: frontend-lib-updater
description: Updates frontend libraries in the wwwroot/lib directory by fetching the latest versions from CDNs (jsDelivr, cdnjs). Use when the user requests to update, upgrade, or change the version of a client-side library like jQuery, Bootstrap, or jQuery Validation.
---

# Frontend Library Updater

This skill automates the process of updating client-side libraries stored in `wwwroot/lib`.

## Workflow

1.  **Identify Target**: Locate the library folder within `wwwroot/lib`.
2.  **Determine Source**: Use the `references/cdns.md` to identify the correct URL pattern for the requested library and version.
3.  **Download & Replace**: Use the `scripts/update_lib.ps1` script to download and overwrite the files.
4.  **Verify**: Check the version header in the downloaded files to confirm successful update.

## CDNs Supported
- **jsDelivr**: `https://cdn.jsdelivr.net/npm/[package]@[version]/dist/[file]`
- **cdnjs**: `https://cdnjs.cloudflare.com/ajax/libs/[package]/[version]/[file]`

## Usage Examples
- "Update jquery-validation to 1.21.0"
- "Upgrade bootstrap to 5.3.3"
- "Fetch the latest version of jquery"
