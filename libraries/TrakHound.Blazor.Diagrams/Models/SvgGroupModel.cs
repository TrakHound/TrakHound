using System.Collections.Generic;
using TrakHound.Blazor.Diagrams.Core.Models;

namespace TrakHound.Blazor.Diagrams.Models;

public class SvgGroupModel : GroupModel
{
    public SvgGroupModel(IEnumerable<NodeModel> children, byte padding = 30, bool autoSize = true) : base(children, padding, autoSize)
    {
    }
}