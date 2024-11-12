// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;

namespace TrakHound.Api
{
    public class TrakHoundApiSchemaInformation
    {
        public string Name { get; set; }

        public IEnumerable<TrakHoundApiSchemaPropertyInformation> Properties { get; set; }


        public static TrakHoundApiSchemaInformation Create(Type objectType)
        {
            if (objectType != null)
            {
                var information = new TrakHoundApiSchemaInformation();
                information.Name = objectType.Name;

                //if (typeof(IEnumerable).IsAssignableFrom(objectType))
                //{
                //    var types = objectType.GetGenericArguments();
                //    if (!types.IsNullOrEmpty())
                //    {

                //    }
                //}
                //else
                //{

                //}

                // Add Properties
                var objProperties = objectType.GetProperties();
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

                return information;
            }

            return null;
        }
    }
}
