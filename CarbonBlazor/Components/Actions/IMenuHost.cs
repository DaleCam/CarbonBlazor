namespace CarbonBlazor.Components.Actions;

/// <summary>
/// Implemented by menu container components so nested <see cref="CbMenuItem"/> elements
/// can close the menu after an item is activated.
/// </summary>
internal interface IMenuHost
{
    void CloseMenu();
}
