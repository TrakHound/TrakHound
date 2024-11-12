// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Net.Http;
using System.Net.Http.Headers;

namespace TrakHound.Http
{
    public struct HttpResponse
    {
        public int StatusCode { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public HttpResponseHeaders Headers { get; set; }


        public HttpResponse(HttpResponseMessage responseMessage, byte[] content)
        {
            StatusCode = (int)responseMessage.StatusCode;
            ContentType = responseMessage.Content.Headers.ContentType?.MediaType;
            Content = content;
            Headers = responseMessage.Headers;
        }

        public HttpResponse(int statusCode, string contentType, byte[] content)
        {
            StatusCode = statusCode;
            ContentType = contentType;
            Content = content;
            Headers = null;
        }
    }
}
