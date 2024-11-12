using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models.Base;

namespace TrakHound.Blazor.Diagrams.Core.Controls;

public abstract class Control
{
    public abstract Point? GetPosition(Model model);
}