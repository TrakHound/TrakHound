namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectExplorerNamespaceTreeItemModel : ObjectExplorerTreeItemModel
    {
        public string Id { get; set; }

        public string Namespace { get; set; }

        public string Title { get; set; }


        public ObjectExplorerNamespaceTreeItemModel(string ns)
        {
            Id = GetNamespaceId(ns);
            Namespace = ns;
            Title = ns;
        }


        public static string GetNamespaceId(string ns)
        {
            return $"NAMESPACE:{ns}".ToLower();
        }
    }
}
