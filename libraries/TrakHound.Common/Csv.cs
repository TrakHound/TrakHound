// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound
{
    /// <summary>
    /// Handles functions related to Reading/Writing of CSV files
    /// </summary>
    public static class Csv
    {
        /// <summary>
        /// Converts an object to a CSV line
        /// </summary>
        public static string ToCsv(object obj)
        {
            if (obj != null)
            {
                var l = new List<object>();

                // Read each property of the object and add to list
                foreach (var property in obj.GetType().GetProperties())
                {
                    try
                    {
                        var val = property.GetValue(obj, null);

                        if (property.PropertyType == typeof(DateTime))
                        {
                            val = ((DateTime)val).ToUnixTime();
                        }

                        l.Add(val != null ? val : "");
                    }
                    catch (Exception) { }
                }

                // Convert the list of strings to a CSV line and Return
                return string.Join(",", l);
            }

            return null;
        }

        public static string ToCsv<T>(object obj)
        {
            if (obj != null)
            {
                var l = new List<string>();

                // Read each property of the type and add to list
                foreach (var property in typeof(T).GetProperties())
                {
                    try
                    {
                        var type = obj.GetType();
                        var objectProperty = type.GetProperty(property.Name);
                        if (objectProperty != null)
                        {
                            var val = objectProperty.GetValue(obj, null);

                            if (property.PropertyType == typeof(DateTime))
                            {
                                val = ((DateTime)val).ToUnixTime();
                            }

                            l.Add(val != null ? val.ToString() : "");
                        }
                        else
                        {
                            l.Add("");
                        }


                        //var val = property.GetValue(), null);

                        //if (property.PropertyType == typeof(DateTime))
                        //{
                        //    val = ((DateTime)val).ToUnixTime();
                        //}

                        //l.Add(val != null ? val.ToString() : "");
                    }
                    catch (Exception) { }
                }

                // Convert the list of strings to a CSV line and Return
                return string.Join(",", l);
            }

            return null;
        }

        /// <summary>
        /// Read an object from a CSV line/>
        /// </summary>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <param name="line">the CSV line to parse</param>
        public static T FromCsv<T>(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                try
                {
                    // Create an array representing each field in the CSV line
                    var fields = line.Split(',');

                    // Get list of properties for the Type "T"
                    var properties = typeof(T).GetProperties();

                    // Make sure that the array matches the number of properties
                    if (fields.Length >= properties.Length)
                    {
                        // Create a new instance of the object of type "T"
                        var obj = (T)Activator.CreateInstance(typeof(T));

                        // Loop through the properties and set values based on the corresponding CSV field
                        for (int i = 0; i < properties.Length; i++)
                        {
                            var p = properties[i];
                            if (p.CanWrite)
                            {
                                var field = fields[i];

                                if (p.PropertyType == typeof(DateTime))
                                {
                                    if (long.TryParse(field, out long unixMs))
                                    {
                                        p.SetValue(obj, UnixTimeExtensions.FromUnixTime(unixMs), null);
                                    }
                                    else
                                    {
                                        if (DateTime.TryParse(field, out DateTime ts))
                                        {
                                            p.SetValue(obj, ts, null);
                                        }
                                    }
                                }
                                else
                                {
                                    p.SetValue(obj, Convert.ChangeType(fields[i], p.PropertyType), null);
                                }
                            }
                        }

                        return obj;
                    }
                }
                catch
                {

                }
            }

            return default(T);
        }
    }
}
