// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text;

namespace TrakHound
{
    public class TrakHoundOnChangeFilter
    {
        private readonly Dictionary<string, string> _values;
        private readonly object _lock = new object();


        public TrakHoundOnChangeFilter()
        {
            _values = new Dictionary<string, string>();
        }


        public void Clear()
        {
            lock (_lock) _values.Clear();
        }


        public bool Filter(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock)
                {
                    var update = false;

                    var existing = _values.GetValueOrDefault(key);
                    if (existing != null)
                    {
                        update = existing != value;
                    }
                    else
                    {
                        update = true;
                    }

                    if (update)
                    {
                        if (existing != null) _values.Remove(key);
                        _values.Add(key, value);
                        return true;
                    }
                }
            }

            return false;
        }

        public static string CreateKey(params string[] parts)
        {
            if (parts != null)
            {
                var builder = new StringBuilder();
                for (var i = 0; i < parts.Length; i++)
                {
                    builder.Append(parts[i]);
                    if (i < parts.Length - 1) builder.Append(':');
                }
                return builder.ToString();
            }

            return null;
        }
    }
}
