using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    internal class ObjectExplorerObjectNamespaceComparer : IComparer<ITrakHoundObjectEntity>
    {
        public int Compare(ITrakHoundObjectEntity x, ITrakHoundObjectEntity y)
        {
            if (IsDirectory(x) && !IsDirectory(y))
            {
                return 1;
            }
            else if (!IsDirectory(x) && IsDirectory(y))
            {
                return -1;
            }
            else
            {
                var xPath = x.GetAbsolutePath();
                var yPath = y.GetAbsolutePath();

                var xParentPath = TrakHoundPath.GetParentPath(xPath);
                var yParentPath = TrakHoundPath.GetParentPath(xPath);

                if (xParentPath == yParentPath)
                {
                    var xName = TrakHoundPath.GetObject(xPath);
                    var yName = TrakHoundPath.GetObject(yPath);

                    if (x != null && y != null)
                    {
                        var xV = xName.ToVersion();
                        var yV = yName.ToVersion();

                        if (xV != null && yV != null)
                        {
                            if (xV < yV) return -1;
                            if (xV > yV) return 1;
                            if (xV == yV) return 0;
                        }
                        else if (xName.IsNumeric() && yName.IsNumeric())
                        {
                            var xD = xName.ToDouble();
                            var yD = yName.ToDouble();

                            if (xD < yD) return -1;
                            if (xD > yD) return 1;
                            if (xD == yD) return 0;
                        }
                        else
                        {
                            return string.Compare(xName, yName);
                        }
                    }
                    else if (x != null)
                    {
                        return 1;
                    }
                }
                else
                {
                    return string.Compare(xPath, yPath);
                }
            }

            return 0;
        }

        private static bool IsDirectory(ITrakHoundObjectEntity entity)
        {
            if (entity != null) return entity.ContentType == TrakHoundObjectContentTypes.Directory;
            return false;
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
