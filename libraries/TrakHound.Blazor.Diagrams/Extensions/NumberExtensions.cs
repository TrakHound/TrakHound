using System.Globalization;

namespace TrakHound.Blazor.Diagrams.Core.Extensions;

public static class NumberExtensions
{
    public static string ToInvariantString(this double n) => n.ToString(CultureInfo.InvariantCulture);
}
