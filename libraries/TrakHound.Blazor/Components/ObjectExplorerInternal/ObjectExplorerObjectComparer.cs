using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    internal class ObjectExplorerObjectNamespaceComparer : IComparer<ITrakHoundObjectEntity>
    {
        public int Compare(ITrakHoundObjectEntity x, ITrakHoundObjectEntity y)
        {
            var xPath = x.GetAbsolutePath();
            var yPath = y.GetAbsolutePath();

            return string.Compare(xPath, yPath);
        }
    }

    internal class ObjectExplorerObjectCombinedComparer : IComparer<ITrakHoundObjectEntity>
    {
        public int Compare(ITrakHoundObjectEntity x, ITrakHoundObjectEntity y)
        {
            var xPath = x.Path;
            var yPath = y.Path;

            return string.Compare(xPath, yPath);
        }
    }
}
