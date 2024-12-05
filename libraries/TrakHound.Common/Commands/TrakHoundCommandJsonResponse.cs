// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TrakHound.Commands
{
    public class TrakHoundCommandJsonResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }


        public TrakHoundCommandJsonResponse() { }

        public TrakHoundCommandJsonResponse(TrakHoundCommandResponse response)
        {
            StatusCode = response.StatusCode;
            ContentType = response.ContentType;
            Parameters = response.Parameters;
            Content = response.GetContentBase64String();
        }

        public TrakHoundCommandResponse ToResponse()
        {
            var response = new TrakHoundCommandResponse();
            response.StatusCode = StatusCode;
            response.ContentType = ContentType;
            response.Parameters = Parameters;
            response.Content = TrakHoundCommandResponse.GetContentStreamFromBase64String(Content);
            return response;
        }


        public static TrakHoundCommandResponse Ok(object content, bool indentOutput = true)
        {
            var json = Json.Convert(content, indented: indentOutput);
            if (!string.IsNullOrEmpty(json))
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                return new TrakHoundCommandResponse(200, bytes, "application/json");
            }

            return new TrakHoundCommandResponse(200);
        }
    }
}
