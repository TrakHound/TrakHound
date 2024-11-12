// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TrakHound.Api
{
    public class TrakHoundApiSchemaPropertyInformation
    {
        public string Name { get; set; }

        public string DataType { get; set; }

        public string Description { get; set; }

        public IEnumerable<TrakHoundApiSchemaPropertyInformation> Properties { get; set; }


        public static TrakHoundApiSchemaPropertyInformation Create(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
                var information = new TrakHoundApiSchemaPropertyInformation();
                information.Name = propertyInfo.Name;

                var description = propertyInfo.GetXmlDocumentation();
                if (!string.IsNullOrEmpty(description)) information.Description = description;

                if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(DateTime))
                {
                    information.DataType = TrakHoundApiSchemaDataType.Get(propertyInfo.PropertyType);
                }
                else
                {
                    information.DataType = "Object";

                    // Add Properties
                    var objProperties = propertyInfo.PropertyType.GetProperties();
                    if (!objProperties.IsNullOrEmpty())
                    {
                        var properties = new List<TrakHoundApiSchemaPropertyInformation>();

                        foreach (var objProperty in objProperties)
                        {
                            var property = TrakHoundApiSchemaPropertyInformation.Create(objProperty);
                            if (property != null) properties.Add(property);
                        }

                        information.Properties = properties;
                    }
                }

                return information;
            }

            return null;
        }
    }
}
