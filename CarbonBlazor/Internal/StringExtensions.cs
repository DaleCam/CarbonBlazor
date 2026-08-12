namespace CarbonBlazor.Internal;

public static class StringExtensions
{
    /// <summary>
    /// Answers true if this String is either null or empty.
    /// </summary>
    /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
    public static bool IsNullOrEmpty(this string? s)
    {
        return string.IsNullOrEmpty(s);
    }
    
    /// <summary>
    /// Answers true if this String is either null or consists only of white-space characters.
    /// </summary>
    /// <remarks>I'm so tired of typing String.IsNullOrWhiteSpace(s)</remarks>
    public static bool IsNullOrWhiteSpace(this string? s)
    {
        return string.IsNullOrWhiteSpace(s);    
    }
}