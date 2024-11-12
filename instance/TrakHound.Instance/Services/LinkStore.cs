namespace TrakHound.Instance.Services
{
    public static class LinkStore
    {
        private static readonly Dictionary<string, string> _links = new Dictionary<string, string>();
        private static readonly object _lock = new object();


        public static string Get(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                lock (_lock)
                {
                    var link = _links.GetValueOrDefault(id);
                    _links.Remove(id);
                    return link;
                }
            }

            return null;
        }

        public static void Add(string id, string link)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(link))
            {
                lock (_lock)
                {
                    if (!_links.ContainsKey(id))
                    {
                        _links.Add(id, link);
                    }
                }
            }
        }
    }
}
