using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Diagrams.Core.Options;

public class DiagramGroupOptions
{
    public bool Enabled { get; set; }
    
    public GroupFactory Factory { get; set; } = (diagram, children) => new GroupModel(children);
}