using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CarbonBlazor;

public static class CarbonBlazorServiceCollectionExtensions
{
    public static IServiceCollection AddCarbonBlazor(this IServiceCollection services)
    {
        services.AddScoped<CarbonBlazorJsModule>();
        return services;
    }
}

public sealed class CarbonBlazorJsModule(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private const string StylesHref = "_content/CarbonBlazor/carbon-blazor.css";

    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() => LoadModuleAsync(jsRuntime));

    private static async Task<IJSObjectReference> LoadModuleAsync(IJSRuntime jsRuntime)
    {
        var module = await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/CarbonBlazor/carbon-blazor.js");
        // Fallback style injection for consumers that did not add <CarbonBlazorStyles />
        // or the <link> tag manually. No-ops if a CarbonBlazor stylesheet is already present.
        try
        {
            await module.InvokeVoidAsync("ensureStyles", StylesHref);
        }
        catch (JSException)
        {
        }
        return module;
    }

    public Task<IJSObjectReference> GetModuleAsync() => _moduleTask.Value;

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
