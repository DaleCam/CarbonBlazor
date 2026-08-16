# Fork Notice

I forked this from the good dev here: https://github.com/portalbh/CarbonBlazor
He built a fantastic foundation and its a great start!
Im just building on his awesome work as I needed a faster turnaround for my projects that im actively developing.

# Carbon Blazor - AI Skills

CarbonBlazor is a Blazor WebAssembly template and Razor Class Library inspired by the Carbon Design System. It provides reusable Blazor components, runtime CSS-variable themes, accessibility-focused interaction behavior, and a documentation/demo app.

This project is not affiliated with, sponsored by, or endorsed by IBM. Carbon Design System and IBM names belong to their respective owners.

## Published Demo

https://carbonblazor.dale.workers.dev/

## Projects

- `CarbonBlazor`: reusable Razor Class Library with components, tokens, CSS, and JS interop helpers.
- `CarbonBlazor.Demo`: Blazor WebAssembly documentation and component demo app.
- `CarbonBlazor.Tests`: bUnit/xUnit tests for rendering, accessibility attributes, state changes, and interactions.

## Run

```powershell
dotnet restore
dotnet build CarbonBlazor.slnx
dotnet test CarbonBlazor.slnx
dotnet run --project CarbonBlazor.Demo
```

The demo app loads the component stylesheet from:

```html
<link rel="stylesheet" href="_content/CarbonBlazor/carbon-blazor.css" />
```

## Basic Usage

Register the library services:

```csharp
builder.Services.AddCarbonBlazor();
```

Wrap the app in a theme provider:

```razor
<CbThemeProvider Theme="CbTheme.G100">
    <CbButton>Save</CbButton>
</CbThemeProvider>
```

Use bindable form components:

```razor
<CbTextInput Label="Name" @bind-Value="name" />
<CbToggle Label="Enable feature" @bind-Value="enabled" />
```

Use data components:

```razor
<CbDataTable TItem="ReleaseTask"
             Title="Release tasks"
             Items="tasks"
             Columns="columns"
             Selectable="true" />
```

## AI Quickstart

Use these files as the first context for AI coding agents (Cursor, Claude, GitHub Copilot):

- [.cursorrules](.cursorrules): repository-level coding and generation rules.
- [.ai/components.md](.ai/components.md): component API reference (parameters, required fields, slots).
- [.ai/tokens.md](.ai/tokens.md): CarbonBlazor token and theme mapping (White, G10, G90, G100, Teal).
- [.ai/best-practices.md](.ai/best-practices.md): page-generation patterns for CarbonBlazor.Demo.

Recommended prompt pattern for agents:

1. Read `.cursorrules` and `.ai/best-practices.md` first.
2. Look up component signatures in `.ai/components.md`.
3. Use `.ai/tokens.md` for any custom styling decisions.
4. Generate code using CarbonBlazor components and `CbThemeProvider`, not raw HTML control replacements.

## Included Components

The v0.1.0 catalog includes shell/navigation, buttons, menus, form controls, notifications, loading states, progress, tags, tiles, lists, code snippets, modal, popover, tooltip, toggletip, accordion, tabs, structured list, data table, pagination, and tree view.

## Theming

Themes are runtime CSS custom properties. Supported themes are:

- `White`
- `G10`
- `G90`
- `G100`

Token groups include background, layer, field, text, icon, border, focus, interactive, support, overlay, spacing, and motion values.

## Accessibility

Components use native HTML controls where possible and add ARIA semantics for richer widgets. Covered behavior includes visible focus, `aria-expanded`, `aria-selected`, `role=dialog`, `aria-modal`, notification live regions, sortable table headers, labeled row selection, keyboard tab behavior, Escape-to-close modal behavior, and responsive pagination.

## Versioning and Publishing

The local release tag is `v0.1.0`. GitHub publishing is intentionally deferred until a public remote exists:

```powershell
git remote add origin https://github.com/<owner>/<repo>.git
git push -u origin master
git push origin v0.1.0
```

## Cloudflare Workers

This repo includes `wrangler.jsonc` for Workers static assets. Use these build settings:

```text
Build command: npm run build:cloudflare
Deploy command: npx wrangler deploy
Root directory: /
```

## Research References

- Carbon component catalog: https://carbondesignsystem.com/components/overview/components/
- Carbon React framework docs: https://carbondesignsystem.com/developing/frameworks/react/
- Carbon themes and tokens: https://carbondesignsystem.com/elements/themes/overview/
- Carbon source repository: https://github.com/carbon-design-system/carbon
