using TrakHound.Blazor.Diagrams.Core.Models.Base;
using TrakHound.Blazor.Diagrams.Models;

namespace TrakHound.Blazor.Diagrams.Extensions;

public static class ModelExtensions
{
    public static bool IsSvg(this Model model)
    {
        return model is SvgNodeModel or SvgGroupModel or BaseLinkModel;
    }
}