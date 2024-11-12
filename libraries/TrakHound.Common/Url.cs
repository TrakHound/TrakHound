// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace TrakHound
{
    public static class Url
    {
        public const char PathSeparator = '/';


        public static string Combine(params string[] paths)
        {
            if (!paths.IsNullOrEmpty())
            {
                var p = new List<string>();
                var pathSeparator = PathSeparator.ToString();

                for (var i = 0; i < paths.Length; i++)
                {
                    var path = paths[i];
                    if (!string.IsNullOrEmpty(path) && path != pathSeparator)
                    {
                        if (i == 0) p.Add(path.TrimStart(PathSeparator).TrimEnd(PathSeparator));
                        else p.Add(path.Trim(PathSeparator));
                    }
                }

                return string.Join(PathSeparator, p).TrimStart(PathSeparator).TrimEnd(PathSeparator);
            }

            return null;
        }

        public static string GetRelativeTo(string relativePath, string path)
        {
            if (!string.IsNullOrEmpty(relativePath) && !string.IsNullOrEmpty(path))
            {
                var result = System.IO.Path.GetRelativePath(relativePath, path);
                return result?.Replace('\\', '/');
            }

            return path;
        }

        public static string Encode(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                return HttpUtility.UrlEncode(url);
            }

            return null;
        }

        public static string Decode(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                return HttpUtility.UrlDecode(url);
            }

            return null;
        }

        public static string AddQueryParameter(string url, string parameterName, object parameterValue)
        {
            if (!string.IsNullOrEmpty(parameterName) && parameterValue != null)
            {
                var lUrl = !string.IsNullOrEmpty(url) ? url : "";

                var valueStr = parameterValue.ToString();
                if (!string.IsNullOrEmpty(valueStr))
                {
                    string path;
                    string query = null;

                    var i = lUrl.IndexOf('?');
                    if (i > 0)
                    {
                        path = lUrl.Substring(0, i);
                        query = lUrl.Substring(i);
                    }
                    else
                    {
                        path = lUrl;
                    }

                    if (!string.IsNullOrEmpty(query))
                    {
                        query += '&' + parameterName + '=' + HttpUtility.UrlEncode(valueStr);
                    }
                    else
                    {
                        query = '?' + parameterName + '=' + HttpUtility.UrlEncode(valueStr);
                    }

                    return path + query;
                }
            }

            return url;
        }

        public static string AddQueryParameterJson(string url, string parameterName, object parameterValue)
        {
            if (parameterValue != null)
            {
                var json = Json.Convert(parameterValue);
                if (json != null)
                {
                    return AddQueryParameter(url, parameterName, json);
                }
            }

            return url;
        }

        public static string RemoveQueryParameters(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var i = url.IndexOf('?');
                if (i > 0)
                {
                    return url.Substring(0, i);
                }
                else
                {
                    return url;
                }
            }

            return null;
        }
       
        public static string GetFirstFragment(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                var s = uri.Trim('/');
                if (!string.IsNullOrEmpty(s))
                {
                    var i = s.IndexOf('/');
                    if (i >= 0)
                    {
                        return s.Substring(0, i);
                    }
                    else return s;
                }
            }

            return null;
        }

        public static string RemoveFirstFragment(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                var s = uri.Trim('/');
                if (!string.IsNullOrEmpty(s))
                {
                    var i = s.IndexOf('/');
                    if (i >= 0)
                    {
                        return s.Substring(i).TrimStart('/');
                    }
                }
            }

            return null;
        }

        public static string RemoveBase(string url, string baseUrl)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(baseUrl))
            {
                var s = url.Trim('/');
                if (!string.IsNullOrEmpty(s))
                {
                    if (s.StartsWith(baseUrl))
                    {
                        s = s.Substring(baseUrl.Length);
                        return s.TrimStart('/');
                    }
                }
            }

            return null;
        }


        public static string GetLastFragment(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                var i = uri.LastIndexOf('/');
                if (i >= 0)
                {
                    return uri.Substring(i).Trim('/');
                }
                else
                {
                    return uri;
                }
            }

            return null;
        }

        public static string RemoveLastFragment(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                var s = uri.Trim('/');
                if (!string.IsNullOrEmpty(s))
                {
                    var i = s.LastIndexOf('/');
                    if (i >= 0)
                    {
                        return s.Substring(0, i);
                    }
                    else
                    {
                        return uri;
                    }
                }
            }

            return null;
        }


        public static string GetRouteParameter(string url, string route, string parameterName)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(route) && !string.IsNullOrEmpty(parameterName))
            {
                var urlParts = url.Trim('/').Split('/');
                var routeParts = route.Trim('/').Split('/');

                if (urlParts != null && routeParts != null && urlParts.Length >= routeParts.Length)
                {
                    for (var i = 0; i < urlParts.Length; i++)
                    {
                        if (i < routeParts.Length)
                        {
                            var urlPart = urlParts[i];
                            var routePart = routeParts[i];

                            // Check for Slug Match
                            var slugRegex = new Regex(@"(\{\*(.*?)(?:\:.+)?\})");
                            if (slugRegex.IsMatch(routePart))
                            {
                                var match = slugRegex.Match(routePart);
                                if (match != null)
                                {
                                    var matchText = match.Groups[1].ToString();
                                    var routeParameterName = match.Groups[2].ToString();

                                    if (routeParameterName == parameterName)
                                    {
                                        var slugParts = new List<string>();
                                        for (var j = i; j < urlParts.Length; j++) slugParts.Add(urlParts[j]);

                                        return string.Join('/', slugParts);
                                    }
                                }
                            }

                            // Check for Exact Match
                            var exactRegex = new Regex(@"(\{(.*?)(?:\:.+)?\})");
                            if (exactRegex.IsMatch(routePart))
                            {
                                var match = exactRegex.Match(routePart);
                                if (match != null)
                                {
                                    var matchText = match.Groups[1].ToString();
                                    var routeParameterName = match.Groups[2].ToString();

                                    if (routeParameterName == parameterName) return urlPart;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static string AddRouteParameter(string url, string route, string parameterName, object parameterValue)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(route) && !string.IsNullOrEmpty(parameterName))
            {
                var urlParts = url.Split('/');
                var routeParts = route.Split('/');

                if (urlParts != null && routeParts != null && urlParts.Length >= routeParts.Length)
                {
                    var resultParts = new List<string>();

                    for (var i = 0; i < urlParts.Length; i++)
                    {
                        if (i < routeParts.Length)
                        {
                            var urlPart = urlParts[i];
                            var routePart = routeParts[i];
                            var resultPart = urlPart;

                            // Check for Slug Match
                            var slugRegex = new Regex(@"(\{\*(.*?)\})");
                            if (slugRegex.IsMatch(routePart))
                            {
                                var match = slugRegex.Match(routePart);
                                if (match != null)
                                {
                                    var matchText = match.Groups[1].ToString();
                                    var routeParameterName = match.Groups[2].ToString().TrimStart('*');

                                    if (routeParameterName == parameterName)
                                    {
                                        resultPart = parameterValue?.ToString();
                                    }
                                }
                            }

                            // Check for Exact Match
                            var exactRegex = new Regex(@"(\{(.*?)\})");
                            if (exactRegex.IsMatch(routePart))
                            {
                                var match = exactRegex.Match(routePart);
                                if (match != null)
                                {
                                    var matchText = match.Groups[1].ToString();
                                    var routeParameterName = match.Groups[2].ToString();

                                    if (routeParameterName == parameterName)
                                    {
                                        resultPart = parameterValue?.ToString();
                                    }
                                }
                            }

                            resultParts.Add(resultPart);
                        }
                    }

                    return string.Join('/', resultParts);
                }
            }

            return null;
        }

        public static string GetQueryParameter(string url, string parameterName)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(parameterName))
            {
                try
                {
                    var i = url.IndexOf('?');
                    if (i >= 0)
                    {
                        var query = url.Substring(i);
                        var parameters = HttpUtility.ParseQueryString(query);
                        return parameters.Get(parameterName);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
