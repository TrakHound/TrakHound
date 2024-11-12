// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace TrakHound.Functions
{
    public struct TrakHoundFunctionResponse
    {
        private readonly string _id = Guid.NewGuid().ToString();
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();


        public string Id { get; set; }

        public bool Success => StatusCode >= 200 && StatusCode < 300;

        public string PackageId { get; set; }

        public string PackageVersion { get; set; }

        public string EngineId { get; set; }

        public int StatusCode { get; set; }

        public long Started { get; set; }

        public long Completed { get; set; }

        public IReadOnlyDictionary<string, string> Parameters => _parameters;


        public TrakHoundFunctionResponse()
        {
            StatusCode = 0;
        }

        public TrakHoundFunctionResponse(int statusCode)
        {
            StatusCode = statusCode;
        }

        public TrakHoundFunctionResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            AddParameter("message", message);
        }

        public TrakHoundFunctionResponse(int statusCode, object obj)
        {
            StatusCode = statusCode;
            AddObjectParameter("object", obj);
        }

        public TrakHoundFunctionResponse(int statusCode, byte[] content, string contentType)
        {
            StatusCode = statusCode;
            AddParameter("contentType", contentType);
            if (content != null) AddParameter("content", Convert.ToBase64String(content));
        }


        public bool ParameterExists(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return _parameters.ContainsKey(name);
            }

            return false;
        }

        public string GetParameter(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return _parameters.GetValueOrDefault(name);
            }

            return null;
        }

        public T GetParameter<T>(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var value = _parameters.GetValueOrDefault(name);
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

        public void AddParameter(string name, string value)
        {
            if (!string.IsNullOrEmpty(name) && value != null)
            {
                _parameters.Remove(name);
                _parameters.Add(name, value);
            }
        }

        public void AddObjectParameter(string name, object obj)
        {
            if (!string.IsNullOrEmpty(name) && obj != null)
            {
                var objType = obj.GetType();

                var objProperties = objType.GetProperties();
                if (!objProperties.IsNullOrEmpty())
                {
                    foreach (var property in objProperties)
                    {
                        var propertyValue = property.GetValue(obj);
                        if (propertyValue != null)
                        {
                            var propertyName = $"{name}.{property.Name.ToCamelCase()}";
                            var propertyType = propertyValue.GetType();

                            if (propertyType.IsClass && propertyType != typeof(string))
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                                {
                                    var arrayIndex = 0;

                                    IEnumerable enumerablePropertyValues = (IEnumerable)propertyValue;
                                    foreach (var enumerablePropertyValue in enumerablePropertyValues)
                                    {
                                        var enumerablePropertyName = $"{propertyName}[{arrayIndex}]";
                                        var enumerablePropertyType = enumerablePropertyValue.GetType();

                                        if (enumerablePropertyType.IsClass && enumerablePropertyType != typeof(string))
                                        {
                                            AddObjectParameter(enumerablePropertyName, enumerablePropertyValue);
                                        }
                                        else
                                        {
                                            AddParameter(enumerablePropertyName, enumerablePropertyValue.ToString());
                                        }

                                        arrayIndex++;
                                    }
                                }
                                else
                                {
                                    AddObjectParameter(propertyName, propertyValue);
                                }  
                            }
                            else
                            {
                                AddParameter(propertyName, propertyValue.ToString());
                            }
                        }
                    }
                }
            }
        }

        public void AddJsonParameter(string name, string json)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(json))
            {
                var objectResults = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (objectResults != null)
                {
                    foreach (var key in objectResults.Keys)
                    {
                        var element = (JsonElement)objectResults[key];

                        ProcessJson(key, element);
                    }
                }
            }
        }

        private void ProcessJson(string key, JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String: AddParameter(key, element.ToString()); break;

                case JsonValueKind.Number: AddParameter(key, element.ToString()); break;

                case JsonValueKind.Array:

                    var arrayIndex = 0;
                    foreach (var entry in element.EnumerateArray())
                    {
                        switch (entry.ValueKind)
                        {
                            case JsonValueKind.String:
                                AddParameter($"{key}[{arrayIndex}]", entry.ToString());
                                break;

                            case JsonValueKind.Number:
                                AddParameter($"{key}[{arrayIndex}]", entry.ToString());
                                break;

                            case JsonValueKind.Object:

                                //var keyProperty = entry.EnumerateObject().FirstOrDefault(o => o.Value.ValueKind == JsonValueKind.String || o.Value.ValueKind == JsonValueKind.Number);
                                //ProcessJson(path, keyProperty.Value.ToString(), entry);
                                break;
                        }

                        arrayIndex++;
                    }
                    break;

                case JsonValueKind.Object:

                    foreach (var property in element.EnumerateObject())
                    {
                        var propertyKey = $"{key}.{property.Name.ToCamelCase()}";
                        ProcessJson(propertyKey, property.Value);
                    }
                    break;
            }
        }


        public string ToJson(bool indent = true)
        {
            //if (!_parameters.IsNullOrEmpty())
            //{
            //    var jsonOptions = new JsonWriterOptions();
            //    jsonOptions.Indented = indent;

            //    using var stream = new MemoryStream();
            //    using var jsonWriter = new Utf8JsonWriter(stream, jsonOptions);

            //    jsonWriter.WriteStartObject();

            //    foreach (var parameter in _parameters)
            //    {
            //        if (parameter.Key.StartsWith("object"))
            //        {
            //            var propertyName = parameter.Key.Substring("object".Length + 1);
            //            propertyName = GetPropertyName(propertyName);

            //            jsonWriter.WriteString(propertyName, parameter.Value);
            //        }
            //    }

            //    jsonWriter.WriteEndObject();
            //    jsonWriter.Flush();
            //    return Encoding.UTF8.GetString(stream.ToArray());
            //}

            return Json.Convert(_parameters, indented: indent);
        }

        //struct JsonPropertyModel
        //{
        //    public string Name { get; set; }


        
        //}


        private static string GetPropertyName(string key)
        {
            var i = key.IndexOf('.');
            if (i > 0)
            {
                return key.Substring(0, i);
            }

            return key;
        }
    }
}
