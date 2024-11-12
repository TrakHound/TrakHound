using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Instance.InstanceConfigurationInternal.Components
{
    public class TargetNode : NodeModel
    {
        public TargetNode(string configurationId, Point? position = null) : base(configurationId, position) { }

        public string ConfigurationId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }


        public PortModel GetInputPort()
        {
            return Ports.OfType<DriverInputPort>()?.FirstOrDefault();
        }

        public PortModel GetOutputPort()
        {
            return Ports.OfType<RedirectPort>()?.FirstOrDefault();
        }
    }
}
