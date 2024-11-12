using TrakHound.Blazor.Diagrams.Core.Geometry;
using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Instance.InstanceConfigurationInternal.Components
{
    public class RouterNode : NodeModel
    {
        public RouterNode(string configurationId, Point? position = null) : base(configurationId, position) { }


        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                if (Updated != null) Updated.Invoke(this, this);
            }
        }


        public event EventHandler<RouterNode> Updated;


        public PortModel GetOutputPort()
        {
            return Ports.OfType<RouterOutputPort>()?.FirstOrDefault();
        }
    }
}
