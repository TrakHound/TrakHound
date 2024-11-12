using System;
using System.Threading.Tasks;
using TrakHound.Blazor.Diagrams.Core.Models;
using TrakHound.Blazor.Diagrams.Core.Models.Base;

namespace TrakHound.Blazor.Diagrams.Core.Options;

public class DiagramConstraintsOptions
{
    public Func<NodeModel, ValueTask<bool>> ShouldDeleteNode { get; set; } = _ => ValueTask.FromResult(true);
    public Func<BaseLinkModel, ValueTask<bool>> ShouldDeleteLink { get; set; } = _ => ValueTask.FromResult(true);
    public Func<GroupModel, ValueTask<bool>> ShouldDeleteGroup { get; set; } = _ => ValueTask.FromResult(true);
}