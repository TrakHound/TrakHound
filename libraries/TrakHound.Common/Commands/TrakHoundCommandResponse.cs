// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace TrakHound.Commands
{
    public struct TrakHoundCommandResponse
    {
        public bool Success => StatusCode >= 200 && StatusCode < 300;

        public int StatusCode { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public Dictionary<string, string> Parameters { get; set; }


        public TrakHoundCommandResponse()
        {
            StatusCode = 0;
            ContentType = null;
            Content = null;
        }


        public bool ParameterExists(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Parameters.IsNullOrEmpty())
            {
                return Parameters.ContainsKey(name);
            }

            return false;
        }

        public string GetParameter(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Parameters.IsNullOrEmpty())
            {
                return Parameters.GetValueOrDefault(name);
            }

            return null;
        }

        public T GetParameter<T>(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Parameters.IsNullOrEmpty())
            {
                var value = Parameters.GetValueOrDefault(name);
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


        public string GetContentUtf8String()
        {
            if (Content != null)
            {
                try
                {
                    return Encoding.UTF8.GetString(Content);
                }
                catch { }
            }

            return null;
        }

        public TOutput GetJsonContentObject<TOutput>()
        {
            return Json.Convert<TOutput>(GetContentUtf8String());
        }
    }
}
