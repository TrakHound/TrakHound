using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Instance.InstanceConfigurationInternal.Components
{
    public class RouteNode : NodeModel
    {
        public RouteNode(string configurationId, Point? position = null) : base(configurationId, position) { }

        public string Pattern { get; set; }


        public PortModel GetInputPort()
        {
            return Ports.OfType<RouterInputPort>()?.FirstOrDefault();
        }

        public PortModel GetOutputPort()
        {
            return Ports.OfType<DriverOutputPort>()?.FirstOrDefault();
        }
    }
}
