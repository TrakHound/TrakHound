using TrakHound.Blazor.Diagrams.Core.Models.Base;
using TrakHound.Blazor.Diagrams.Core.Events;

namespace TrakHound.Blazor.Diagrams.Core.Behaviors;

public class SelectionBehavior : Behavior
{
    public SelectionBehavior(Diagram diagram) : base(diagram)
    {
        Diagram.PointerDown += OnPointerDown;
    }

    private void OnPointerDown(Model? model, PointerEventArgs e)
    {
        var ctrlKey = e.CtrlKey;
        switch (model)
        {
            case null:
                Diagram.UnselectAll();
                break;
            case SelectableModel sm when ctrlKey && sm.Selected:
                Diagram.UnselectModel(sm);
                break;
            case SelectableModel sm:
            {
                if (!sm.Selected)
                {
                    Diagram.SelectModel(sm, !ctrlKey || !Diagram.Options.AllowMultiSelection);
                }

                break;
            }
        }
    }

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
    }
}
