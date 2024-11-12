// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace TrakHound.Api
{
    public class TrakHoundApiResponseInformation
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("schema")]
        public TrakHoundApiSchemaInformation Schema { get; set; }


        public static TrakHoundApiResponseInformation Create(int statusCode, Type objectType)
        {
            if (objectType != null)
            {
                var responseInformation = new TrakHoundApiResponseInformation();
                responseInformation.StatusCode = statusCode;

                if (objectType == typeof(byte[]))
                {
                    responseInformation.ContentType = "application/octet-stream";
                }
                else if (!objectType.IsPrimitive && objectType != typeof(string) && objectType != typeof(DateTime))
                {
                    responseInformation.ContentType = "application/json";
                    responseInformation.Schema = TrakHoundApiSchemaInformation.Create(objectType);
                }
                else
                {
                    responseInformation.ContentType = "text/plain";
                }

                return responseInformation;
            }

            return null;
        }
    }
}
