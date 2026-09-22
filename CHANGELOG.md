# Changelog

## v0.1.4 - 2026-09-23

- Rewrote `CbDropdown` as a generic, custom listbox with keyboard navigation and outside-click closing, replacing the native `<select>` wrapper.
- Added `Size` parameter to `CbDropdown` and `CbSelect`.
- Added `Danger` parameter to `CbMenuItem` for a destructive-action style.
- Added an open-state class to `CbOverflowMenu` and fixed its `aria-expanded` value to be a proper boolean string.
- Deferred `focusById` in `carbon-blazor.js` so focus changes made during keydown handling land after the browser's default key action.
- Added `Save`, `TableSplit`, and `ChartCandlestick` icons.
- Fixed `CbIcon` throwing `ArgumentOutOfRangeException` for `CbIconName.Table`, which had a sprite symbol but no switch case.

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
