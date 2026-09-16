# Changelog

## v0.1.3 - 2026-09-16

- Added `CbBox` layout component with responsive padding and gap spacing.
- Enhanced `CbTabs` with `IconOnly`, `Size`, and `Type` parameters for icon-only and size variants.
- Added keyboard navigation to `CbTabs` (arrow keys, Home, End) with disabled tab handling.
- Added `Icon` property to `CbTabItem` for icon support in tab buttons.
- Added icon support to tab buttons with automatic label visibility toggle in icon-only mode.

## v0.1.2 - 2026-09-12

- Added `CbOrientation` enum and horizontal layout support to `CbRadioGroup`.
- Added `Disabled`, `ReadOnly`, `Invalid`, and `InvalidText` states to `CbRadioGroup`, with disabled/read-only enforcement on value changes.
- Added disabled and read-only pass-through, and a `Class` parameter, to `CbRadio`.
- Added a version/changelog consistency check to the NuGet publish workflow.
- Added bUnit tests for the new radio group states.
- Added `ChartPerformance` and `DataTable` icons, plus icon search in the demo.

## v0.1.0 - 2026-05-13

- Scaffolded the CarbonBlazor Razor Class Library, Blazor WebAssembly demo app, and bUnit/xUnit test project.
- Added runtime theme tokens for White, Gray 10, Gray 90, and Gray 100 themes.
- Added shell, navigation, action, form, content, feedback, overlay, structure, and data components.
- Added demo documentation pages for foundations, components, patterns, data, and accessibility.
- Added accessibility and rendering tests for key components.
- Added Apache-2.0 license metadata and local release tagging workflow.
