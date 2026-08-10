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

    private sealed record Person(string Name, string Role);
}
