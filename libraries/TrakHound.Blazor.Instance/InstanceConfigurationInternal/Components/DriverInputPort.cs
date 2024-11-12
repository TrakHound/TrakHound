using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Instance.InstanceConfigurationInternal.Components
{
    public class DriverInputPort : PortModel
    {
        public bool In { get; set; }

        public DriverInputPort(NodeModel parent, bool @in, PortAlignment alignment = PortAlignment.Bottom, Point? position = null, Size? size = null)
            : base(parent, alignment, position, size)
        {
            In = @in;
        }

        public DriverInputPort(string id, NodeModel parent, bool @in, PortAlignment alignment = PortAlignment.Bottom, Point? position = null, Size? size = null)
            : base(id, parent, alignment, position, size)
        {
            In = @in;
        }
    }
}
