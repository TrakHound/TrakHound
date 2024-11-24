// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TrakHound.Entities;

namespace TrakHound
{
    public static class TrakHoundPath
    {
        public const char PathSeparator = '/';
        private const string _namespaceDelimiter = ":";
        private const string _uuidPrefix = "uuid=";
        private const string _uuidAbsolutePrefix = "/uuid=";


        public static string Combine(params string[] paths)
		{
			if (!paths.IsNullOrEmpty())
			{
				var p = new List<string>();

                for (var i = 0; i < paths.Length; i++)
                {
                    var path = paths[i];
                    if (!string.IsNullOrEmpty(path))
                    {
						if (i == 0) p.Add(path.TrimEnd(PathSeparator));
						else p.Add(path.Trim(PathSeparator));
					}
                }

				return string.Join(PathSeparator, p).TrimEnd(PathSeparator);
			}

			return null;
		}

        public static string SetNamespace(string ns, string path)
        {
            if (!string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(path))
            {
                var partialPath = GetPartialPath(path)?.TrimStart(PathSeparator);

                // [NAMESPACE]:/[PATH]
                return $"{ns}{_namespaceDelimiter}{PathSeparator}{partialPath}";
            }

            return null;
        }

        public static string GetUuid(string absolutePath)
        {
            var bytes = GetUuidBytes(absolutePath);
            return StringFunctions.ToSHA256HashString(bytes);
        }

        public static byte[] GetUuidBytes(string absolutePath)
        {
            var ns = GetNamespace(absolutePath);
            if (string.IsNullOrEmpty(ns)) ns = TrakHoundNamespace.DefaultNamespace;

            var partialPath = GetPartialPath(absolutePath);

            return GetUuidBytes(ns, partialPath);
        }

        public static string GetUuid(string ns, string partialPath)
        {
            if (string.IsNullOrEmpty(ns)) ns = TrakHoundNamespace.DefaultNamespace;
            var bytes = GetUuidBytes(ns, partialPath);
            return StringFunctions.ToSHA256HashString(bytes);
        }

        public static byte[] GetUuidBytes(string ns, string partialPath)
        {
            if (!string.IsNullOrEmpty(partialPath))
            {
                byte[] uuid = null;
                byte[] parentUuid = null;

                var parts = partialPath.Split('/');
                foreach (var part in parts)
                {
                    if (part.StartsWith(_uuidPrefix))
                    {
                        uuid = StringFunctions.ConvertHexidecimalToBytes(part.Substring(_uuidPrefix.Length));
                    }
                    else
                    {
                        uuid = TrakHoundObjectEntity.GenerateUuidBytes(ns, part, parentUuid);
                    }

                    parentUuid = uuid;
                }

                return uuid;
            }

            return null;
        }

        public static string[] GetParentUuids(string absolutePath)
        {
            if (!string.IsNullOrEmpty(absolutePath) && TrakHoundPath.GetType(absolutePath) == TrakHoundPathType.Absolute)
            {
                var parts = absolutePath.Split('/');
                var uuids = new string[parts.Length];
                string uuid = null;
                string parentUuid = null;

                for (var i = 0; i < parts.Length; i++)
                {
                    uuid = TrakHoundObjectEntity.GenerateUuid(parts[i], parentUuid);
                    if (uuid != null) uuids[i] = uuid;
                    parentUuid = uuid;
                }

                return uuids;
            }

            return null;
        }

        public static string GetNamespace(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var i = path.IndexOf(_namespaceDelimiter);
                if (i >= 0)
                {
                    return path.Substring(0, i);
                }
            }

            return null;
        }

        public static string GetPartialPath(string absolutePath)
        {
            if (!string.IsNullOrEmpty(absolutePath))
            {
                var i = absolutePath.IndexOf(_namespaceDelimiter);
                if (i >= 0)
                {
                    return absolutePath.Substring(i + _namespaceDelimiter.Length);
                }
                else
                {
                    return absolutePath;
                }
            }

            return null;
        }

        public static TrakHoundPathType GetType(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                //if (path.Contains("*")) return TrakHoundPathType.Expression;
                //if (path == "/") return TrakHoundPathType.Expression;
                //if (!path.StartsWith("/") && !path.StartsWith(_uuidPrefix)) return TrakHoundPathType.Expression;
                //if (path.Contains("..")) return TrakHoundPathType.Expression;
                //if (path.Contains("content-type=")) return TrakHoundPathType.Expression;
                //if (path.Contains("type=")) return TrakHoundPathType.Expression;
                //if (path.Contains("meta@")) return TrakHoundPathType.Expression;

                var ns = GetNamespace(path);
                var partialPath = GetPartialPath(path);

                if (!string.IsNullOrEmpty(partialPath))
                {
                    if (string.IsNullOrEmpty(ns) && !partialPath.StartsWith(_uuidPrefix)) return TrakHoundPathType.Expression;
                    if (partialPath.Contains("*")) return TrakHoundPathType.Expression;
                    if (partialPath == "/") return TrakHoundPathType.Expression;
                    if (!partialPath.StartsWith("/") && !partialPath.StartsWith(_uuidPrefix)) return TrakHoundPathType.Expression;
                    if (partialPath.Contains("..")) return TrakHoundPathType.Expression;
                    if (partialPath.Contains("content-type=")) return TrakHoundPathType.Expression;
                    if (partialPath.Contains("type=")) return TrakHoundPathType.Expression;
                    if (partialPath.Contains("meta@")) return TrakHoundPathType.Expression;
                }
            }

            return TrakHoundPathType.Absolute;
        }

        public static bool IsAbsolute(string path) => GetType(path) == TrakHoundPathType.Absolute;

        public static bool IsExpression(string path) => GetType(path) == TrakHoundPathType.Expression;

        public static string ToRoot(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return !path.StartsWith('/') ? $"/{path}" : path;
            }

            return null;
        }

		public static bool IsRoot(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return !path.Contains(PathSeparator);
            }

            return false;
        }

        public static bool IsChildOf(string parentPath, string path)
        {
            if (!string.IsNullOrEmpty(parentPath) && !string.IsNullOrEmpty(path))
            {
                var parentPaths = GetPaths(parentPath).ToArray();
                var paths = GetPaths(path).ToArray();

                if (parentPaths.Count() < paths.Count())
                {
                    for (var i = 0; i < parentPaths.Count(); i++)
                    {
                        var parent = parentPaths[i];
                        var target = paths[i];

                        if (target != parent) return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public static string GetRoot(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(PathSeparator))
                {
                    var i = path.IndexOf(PathSeparator);
                    if (i > 0 && i < path.Length - 1)
                    {
                        return path.Substring(0, i);
                    }
                }
            }

            return path;
        }

        public static string GetParentPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(PathSeparator))
                {
                    var i = path.LastIndexOf(PathSeparator);
                    if (i > 0 && i < path.Length - 1)
                    {
                        return path.Substring(0, i);
                    }
                }
            }

            return null;
        }

        public static string GetParentPath(string path, int index)
        {
            if (!string.IsNullOrEmpty(path) && index > -1)
            {
                var parts = GetPaths(path).ToArray();
                if (index < parts.Length - 1)
                {
                    return parts[index];
                }
            }

            return null;
        }

        public static string GetObject(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(PathSeparator))
                {
                    var i = path.LastIndexOf(PathSeparator);
                    if (i > 0 && i < path.Length - 1)
                    {
                        return path.Substring(i + 1).Trim('/');
                    }
                }

                return path.Trim('/');
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

        public static string GetTruncatedPath(IEnumerable<string> paths, string targetPath)
        {
            if (!paths.IsNullOrEmpty() && !string.IsNullOrEmpty(targetPath))
            {
                var pathPairs = new Dictionary<string, string[]>();
                var dPaths = paths.Where(o => !string.IsNullOrEmpty(o))?.Distinct();

                if (!dPaths.IsNullOrEmpty())
                {
                    foreach (var path in dPaths)
                    {
                        // Get a list of the Part Parts
                        var parts = path.Split(PathSeparator);
                        if (!parts.IsNullOrEmpty()) pathPairs.Add(path, parts.ToArray());
                    }

                    if (pathPairs.ContainsKey(targetPath))
                    {
                        var targetParts = pathPairs[targetPath];
                        var otherPairs = pathPairs.Where(o => o.Key != targetPath);

                        var truncatedPath = "";
                        var prevMatch = false;
                        var prevPart = "";

                        for (var i = 0; i < targetParts.Length; i++)
                        {
                            var targetPart = targetParts[i];
                            var match = true;

                            if (i < targetParts.Length - 1)
                            {
                                foreach (var otherPair in otherPairs)
                                {
                                    if (otherPair.Value.Length > i)
                                    {
                                        var otherPart = otherPair.Value[i];

                                        if (targetPart != otherPart)
                                        {
                                            if (prevMatch) truncatedPath += "/../";
                                            truncatedPath = Combine(truncatedPath, prevPart, targetPart);
                                            match = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Always add the last Part
                                truncatedPath = Combine(truncatedPath, targetPart);
                            }

                            prevPart = targetPart;
                            prevMatch = match;
                        }

                        return truncatedPath;
                    }
                }
            }

            return targetPath;
        }

        public static IEnumerable<string> GetPaths(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var paths = new List<string>();

                var parts = path.Split(PathSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                if (!parts.IsNullOrEmpty())
                {
                    for (var i = 0; i < parts.Length; i++)
                    {
                        // Add Parent Paths
                        var partpaths = new List<string>();
                        for (var j = 0; j <= i; j++) partpaths.Add(parts[j]);

                        paths.Add(ToRoot(string.Join(PathSeparator, partpaths)));
                    }
                }

                return paths;
            }

            return null;
        }

        public static string GetPathParameter(string path, string pattern, string parameterName)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(parameterName))
            {
                var pathParts = path.Trim('/').Split('/');
                var patternParts = pattern.Trim('/').Split('/');

                if (pathParts != null && patternParts != null && pathParts.Length >= patternParts.Length)
                {
                    for (var i = 0; i < pathParts.Length; i++)
                    {
                        if (i < patternParts.Length)
                        {
                            var pathPart = pathParts[i];
                            var patternPart = patternParts[i];

                            // Check for Slug Match
                            var slugRegex = new Regex(@"(\{\*(.*?)\})");
                            if (slugRegex.IsMatch(patternPart))
                            {
                                var match = slugRegex.Match(patternPart);
                                if (match != null)
                                {
                                    var matchText = match.Groups[1].ToString();
                                    var pathParameterName = match.Groups[2].ToString();

                                    if (pathParameterName == parameterName)
                                    {
                                        var slugParts = new List<string>();
                                        for (var j = i; j < pathParts.Length; j++) slugParts.Add(pathParts[j]);

                                        return string.Join('/', slugParts);
                                    }
                                }
                            }

                            // Check for Exact Match
                            var exactRegex = new Regex(@"(\{(.*?)\})");
                            if (exactRegex.IsMatch(patternPart))
                            {
                                var match = exactRegex.Match(patternPart);
                                if (match != null)
                                {
                                    var matchText = match.Groups[1].ToString();
                                    var pathParameterName = match.Groups[2].ToString();

                                    if (pathParameterName == parameterName) return pathPart;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static string SetPathParameter(string path, string pattern, string parameterName, string parameterValue)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(parameterName))
            {
                var pathParts = path.Split('/');
                var patternParts = pattern.Split('/');

                if (pathParts != null && patternParts != null && pathParts.Length >= patternParts.Length)
                {
                    for (var i = 0; i < pathParts.Length; i++)
                    {
                        if (i < patternParts.Length)
                        {
                            var pathPart = pathParts[i];
                            var patternPart = patternParts[i];
                            if (!string.IsNullOrEmpty(patternPart) && !string.IsNullOrEmpty(parameterValue))
                            {
                                var regexPattern = "({" + parameterName + "})";

                                var regex = new Regex(regexPattern);
                                if (regex.IsMatch(patternPart))
                                {
                                    pathParts[i] = regex.Replace(patternPart, parameterValue);
                                }
                            }
                        }
                    }
                }

                return string.Join('/', pathParts);
            }

            return path;
        }
    }
}
