// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TrakHound.Api
{
    public struct TrakHoundApiResponse
    {
        public bool Success { get; set; }

        public int StatusCode { get; set; }

        //public string Path { get; set; }

        public string ContentType { get; set; }

        public Stream Content { get; set; }

        public Dictionary<string, string> Parameters { get; set; }


        public TrakHoundApiResponse(int statusCode, Dictionary<string, string> parameters = null)
        {
            StatusCode = statusCode;
            Parameters = parameters;
            Success = statusCode >= 200 && statusCode < 400;
        }

        public TrakHoundApiResponse(int statusCode, byte[] content, string contentType = null, Dictionary<string, string> parameters = null)
        {
            StatusCode = statusCode;
            ContentType = contentType;
            Parameters = parameters;
            Success = statusCode >= 200 && statusCode < 400;

            if (content != null)
            {
                try
                {
                    Content = new MemoryStream(content);
                }
                catch { }
            }
        }

        public TrakHoundApiResponse(int statusCode, Stream content, string contentType = null, Dictionary<string, string> parameters = null)
        {
            StatusCode = statusCode;
            Content = content;
            ContentType = contentType;
            Parameters = parameters;
            Success = statusCode >= 200 && statusCode < 400;
        }

        //public TrakHoundApiResponse(int statusCode, string path = null, Dictionary<string, string> parameters = null)
        //{
        //    StatusCode = statusCode;
        //    //Path = path;
        //    Parameters = parameters;
        //    Success = statusCode >= 200 && statusCode < 400;
        //}

        //public TrakHoundApiResponse(int statusCode, byte[] content, string contentType = null, string path = null, Dictionary<string, string> parameters = null)
        //{
        //    StatusCode = statusCode;
        //    ContentType = contentType;
        //    //Path = path;
        //    Parameters = parameters;
        //    Success = statusCode >= 200 && statusCode < 400;

        //    if (content != null)
        //    {
        //        try
        //        {
        //            Content = new MemoryStream(content);
        //        }
        //        catch { }
        //    }
        //}

        //public TrakHoundApiResponse(int statusCode, Stream content, string contentType = null, string path = null, Dictionary<string, string> parameters = null)
        //{
        //    StatusCode = statusCode;
        //    Content = content;
        //    ContentType = contentType;
        //    //Path = path;
        //    Parameters = parameters;
        //    Success = statusCode >= 200 && statusCode < 400;
        //}


        public bool IsValid() => StatusCode != 0;


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


        public byte[] GetContentBytes()
        {
            if (Content != null)
            {
                return GetContentBytes(Content);
            }

            return null;
        }

        public string GetContentUtf8String()
        {
            var contentBytes = GetContentBytes();
            if (contentBytes != null)
            {
                try
                {
                    return Encoding.UTF8.GetString(contentBytes);
                }
                catch { }
            }

            return null;
        }

        public string GetContentBase64String()
        {
            var contentBytes = GetContentBytes();
            if (contentBytes != null)
            {
                try
                {
                    return Convert.ToBase64String(contentBytes);
                }
                catch { }
            }

            return null;
        }

        public TOutput GetJsonContentObject<TOutput>()
        {
            return Json.Convert<TOutput>(GetContentUtf8String());
        }


        public static Stream GetJsonContentStream(object contentObject)
        {
            if (contentObject != null)
            {
                try
                {
                    var json = Json.Convert(contentObject);
                    var contentBytes = Encoding.UTF8.GetBytes(json);
                    return GetContentStream(contentBytes);
                }
                catch { }
            }

            return null;
        }


        public static Stream GetContentStream(byte[] contentBytes)
        {
            if (contentBytes != null)
            {
                try
                {
                    return new MemoryStream(contentBytes);
                }
                catch { }
            }

            return null;
        }

        public static byte[] GetContentBytes(Stream contentStream)
        {
            if (contentStream != null)
            {
                try
                {
                    using (var readStream = new MemoryStream())
                    {
                        contentStream.CopyTo(readStream);
                        readStream.Seek(0, SeekOrigin.Begin);
                        
                        return readStream.ToArray();
                    }
                }
                catch { }
                finally
                {
                    contentStream.Dispose();
                }
            }

            return null;
        }

        //public static byte[] GetContentBytes(Stream contentStream)
        //{
        //    if (contentStream != null)
        //    {
        //        try
        //        {
        //            using (var readStream = new MemoryStream())
        //            {
        //                contentStream.CopyTo(readStream);
        //                readStream.Seek(0, SeekOrigin.Begin);
        //                return readStream.ToArray();
        //            }
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        public static Stream GetContentStreamFromBase64String(string base64Content)
        {
            if (!string.IsNullOrEmpty(base64Content))
            {
                try
                {
                    var contentBytes = Convert.FromBase64String(base64Content);
                    return GetContentStream(contentBytes);
                }
                catch { }
            }

            return null;
        }
    }
}
