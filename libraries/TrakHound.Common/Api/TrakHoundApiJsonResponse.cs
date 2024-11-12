// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TrakHound.Api
{
    public class TrakHoundApiJsonResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }


        public TrakHoundApiJsonResponse() { }

        public TrakHoundApiJsonResponse(TrakHoundApiResponse response)
        {
            StatusCode = response.StatusCode;
            Path = response.Path;
            ContentType = response.ContentType;
            Parameters = response.Parameters;
            Content = response.GetContentBase64String();
        }

        public TrakHoundApiResponse ToResponse()
        {
            var response = new TrakHoundApiResponse();
            response.StatusCode = StatusCode;
            response.Path = Path;
            response.ContentType = ContentType;
            response.Parameters = Parameters;
            response.Content = TrakHoundApiResponse.GetContentStreamFromBase64String(Content);
            return response;
        }


        public static TrakHoundApiResponse Ok(object content, string path = null, bool indentOutput = true)
        {
            var json = Json.Convert(content, indented: indentOutput);
            if (!string.IsNullOrEmpty(json))
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                return new TrakHoundApiResponse(200, bytes, "application/json", path);
            }

            return new TrakHoundApiResponse(200, path);
        }
    }
}
