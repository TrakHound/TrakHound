using TrakHound.Blazor.Diagrams.Core.Options;

namespace TrakHound.Blazor.Diagrams.Options;

public class BlazorDiagramLinkOptions : DiagramLinkOptions
{
    public string DefaultColor { get; set; } = "black";
    public string DefaultSelectedColor { get; set; } = "rgb(110, 159, 212)";
}