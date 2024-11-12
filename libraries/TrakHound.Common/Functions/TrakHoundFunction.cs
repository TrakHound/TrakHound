// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Packages;
using TrakHound.Requests;
using TrakHound.Volumes;

namespace TrakHound.Functions
{
    public abstract class TrakHoundFunction : ITrakHoundFunction
    {
        private readonly ITrakHoundFunctionConfiguration _configuration;
        private readonly ITrakHoundClient _client;
        private readonly ITrakHoundVolume _volume;


        public string Id { get; set; }

        public string InstanceId { get; set; }

        public TrakHoundPackage Package { get; set; }

        public TrakHoundSourceChain SourceChain { get; set; }

        public TrakHoundSourceEntry Source { get; set; }

        public ITrakHoundFunctionConfiguration Configuration => _configuration;

        public ITrakHoundClient Client => _client;

        public ITrakHoundVolume Volume => _volume;


        public event EventHandler<TrakHoundLogItem> LogReceived;


        public TrakHoundFunction(
            ITrakHoundFunctionConfiguration configuration,
            ITrakHoundClient client,
            ITrakHoundVolume volume
            )
        {
            _configuration = configuration;
            _client = client;
            _volume = volume;
        }


        public async Task<TrakHoundFunctionResponse> Run(IReadOnlyDictionary<string, string> parameters)
        {
            return await OnRun(parameters);
        }

        protected async virtual Task<TrakHoundFunctionResponse> OnRun(IReadOnlyDictionary<string, string> parameters) => new TrakHoundFunctionResponse();


        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose() { }


        protected void Log(TrakHoundLogLevel level, string message, string code = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem("Main", level, message, code, UnixDateTime.Now));
        }

        protected void Log(string logName, TrakHoundLogLevel level, string message, string code = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem(logName, level, message, code, UnixDateTime.Now));
        }


        public static TrakHoundFunctionResponse Ok() => new TrakHoundFunctionResponse(200);
        public static TrakHoundFunctionResponse Ok(string message) => new TrakHoundFunctionResponse(200, message);
        public static TrakHoundFunctionResponse Ok(object response) => new TrakHoundFunctionResponse(200, response);
        public static TrakHoundFunctionResponse Ok(byte[] content, string contentType) => new TrakHoundFunctionResponse(200, content, contentType);


        public static TrakHoundFunctionResponse Created() => new TrakHoundFunctionResponse(201);
        public static TrakHoundFunctionResponse Created(string message) => new TrakHoundFunctionResponse(201, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundFunctionResponse Created(object response) => new TrakHoundFunctionResponse(201, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundFunctionResponse Created(byte[] content, string contentType) => new TrakHoundFunctionResponse(201, content, contentType);


        public static TrakHoundFunctionResponse Accepted() => new TrakHoundFunctionResponse(202);
        public static TrakHoundFunctionResponse Accepted(string message) => new TrakHoundFunctionResponse(202, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundFunctionResponse Accepted(object response) => new TrakHoundFunctionResponse(202, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundFunctionResponse Accepted(byte[] content, string contentType) => new TrakHoundFunctionResponse(202, content, contentType);

        public static TrakHoundFunctionResponse File(byte[] content, string contentType, string filename)
        {
            var response = new TrakHoundFunctionResponse();
            response.StatusCode = 200;
            response.AddObjectParameter("filename", filename);
            response.AddObjectParameter("contentType", contentType);
            if (content != null) response.AddObjectParameter("content", Convert.ToBase64String(content));

            return response;
        }

        public static TrakHoundFunctionResponse BadRequest() => new TrakHoundFunctionResponse(400);
        public static TrakHoundFunctionResponse BadRequest(string message) => new TrakHoundFunctionResponse(400, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundFunctionResponse BadRequest(object response) => new TrakHoundFunctionResponse(400, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundFunctionResponse BadRequest(byte[] content, string contentType) => new TrakHoundFunctionResponse(400, content, contentType);


        public static TrakHoundFunctionResponse NotAuthorized() => new TrakHoundFunctionResponse(401);
        public static TrakHoundFunctionResponse NotAuthorized(string message) => new TrakHoundFunctionResponse(401, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundFunctionResponse NotAuthorized(object response) => new TrakHoundFunctionResponse(401, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundFunctionResponse NotAuthorized(byte[] content, string contentType) => new TrakHoundFunctionResponse(401, content, contentType);


        public static TrakHoundFunctionResponse NotFound() => new TrakHoundFunctionResponse(404);
        public static TrakHoundFunctionResponse NotFound(string message) => new TrakHoundFunctionResponse(404, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundFunctionResponse NotFound(object response) => new TrakHoundFunctionResponse(404, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundFunctionResponse NotFound(byte[] content, string contentType) => new TrakHoundFunctionResponse(404, content, contentType);


        public static TrakHoundFunctionResponse InternalError() => new TrakHoundFunctionResponse(500);
        public static TrakHoundFunctionResponse InternalError(string message) => new TrakHoundFunctionResponse(500, message.ToUtf8Bytes(), "text/plain");
        public static TrakHoundFunctionResponse InternalError(object response) => new TrakHoundFunctionResponse(500, response.ToJson(true).ToUtf8Bytes(), "application/json");
        public static TrakHoundFunctionResponse InternalError(byte[] content, string contentType) => new TrakHoundFunctionResponse(500, content, contentType);
    }
}
