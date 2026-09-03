using Bunit;
using CarbonBlazor.Components.Content;
using CarbonBlazor.Components.Actions;
using CarbonBlazor.Components.Data;
using CarbonBlazor.Components.Feedback;
using CarbonBlazor.Components.Forms;
using CarbonBlazor.Components.Foundations;
using CarbonBlazor.Components.Overlays;
using CarbonBlazor.Components.Shell;
using CarbonBlazor.Components.Structure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace CarbonBlazor.Tests;

public sealed class ComponentTests : BunitContext
{
    [Fact]
    public void ThemeProvider_RendersThemeTokenAttribute()
    {
        var cut = Render<CbThemeProvider>(parameters => parameters
            .Add(p => p.Theme, CbTheme.G100)
            .AddChildContent("content"));

        Assert.Equal("g100", cut.Find(".cb-theme").GetAttribute("data-theme"));
    }

    [Fact]
    public void Button_RendersVariantAndDisabledState()
    {
        var cut = Render<CbButton>(parameters => parameters
            .Add(p => p.Variant, CbButtonVariant.Secondary)
            .Add(p => p.Disabled, true)
            .AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.Contains("cb-btn--secondary", button.GetAttribute("class"));
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void TextInput_RaisesBindableValueChange()
    {
        string? value = null;
        var cut = Render<CbTextInput>(parameters => parameters
            .Add(p => p.Label, "Name")
            .Add(p => p.ValueChanged, changed => value = changed));

        cut.Find("input").Input("Ada");

        Assert.Equal("Ada", value);
        Assert.Equal("Name", cut.Find("label").TextContent);
    }

    [Fact]
    public void AccordionItem_TogglesExpandedState()
    {
        var cut = Render<CbAccordionItem>(parameters => parameters
            .Add(p => p.Title, "Details")
            .AddChildContent("Hidden content"));

        var button = cut.Find("button");
        Assert.Equal("false", button.GetAttribute("aria-expanded"));

        button.Click();

        Assert.Equal("true", cut.Find("button").GetAttribute("aria-expanded"));
        Assert.Contains("Hidden content", cut.Markup);
    }

    [Fact]
    public void Tabs_ClickingTab_SelectsPanelAndAriaState()
    {
        var tabs = new[]
        {
            new CbTabItem { Label = "Usage", Content = builder => builder.AddContent(0, "Usage panel") },
            new CbTabItem { Label = "Code", Content = builder => builder.AddContent(0, "Code panel") }
        };

        var cut = Render<CbTabs>(parameters => parameters.Add(p => p.Items, tabs));
        cut.FindAll("[role=tab]")[1].Click();

        Assert.Equal("true", cut.FindAll("[role=tab]")[1].GetAttribute("aria-selected"));
        Assert.Contains("Code panel", cut.Markup);
    }

    [Fact]
    public void Notification_UsesAlertRoleForErrors()
    {
        var cut = Render<CbNotification>(parameters => parameters
            .Add(p => p.Kind, CbNotificationKind.Error)
            .Add(p => p.Title, "Failed"));

        Assert.Equal("alert", cut.Find(".cb-notification").GetAttribute("role"));
        Assert.Equal("assertive", cut.Find(".cb-notification").GetAttribute("aria-live"));
    }

    [Fact]
    public void Modal_RendersDialogSemanticsWhenOpen()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbModal>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.Title, "Confirm")
            .AddChildContent("Confirm body"));

        var dialog = cut.Find("[role=dialog]");
        Assert.Equal("true", dialog.GetAttribute("aria-modal"));
        Assert.Contains("Confirm body", cut.Markup);
    }

    [Fact]
    public void DataTable_RendersRowsAndSortableHeader()
    {
        var people = new[]
        {
            new Person("Grace", "Compiler"),
            new Person("Ada", "Analyst")
        };
        var columns = new[]
        {
            new CbDataTableColumn<Person> { Header = "Name", Text = item => item.Name, SortKey = item => item.Name },
            new CbDataTableColumn<Person> { Header = "Role", Text = item => item.Role }
        };

        var cut = Render<CbDataTable<Person>>(parameters => parameters
            .Add(p => p.Items, people)
            .Add(p => p.Columns, columns)
            .Add(p => p.Title, "People"));

        cut.Find(".cb-data-table__sort").Click();

        Assert.Contains("People", cut.Markup);
        Assert.Contains("Ada", cut.Find("tbody tr:first-child").TextContent);
    }

    [Fact]
    public void DataTable_RendersFooterInsideContainer()
    {
        var columns = new[]
        {
            new CbDataTableColumn<Person> { Header = "Name", Text = item => item.Name }
        };

        var cut = Render<CbDataTable<Person>>(parameters => parameters
            .Add(p => p.Items, new[] { new Person("Ada", "Analyst") })
            .Add(p => p.Columns, columns)
            .Add(p => p.Footer, builder => builder.AddMarkupContent(0, "<span class=\"pager\">pager</span>")));

        Assert.NotNull(cut.Find(".cb-data-table .cb-data-table__footer .pager"));
    }

    [Fact]
    public void Pagination_NextButtonRaisesPageChanged()
    {
        var page = 1;
        var cut = Render<CbPagination>(parameters => parameters
            .Add(p => p.TotalItems, 30)
            .Add(p => p.Page, page)
            .Add(p => p.PageChanged, value => page = value));

        cut.Find("button[aria-label='Next page']").Click();

        Assert.Equal(2, page);
    }

    [Fact]
    public void Pagination_PreviousButtonHasAriaLabel()
    {
        var cut = Render<CbPagination>(parameters => parameters
            .Add(p => p.TotalItems, 30)
            .Add(p => p.Page, 2));

        Assert.NotNull(cut.Find("button[aria-label='Previous page']"));
    }

    [Fact]
    public void Checkbox_BindsCheckedValue()
    {
        var value = false;
        var cut = Render<CbCheckbox>(parameters => parameters
            .Add(p => p.ValueChanged, changed => value = changed)
            .AddChildContent("Accept"));

        cut.Find("input").Change(true);

        Assert.True(value);
    }

    [Fact]
    public void Toggle_TogglesBoolValue()
    {
        var value = false;
        var cut = Render<CbToggle>(parameters => parameters
            .Add(p => p.ValueChanged, changed => value = changed)
            .Add(p => p.Label, "Enabled"));

        cut.Find("input").Change(true);

        Assert.True(value);
    }

    [Fact]
    public void TextArea_BindsValue()
    {
        string? value = null;
        var cut = Render<CbTextArea>(parameters => parameters
            .Add(p => p.Label, "Notes")
            .Add(p => p.ValueChanged, changed => value = changed));

        cut.Find("textarea").Input("Line one");

        Assert.Equal("Line one", value);
    }

    [Fact]
    public void Slider_RaisesValueChanged()
    {
        double value = 0;
        var cut = Render<CbSlider>(parameters => parameters
            .Add(p => p.ValueChanged, changed => value = changed));

        cut.Find("input").Input("42");

        Assert.Equal(42, value);
    }

    [Fact]
    public void Select_BindsSelectedValue()
    {
        string? value = null;
        var cut = Render<CbSelect>(parameters => parameters
            .Add(p => p.Label, "Choice")
            .Add(p => p.ValueChanged, changed => value = changed)
            .AddChildContent("<option value=\"a\">A</option><option value=\"b\">B</option>"));

        cut.Find("select").Change("b");

        Assert.Equal("b", value);
    }

    [Fact]
    public void Select_InvalidAddsInvalidClass()
    {
        var cut = Render<CbSelect>(parameters => parameters
            .Add(p => p.Label, "Choice")
            .Add(p => p.Invalid, true));

        Assert.Contains("cb-select--invalid", cut.Find("select").GetAttribute("class"));
    }

    [Fact]
    public void Search_RaisesValueChanged()
    {
        string? value = null;
        var cut = Render<CbSearch>(parameters => parameters
            .Add(p => p.Label, "Search")
            .Add(p => p.ValueChanged, changed => value = changed));

        cut.Find("input").Input("carbon");

        Assert.Equal("carbon", value);
    }

    [Fact]
    public void Tag_RendersKindClass()
    {
        var cut = Render<CbTag>(parameters => parameters
            .Add(p => p.Kind, CbTagKind.Blue)
            .AddChildContent("Beta"));

        Assert.Contains("cb-tag--blue", cut.Find(".cb-tag").GetAttribute("class"));
    }

    [Fact]
    public void Tag_DismissibleFiresOnDismiss()
    {
        var dismissed = false;
        var cut = Render<CbTag>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .Add(p => p.OnDismiss, () => dismissed = true)
            .AddChildContent("Beta"));

        cut.Find("button[aria-label='Remove tag']").Click();

        Assert.True(dismissed);
    }

    [Fact]
    public void ProgressBar_HasProgressbarRole()
    {
        var cut = Render<CbProgressBar>(parameters => parameters
            .Add(p => p.Value, 40)
            .Add(p => p.Max, 80));

        var progress = cut.Find("[role=progressbar]");
        Assert.Equal("0", progress.GetAttribute("aria-valuemin"));
        Assert.Equal("80", progress.GetAttribute("aria-valuemax"));
        Assert.Equal("40", progress.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void InlineLoading_ShowsLoadingState()
    {
        var cut = Render<CbInlineLoading>(parameters => parameters.Add(p => p.Text, "Saving"));

        Assert.Equal("status", cut.Find(".cb-inline-loading").GetAttribute("role"));
        Assert.Contains("Saving", cut.Markup);
    }

    [Fact]
    public void Heading_DefaultsToH1WhenStandalone()
    {
        var cut = Render<CbHeading>(parameters => parameters.AddChildContent("Title"));

        Assert.NotNull(cut.Find("h1"));
        Assert.Contains("Title", cut.Find("h1").TextContent);
    }

    [Fact]
    public void Heading_AdvancesLevelWithNestedSections()
    {
        var cut = Render<CbSection>(parameters => parameters.AddChildContent<CbHeading>(h => h.AddChildContent("One"))
            .AddChildContent<CbSection>(s => s.AddChildContent<CbHeading>(h => h.AddChildContent("Two"))));

        Assert.Equal("One", cut.Find("section > h1").TextContent);
        Assert.Equal("Two", cut.Find("section section > h2").TextContent);
    }

    [Fact]
    public void Section_RespectsExplicitLevel()
    {
        var cut = Render<CbSection>(parameters => parameters
            .Add(p => p.Level, 3)
            .AddChildContent<CbHeading>(h => h.AddChildContent("Deep")));

        Assert.Equal("Deep", cut.Find("h3").TextContent);
    }

    [Fact]
    public void Section_RendersCustomTag()
    {
        var cut = Render<CbSection>(parameters => parameters
            .Add(p => p.Tag, "article")
            .AddChildContent<CbHeading>(h => h.AddChildContent("A")));

        Assert.Equal("A", cut.Find("article > h1").TextContent);
    }

    [Fact]
    public void NotificationQueue_AddsAndRemovesItems()
    {
        var cut = Render<CbNotificationQueue>(parameters => parameters
            .Add(p => p.AutoDismiss, 0));

        Guid id = Guid.Empty;
        cut.InvokeAsync(() => id = cut.Instance.Add(CbNotificationKind.Success, "Saved", "Done"));

        Assert.Single(cut.FindAll(".cb-notification"));
        Assert.Contains("Saved", cut.Markup);

        cut.InvokeAsync(() => cut.Instance.Remove(id));
        Assert.Empty(cut.FindAll(".cb-notification"));
    }

    [Fact]
    public void NotificationQueue_HonorsMaxItems()
    {
        var cut = Render<CbNotificationQueue>(parameters => parameters
            .Add(p => p.AutoDismiss, 0)
            .Add(p => p.MaxItems, 2));

        cut.InvokeAsync(() =>
        {
            cut.Instance.Add(CbNotificationKind.Info, "One");
            cut.Instance.Add(CbNotificationKind.Info, "Two");
            cut.Instance.Add(CbNotificationKind.Info, "Three");
        });

        Assert.Equal(2, cut.FindAll(".cb-notification").Count);
        Assert.DoesNotContain("One", cut.Markup);
        Assert.Contains("Three", cut.Markup);
    }

    [Fact]
    public void NotificationQueue_CloseButtonMatchesNotification()
    {
        var cut = Render<CbNotificationQueue>(parameters => parameters
            .Add(p => p.AutoDismiss, 0));

        cut.InvokeAsync(() => cut.Instance.Add(CbNotificationKind.Info, "Hi"));

        Assert.Single(cut.FindAll("button.cb-notification__close"));
    }

    [Fact]
    public void Link_RendersHref()
    {
        var cut = Render<CbLink>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .AddChildContent("Docs"));

        Assert.Equal("/docs", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void Tooltip_HasTooltipRole()
    {
        var cut = Render<CbTooltip>(parameters => parameters
            .Add(p => p.Text, "More detail")
            .AddChildContent("Help"));

        var tooltip = cut.Find("[role=tooltip]");
        Assert.Equal("More detail", tooltip.TextContent);
        Assert.Equal(tooltip.Id, cut.Find(".cb-tooltip__trigger").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void CodeSnippet_CopyButton_CopiesCodeViaInterop()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.Setup<bool>("copyElementText", _ => true).SetResult(true);

        var cut = Render<CbCodeSnippet>(parameters => parameters
            .Add(p => p.Code, "dotnet build"));

        var button = cut.Find("button.cb-code-snippet__copy");
        Assert.Equal("Copy to clipboard", button.GetAttribute("aria-label"));

        button.Click();

        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "copyElementText");
        Assert.Contains("Copied!", cut.Find(".cb-code-snippet__feedback").TextContent);
    }

    [Fact]
    public void CodeSnippet_HidesCopyButton_WhenDisabled()
    {
        var cut = Render<CbCodeSnippet>(parameters => parameters
            .Add(p => p.Code, "x")
            .Add(p => p.ShowCopyButton, false));

        Assert.Empty(cut.FindAll("button.cb-code-snippet__copy"));
    }

    [Fact]
    public void Popover_TogglesOpenState()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbPopover>(parameters => parameters
            .Add(p => p.Trigger, builder => builder.AddContent(0, "Open"))
            .AddChildContent("Panel"));

        cut.Find("button").Click();

        Assert.True(cut.Find("button").HasAttribute("aria-expanded"));
        Assert.Contains("Panel", cut.Find("[role=dialog]").TextContent);
    }

    [Fact]
    public void TreeViewNode_ExpandsOnArrowRight()
    {
        var node = new CbTreeNode
        {
            Label = "Parent",
            Children = { new CbTreeNode { Label = "Child" } }
        };
        var cut = Render<CbTreeView>(parameters => parameters.Add(p => p.Nodes, [node]));

        cut.Find("button").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        Assert.True(node.Expanded);
        Assert.Contains("Child", cut.Markup);
    }

    [Fact]
    public void Tile_ClickableRaisesOnClick()
    {
        var clicked = false;
        var cut = Render<CbTile>(parameters => parameters
            .Add(p => p.Kind, CbTileKind.Clickable)
            .Add(p => p.OnClick, _ => clicked = true)
            .AddChildContent("Open tile"));

        cut.Find("button.cb-tile").Click();

        Assert.True(clicked);
    }

    [Fact]
    public void Tile_SelectableRaisesSelectedChanged()
    {
        var selected = false;
        var cut = Render<CbTile>(parameters => parameters
            .Add(p => p.Kind, CbTileKind.Selectable)
            .Add(p => p.SelectedChanged, changed => selected = changed)
            .AddChildContent("Select tile"));

        cut.Find("input").Change(true);

        Assert.True(selected);
    }

    [Fact]
    public void SideNavLink_HasAriaCurrentWhenActive()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/components");
        var cut = Render<CbSideNavLink>(parameters => parameters
            .Add(p => p.Href, "/components")
            .Add(p => p.Match, NavLinkMatch.All)
            .AddChildContent("Components"));

        Assert.Equal("page", cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void SideNavLink_RendersIconFromEnumAndKeepsLabel()
    {
        var cut = Render<CbSideNavLink>(parameters => parameters
            .Add(p => p.Href, "/overview")
            .Add(p => p.IconName, CbIconName.Home)
            .AddChildContent("Overview"));

        Assert.NotNull(cut.Find(".cb-side-nav__icon"));
        Assert.Equal("Overview", cut.Find(".cb-side-nav__label").TextContent.Trim());
    }

    [Fact]
    public void SideNav_AppliesCollapsedClass()
    {
        var cut = Render<CbSideNav>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.Collapsed, true)
            .AddChildContent("<a class='cb-side-nav__link'>Link</a>"));

        var className = cut.Find("aside").GetAttribute("class") ?? string.Empty;
        Assert.Contains("cb-side-nav--open", className);
        Assert.Contains("cb-side-nav--collapsed", className);
    }

    [Fact]
    public void SideNav_PersistentIsOptInAndDefaultsOff()
    {
        var cut = Render<CbSideNav>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent("<a class='cb-side-nav__link'>Link</a>"));

        var className = cut.Find("aside").GetAttribute("class") ?? string.Empty;
        Assert.DoesNotContain("cb-side-nav--persistent", className);
    }

    [Fact]
    public void SideNav_FixedAppliesModifierClass()
    {
        var cut = Render<CbSideNav>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.Fixed, true)
            .AddChildContent("<a class='cb-side-nav__link'>Link</a>"));

        var className = cut.Find("aside").GetAttribute("class") ?? string.Empty;
        Assert.Contains("cb-side-nav--persistent", className);
    }

    [Fact]
    public void SideNav_DoesNotSetAriaHiddenAttribute()
    {
        var cut = Render<CbSideNav>(parameters => parameters
            .Add(p => p.Open, false)
            .AddChildContent("<a class='cb-side-nav__link'>Link</a>"));

        Assert.False(cut.Find("aside").HasAttribute("aria-hidden"));
    }

    [Fact]
    public void Header_DefaultMenuButtonTogglesSideNavOpenState()
    {
        var sideNavOpen = false;
        var cut = Render<CbHeader>(parameters => parameters
            .Add(p => p.SideNavOpen, sideNavOpen)
            .Add(p => p.SideNavOpenChanged, value => sideNavOpen = value));

        cut.Find("button.cb-header__button").Click();

        Assert.True(sideNavOpen);
    }

    [Fact]
    public void Header_CustomMenuToggleCallbackOverridesDefaultToggle()
    {
        var sideNavOpen = false;
        var callbackTriggered = false;
        var cut = Render<CbHeader>(parameters => parameters
            .Add(p => p.SideNavOpen, sideNavOpen)
            .Add(p => p.SideNavOpenChanged, value => sideNavOpen = value)
            .Add(p => p.OnMenuToggle, () => callbackTriggered = true));

        cut.Find("button.cb-header__button").Click();

        Assert.True(callbackTriggered);
        Assert.False(sideNavOpen);
    }

    [Fact]
    public void MenuButton_RegistersClickOutsideWhenOpened()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbMenuButton>(parameters => parameters
            .Add(p => p.Label, "Actions")
            .AddChildContent<CbMenuItem>(item => item.AddChildContent("Archive")));

        cut.Find("button").Click();

        Assert.Contains("Archive", cut.Markup);
        Assert.Contains(JSInterop.Invocations, invocation => invocation.Identifier == "import");
    }

    [Fact]
    public void OverflowMenu_TriggerDefaultsToVerticalOverflowIcon()
    {
        var cut = Render<CbOverflowMenu>(parameters => parameters
            .Add(p => p.Label, "Row actions")
            .AddChildContent<CbMenuItem>(item => item.AddChildContent("Edit")));

        Assert.Equal(
            "_content/CarbonBlazor/icons.svg#overflow-menu-vertical",
            cut.Find("button.cb-icon-btn use").GetAttribute("href"));
    }

    [Fact]
    public void OverflowMenu_EscapeKeyClosesMenu()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbOverflowMenu>(parameters => parameters
            .Add(p => p.Label, "Row actions")
            .AddChildContent<CbMenuItem>(item => item.AddChildContent("Edit")));

        cut.Find("button.cb-icon-btn").Click();
        Assert.Contains("cb-menu--overflow", cut.Markup);

        cut.Find("div.cb-overflow-menu").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.DoesNotContain("cb-menu--overflow", cut.Markup);
    }

    [Fact]
    public void OverflowMenu_ItemClickClosesMenu()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var clicked = false;
        var cut = Render<CbOverflowMenu>(parameters => parameters
            .Add(p => p.Label, "Row actions")
            .AddChildContent<CbMenuItem>(item => item
                .Add(i => i.OnClick, () => clicked = true)
                .AddChildContent("Edit")));

        cut.Find("button.cb-icon-btn").Click();
        cut.Find("button.cb-menu__item").Click();

        Assert.True(clicked);
        Assert.DoesNotContain("cb-menu--overflow", cut.Markup);
    }

    [Fact]
    public void MenuButton_ItemClickClosesMenu()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbMenuButton>(parameters => parameters
            .Add(p => p.Label, "Actions")
            .AddChildContent<CbMenuItem>(item => item.AddChildContent("Archive")));

        cut.Find("button.cb-btn").Click();
        Assert.Contains("cb-menu__item", cut.Markup);

        cut.Find("button.cb-menu__item").Click();

        Assert.DoesNotContain("cb-menu__item", cut.Markup);
    }

    [Fact]
    public void OverflowMenu_IconOverridesDefaultGlyph()
    {
        var cut = Render<CbOverflowMenu>(parameters => parameters
            .Add(p => p.Label, "Settings")
            .Add(p => p.Icon, "⚙")
            .AddChildContent<CbMenuItem>(item => item.AddChildContent("Preferences")));

        Assert.Empty(cut.FindAll("button.cb-icon-btn use"));
        Assert.Contains("⚙", cut.Markup);
    }

    [Fact]
    public void Icon_WithoutLabel_IsHiddenFromAccessibilityTree()
    {
        var cut = Render<CbIcon>(parameters => parameters
            .Add(p => p.Name, CbIconName.Home));

        var svg = cut.Find("svg");
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.False(svg.HasAttribute("aria-label"));
        Assert.False(svg.HasAttribute("role"));
        Assert.Equal("_content/CarbonBlazor/icons.svg#home", cut.Find("use").GetAttribute("href"));
    }

    [Fact]
    public void Icon_WithLabel_ExposesImgRoleAndAriaLabel()
    {
        var cut = Render<CbIcon>(parameters => parameters
            .Add(p => p.Name, CbIconName.Settings)
            .Add(p => p.Label, "Settings"));

        var svg = cut.Find("svg");
        Assert.False(svg.HasAttribute("aria-hidden"));
        Assert.Equal("Settings", svg.GetAttribute("aria-label"));
        Assert.Equal("img", svg.GetAttribute("role"));
        Assert.Equal("_content/CarbonBlazor/icons.svg#settings", cut.Find("use").GetAttribute("href"));
    }

    [Fact]
    public void Icon_SizeTokenControlsDimensionsAndSizeClass()
    {
        var cut = Render<CbIcon>(parameters => parameters
            .Add(p => p.Name, CbIconName.Add)
            .Add(p => p.SizeToken, CbIconSize.Size24));

        var svg = cut.Find("svg");
        Assert.Equal("24", svg.GetAttribute("width"));
        Assert.Equal("24", svg.GetAttribute("height"));
        Assert.Contains("cb-icon--lg", svg.GetAttribute("class"));
    }

    [Fact]
    public void SkipToContent_RendersHrefAndDefaultText()
    {
        var cut = Render<CbSkipToContent>(parameters => parameters
            .Add(p => p.Href, "#main-content"));

        var anchor = cut.Find("a.cb-skip-to-content");
        Assert.Equal("#main-content", anchor.GetAttribute("href"));
        Assert.Equal("Skip to main content", anchor.TextContent.Trim());
    }

    [Fact]
    public void SideNavDivider_RendersHrElement()
    {
        var cut = Render<CbSideNavDivider>();

        Assert.NotNull(cut.Find("hr.cb-side-nav__divider"));
    }

    [Fact]
    public void SideNavMenu_TogglesOpenStateAndRaisesCallback()
    {
        var open = false;
        var cut = Render<CbSideNavMenu>(parameters => parameters
            .Add(p => p.Text, "Kubernetes")
            .Add(p => p.Open, open)
            .Add(p => p.OpenChanged, value => open = value)
            .AddChildContent("<li class='cb-side-nav__menu-item'><a class='cb-side-nav__link' href='#'>Clusters</a></li>"));

        Assert.Equal("false", cut.Find("button.cb-side-nav__submenu").GetAttribute("aria-expanded"));

        cut.Find("button.cb-side-nav__submenu").Click();

        Assert.True(open);
    }

    [Fact]
    public void SideNavMenuItem_RendersNestedLink()
    {
        var cut = Render<CbSideNavMenuItem>(parameters => parameters
            .Add(p => p.Href, "/kubernetes/clusters")
            .AddChildContent("Clusters"));

        Assert.NotNull(cut.Find("a.cb-side-nav__link--nested"));
        Assert.Equal("Clusters", cut.Find(".cb-side-nav__label").TextContent.Trim());
    }

    [Fact]
    public void HeaderNavMenu_TogglesOpenStateOnTriggerClick()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbHeaderNavMenu>(parameters => parameters
            .Add(p => p.Text, "Manage")
            .AddChildContent("<li class='cb-header__menu-item'><a class='cb-header__link' href='#'>Account</a></li>"));

        Assert.Empty(cut.FindAll("ul.cb-header__menu"));

        cut.Find("button.cb-header__menu-title").Click();

        Assert.Single(cut.FindAll("ul.cb-header__menu"));
        Assert.Single(cut.FindAll("li.cb-header__menu-item"));
        Assert.Equal("true", cut.Find("button.cb-header__menu-title").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void HeaderMenuItem_RendersListItemWithLink()
    {
        var cut = Render<CbHeaderMenuItem>(parameters => parameters
            .Add(p => p.Href, "/account")
            .AddChildContent("Account"));

        Assert.NotNull(cut.Find("li.cb-header__menu-item"));
        Assert.Equal("Account", cut.Find("a.cb-header__link").TextContent.Trim());
    }

    [Fact]
    public void Header_MenuButtonSwapsIconAndAriaExpandedWithSideNavOpen()
    {
        var cut = Render<CbHeader>(parameters => parameters
            .Add(p => p.SideNavOpen, true));

        var button = cut.Find("button.cb-header__button");
        Assert.Equal("true", button.GetAttribute("aria-expanded"));
        Assert.Contains("close", cut.Find("button.cb-header__button use").GetAttribute("href"));
    }

    [Fact]
    public void Grid_FullWidthAppliesModifierClass()
    {
        var cut = Render<CbGrid>(parameters => parameters
            .Add(p => p.FullWidth, true)
            .AddChildContent("content"));

        var className = cut.Find("div").GetAttribute("class") ?? string.Empty;
        Assert.Contains("cb-layout-grid", className);
        Assert.Contains("cb-layout-grid--full-width", className);
    }

    [Fact]
    public void Row_CondensedAndNarrowApplyModifierClasses()
    {
        var cut = Render<CbRow>(parameters => parameters
            .Add(p => p.Condensed, true)
            .Add(p => p.Narrow, true));

        var className = cut.Find("div").GetAttribute("class") ?? string.Empty;
        Assert.Contains("cb-layout-row--condensed", className);
        Assert.Contains("cb-layout-row--narrow", className);
    }

    [Fact]
    public void Column_BreakpointParametersProduceSpanClasses()
    {
        var cut = Render<CbColumn>(parameters => parameters
            .Add(p => p.Sm, 4)
            .Add(p => p.Md, 8)
            .Add(p => p.Lg, 12));

        var className = cut.Find("div").GetAttribute("class") ?? string.Empty;
        Assert.Contains("cb-layout-col-sm-4", className);
        Assert.Contains("cb-layout-col-md-8", className);
        Assert.Contains("cb-layout-col-lg-12", className);
    }

    [Fact]
    public void Column_RejectsSpansBeyondThatBreakpointsColumnCount()
    {
        // sm has 4 columns and lg has 16; a span of 5 is only valid at lg.
        var cut = Render<CbColumn>(parameters => parameters
            .Add(p => p.Sm, 5)
            .Add(p => p.Lg, 16));

        var className = cut.Find("div").GetAttribute("class") ?? string.Empty;
        Assert.DoesNotContain("cb-layout-col-sm-5", className);
        Assert.Contains("cb-layout-col-lg-16", className);
    }

    [Fact]
    public void Column_NoGutterAppliesModifierClasses()
    {
        var cut = Render<CbColumn>(parameters => parameters
            .Add(p => p.NoGutter, true)
            .Add(p => p.NoGutterLeft, true)
            .Add(p => p.NoGutterRight, true));

        var className = cut.Find("div").GetAttribute("class") ?? string.Empty;
        Assert.Contains("cb-layout-col--no-gutter", className);
        Assert.Contains("cb-layout-col--no-gutter-left", className);
        Assert.Contains("cb-layout-col--no-gutter-right", className);
    }

    [Fact]
    public void CarbonBlazorStyles_EmitsStylesheetLink()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var head = Render<HeadOutlet>();
        Render<CarbonBlazorStyles>(parameters => parameters
            .Add(p => p.IncludeFont, false));

        Assert.Contains("_content/CarbonBlazor/carbon-blazor.css", head.Markup);
    }

    [Fact]
    public void CarbonBlazorStyles_AppliesPathPrefix()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var head = Render<HeadOutlet>();
        Render<CarbonBlazorStyles>(parameters => parameters
            .Add(p => p.IncludeFont, false)
            .Add(p => p.PathPrefix, "/myapp/"));

        Assert.Contains("/myapp/_content/CarbonBlazor/carbon-blazor.css", head.Markup);
    }

    [Fact]
    public void HeaderSearch_TriggerActivatesExpandedInput()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbHeaderSearch>(parameters => parameters
            .Add(p => p.Placeholder, "Search resources"));

        Assert.Empty(cut.FindAll("input.cb-header__search-input"));

        cut.Find("button.cb-header__search-trigger").Click();

        var input = cut.Find("input.cb-header__search-input");
        Assert.Equal("Search resources", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void HeaderSearch_RendersCustomResultsWhenActive()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbHeaderSearch>(parameters => parameters
            .Add(p => p.Active, true)
            .AddChildContent<CbHeaderSearchResult>(result => result
                .Add(r => r.Text, "web-prod-01")
                .Add(r => r.Description, "Virtual server")));

        var menu = cut.Find("ul.cb-header__search-menu");
        Assert.Equal("listbox", menu.GetAttribute("role"));
        Assert.Contains("web-prod-01", menu.TextContent);
        Assert.Contains("Virtual server", menu.TextContent);
        Assert.Equal("option", cut.Find("a.cb-header__search-link").GetAttribute("role"));
    }

    [Fact]
    public void HeaderSearch_EscapeDeactivatesAndClearsValue()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        string? value = "web";
        var cut = Render<CbHeaderSearch>(parameters => parameters
            .Add(p => p.Active, true)
            .Add(p => p.Value, "web")
            .Add(p => p.ValueChanged, changed => value = changed));

        cut.Find("div.cb-header__search-field").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.Empty(cut.FindAll("input.cb-header__search-input"));
        Assert.Equal(string.Empty, value);
    }

    [Fact]
    public void HeaderSearchResult_SelectRaisesCallback()
    {
        var selected = false;
        var cut = Render<CbHeaderSearchResult>(parameters => parameters
            .Add(p => p.Text, "billing-db")
            .Add(p => p.OnSelect, () => selected = true));

        cut.Find("a.cb-header__search-link").Click();

        Assert.True(selected);
    }

    [Fact]
    public void HeaderSearch_ClosesAfterResultSelectedByDefault()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbHeaderSearch>(parameters => parameters
            .Add(p => p.Active, true)
            .AddChildContent<CbHeaderSearchResult>(result => result.Add(r => r.Text, "billing-db")));

        cut.Find("a.cb-header__search-link").Click();

        Assert.Empty(cut.FindAll("input.cb-header__search-input"));
    }

    [Fact]
    public void HeaderSearch_KeepsOpenAfterSelectWhenCloseOnSelectFalse()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbHeaderSearch>(parameters => parameters
            .Add(p => p.Active, true)
            .Add(p => p.CloseOnSelect, false)
            .AddChildContent<CbHeaderSearchResult>(result => result.Add(r => r.Text, "billing-db")));

        cut.Find("a.cb-header__search-link").Click();

        Assert.Single(cut.FindAll("input.cb-header__search-input"));
    }

    [Fact]
    public void Header_RendersSearchFragment()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = Render<CbHeader>(parameters => parameters
            .Add(p => p.Search, "<div class=\"probe\">search slot</div>"));

        Assert.Contains("search slot", cut.Find(".probe").TextContent);
    }

    private sealed record Person(string Name, string Role);
}
