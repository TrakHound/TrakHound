// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.ExplorerInternal.Components.Entities
{
    public struct EntityQuery
    {
        private Dictionary<string, object> _parameters;
        public Dictionary<string, object> Parameters
        {
            get
            {
                if (_parameters == null) _parameters = new Dictionary<string, object>();
                return _parameters;
            }
        }


        public string GetParameter(string key)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                var value = Parameters.GetValueOrDefault(key);
                if (value != null)
                {
                    return value.ToString();
                }
            }

            return null;
        }

        public T GetParameter<T>(string key)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                var value = Parameters.GetValueOrDefault(key);
                if (value != null)
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }


        public void AddParameter(string key, object value)
        {
            Parameters.Remove(key);
            Parameters.Add(key, value);
        }

        public void ClearParameters()
        {
            Parameters.Clear();
        }


        public override string ToString()
        {
            if (!Parameters.IsNullOrEmpty())
            {
                var parameters = new List<string>();
                foreach (var parameter in Parameters)
                {
                    parameters.Add($"{parameter.Key}:{parameter.Value}");
                }
                return string.Join(';', parameters).ToMD5Hash();
            }

            return null;
        }
    }
}
