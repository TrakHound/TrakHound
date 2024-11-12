using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models.Base;
using System.Linq;

namespace TrakHound.Blazor.Diagrams.Core.Routers;

public class NormalRouter : Router
{
    public override Point[] GetRoute(Diagram diagram, BaseLinkModel link)
    {
        return link.Vertices.Select(v => v.Position).ToArray();
    }
}
