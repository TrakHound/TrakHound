using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectExplorerTreeItemModel
    {
        public string Uuid { get; set; }

        public string Namespace { get; set; }

        public string Path { get; set; }

        public string Title { get; set; }

        public TrakHoundObjectContentType ContentType { get; set; }

        public int Depth { get; set; }

        public bool LastSibling { get; set; }

        public string Value { get; set; }

        public long ValueLastUpdated { get; set; }

        public bool RecentValue => UnixDateTime.Now - ValueLastUpdated < ObjectExplorerService.RecentLimit;


        public ObjectExplorerTreeItemModel(ITrakHoundObjectEntity entity)
        {
            if (entity != null)
            {
                Uuid = entity.Uuid;
                Namespace = entity.Namespace;
                Path = entity.Path;
                Title = entity.Name;
                ContentType = entity.ContentType.ConvertEnum<TrakHoundObjectContentType>();
                Depth = TrakHoundPath.GetPaths(entity.Path).Count() - 1;
            }
        }
    }
}
