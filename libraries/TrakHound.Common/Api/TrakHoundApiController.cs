// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Packages;
using TrakHound.Requests;
using TrakHound.Volumes;

namespace TrakHound.Api
{
    public abstract class TrakHoundApiController : ITrakHoundApiController
    {
        private const string _filenameParameterKey = "Filename";
        private const string _locationParameterKey = "Location";

        internal ITrakHoundApiConfiguration _configuration;
        internal TrakHoundPackage _package;
        internal ITrakHoundClient _client;
        internal ITrakHoundVolume _volume;
        internal ITrakHoundLogger _logger;

        public string Id { get; set; }

        public string InstanceId { get; set; }

        public string BaseUrl { get; set; }

        public string BasePath { get; set; }

        public string BaseLocation { get; set; }

        public TrakHoundSourceChain SourceChain { get; set; }

        public TrakHoundSourceEntry Source { get; set; }

        public ITrakHoundApiConfiguration Configuration => _configuration;

        public TrakHoundPackage Package => _package;

        public ITrakHoundClient Client => _client;

        public ITrakHoundVolume Volume => _volume;

        public ITrakHoundLogger Logger => _logger;


        public void SetConfiguration(ITrakHoundApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetPackage(TrakHoundPackage package)
        {
            _package = package;
            AfterPackageSet();
        }

        public void SetClient(ITrakHoundClient client)
        {
            _client = client;
            AfterClientSet();
        }

        public void SetVolume(ITrakHoundVolume volume)
        {
            _volume = volume;
            AfterVolumeSet();
        }

        public void SetLogger(ITrakHoundLogger logger)
        {
            _logger = logger;
            AfterLoggerSet();
        }


        protected string GetUrl(string destination)
        {
            return Url.Combine(BaseUrl, destination);
        }

        protected string GetPath(string destination)
        {
            return Url.Combine(BasePath, destination);
        }

        protected string GetLocation(string destination)
        {
            return Url.Combine(BaseLocation, destination);
        }


        protected virtual void AfterPackageSet() { }

        protected virtual void AfterClientSet() { }

        protected virtual void AfterVolumeSet() { }

        protected virtual void AfterLoggerSet() { }


        public static TrakHoundApiResponse Ok() => new TrakHoundApiResponse(200);
        public static TrakHoundApiResponse Ok(string message) => new TrakHoundApiResponse(200, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundApiResponse Ok(object response) => new TrakHoundApiResponse(200, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundApiResponse Ok(byte[] content, string contentType) => new TrakHoundApiResponse(200, content, contentType);


        public static TrakHoundApiResponse Created() => new TrakHoundApiResponse(201);
        public static TrakHoundApiResponse Created(string message) => new TrakHoundApiResponse(201, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundApiResponse Created(object response) => new TrakHoundApiResponse(201, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundApiResponse Created(byte[] content, string contentType) => new TrakHoundApiResponse(201, content, contentType);


        public static TrakHoundApiResponse Accepted() => new TrakHoundApiResponse(202);
        public static TrakHoundApiResponse Accepted(string message) => new TrakHoundApiResponse(202, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundApiResponse Accepted(object response) => new TrakHoundApiResponse(202, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundApiResponse Accepted(byte[] content, string contentType) => new TrakHoundApiResponse(202, content, contentType);

        public static TrakHoundApiResponse File(byte[] content, string contentType, string filename)
        {
            var response = new TrakHoundApiResponse();
            response.Success = true;
            response.StatusCode = 200;
            response.ContentType = contentType;

            var parameters = new Dictionary<string, string>();
            parameters.Add(_filenameParameterKey, filename);
            response.Parameters = parameters;

            if (content != null)
            {
                try
                {
                    response.Content = new MemoryStream(content);
                }
                catch { }
            }

            return response;
        }

        public static TrakHoundApiResponse File(Stream content, string contentType, string filename)
        {
            var response = new TrakHoundApiResponse();
            response.Success = true;
            response.StatusCode = 200;
            response.Content = content;
            response.ContentType = contentType;

            var parameters = new Dictionary<string, string>();
            parameters.Add(_filenameParameterKey, filename);
            response.Parameters = parameters;

            return response;
        }


        public static TrakHoundApiResponse Redirect(string location, int statusCode = 303)
        {
            var response = new TrakHoundApiResponse();
            response.Success = true;
            response.StatusCode = statusCode;

            var parameters = new Dictionary<string, string>();
            parameters.Add(_locationParameterKey, location);
            response.Parameters = parameters;

            return response;
        }


        public static TrakHoundApiResponse BadRequest() => new TrakHoundApiResponse(400);
        public static TrakHoundApiResponse BadRequest(string message) => new TrakHoundApiResponse(400, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundApiResponse BadRequest(object response) => new TrakHoundApiResponse(400, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundApiResponse BadRequest(byte[] content, string contentType) => new TrakHoundApiResponse(400, content, contentType);


        public static TrakHoundApiResponse NotAuthorized() => new TrakHoundApiResponse(401);
        public static TrakHoundApiResponse NotAuthorized(string message) => new TrakHoundApiResponse(401, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundApiResponse NotAuthorized(object response) => new TrakHoundApiResponse(401, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundApiResponse NotAuthorized(byte[] content, string contentType) => new TrakHoundApiResponse(401, content, contentType);


        public static TrakHoundApiResponse NotFound() => new TrakHoundApiResponse(404);
        public static TrakHoundApiResponse NotFound(string message) => new TrakHoundApiResponse(404, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundApiResponse NotFound(object response) => new TrakHoundApiResponse(404, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundApiResponse NotFound(byte[] content, string contentType) => new TrakHoundApiResponse(404, content, contentType);


        public static TrakHoundApiResponse InternalError() => new TrakHoundApiResponse(500);
        public static TrakHoundApiResponse InternalError(string message) => new TrakHoundApiResponse(500, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundApiResponse InternalError(object response) => new TrakHoundApiResponse(500, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundApiResponse InternalError(byte[] content, string contentType) => new TrakHoundApiResponse(500, content, contentType);
    }
}
