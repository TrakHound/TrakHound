using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    internal class ObjectExplorerObjectComparer : IComparer<ITrakHoundObjectEntity>
    {
        public int Compare(ITrakHoundObjectEntity x, ITrakHoundObjectEntity y)
        {
            var xPath = x.GetAbsolutePath();
            var yPath = y.GetAbsolutePath();

            return string.Compare(xPath, yPath);
        }

        //public int Compare(ITrakHoundObjectEntity x, ITrakHoundObjectEntity y)
        //{
        //    if (x != null && y != null)
        //    {
        //        var namespaceCompare = string.Compare(x.Namespace, y.Namespace);
        //        if (namespaceCompare != 0)
        //        {
        //            return namespaceCompare;
        //        }
        //        else
        //        {
        //            var xParentPath = TrakHoundPath.GetParentPath(x.Path);
        //            var yParentPath = TrakHoundPath.GetParentPath(y.Path);

        //            var pathCompare = string.Compare(xParentPath, yParentPath);
        //            if (pathCompare != 0)
        //            {
        //                return pathCompare;
        //            }
        //            else
        //            {
        //                var xV = x.Name.ToVersion();
        //                var yV = y.Name.ToVersion();

        //                if (xV != null && yV != null)
        //                {
        //                    if (xV < yV) return -1;
        //                    if (xV > yV) return 1;
        //                    if (xV == yV) return 0;
        //                }
        //                else if (x.Name.IsNumeric() && y.Name.IsNumeric())
        //                {
        //                    var xD = x.Name.ToDouble();
        //                    var yD = y.Name.ToDouble();

        //                    if (xD < yD) return -1;
        //                    if (xD > yD) return 1;
        //                    if (xD == yD) return 0;
        //                }
        //                else
        //                {
        //                    return string.Compare(x.Name, y.Name);
        //                }
        //            }
        //        }
        //    }
        //    else if (x != null)
        //    {
        //        return 1;
        //    }

        //    return 0;
        //}
    }
}
