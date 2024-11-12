// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemApiClient
    {
        Task<IEnumerable<TrakHoundApiInformation>> GetRouteInformation();
        Task<IEnumerable<TrakHoundApiInformation>> GetPackageInformation();
        Task<TrakHoundApiInformation> GetInformation(string apiId);


        Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, string routerId = null);
        Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null);
        Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null);
        Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null);


        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, string routerId = null);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null);


        Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, string routerId = null);
        Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null);
        Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null);
        Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null);
        Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, Stream stream, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string apiId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Trace);
    }
}
