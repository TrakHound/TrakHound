using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Instance.InstanceConfigurationInternal.Components
{
    public class RedirectNode : NodeModel
    {
        public RedirectNode(string configurationId, Point? position = null) : base(configurationId, position) { }

        public string ConfigurationId { get; set; }

        public IEnumerable<string> Conditions { get; set; }

        public bool IsPublishOptionEnabled { get; set; }

        public bool IsEmptyOptionEnabled { get; set; }


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
