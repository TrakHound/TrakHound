using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Diagrams.Models;

public class SvgNodeModel : NodeModel
{
    public SvgNodeModel(Point? position = null) : base(position)
    {
    }

    public SvgNodeModel(string id, Point? position = null) : base(id, position)
    {
    }
}