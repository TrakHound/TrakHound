using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models.Base;

namespace TrakHound.Blazor.Diagrams.Core.Positions;

public interface IPositionProvider
{
    public Point? GetPosition(Model model);
}