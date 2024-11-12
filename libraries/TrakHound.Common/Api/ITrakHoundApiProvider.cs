// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TrakHound.Api
{
    public interface ITrakHoundApiProvider : IDisposable
    {
        public event TrakHoundApiLogHandler ApiLogUpdated;


        IEnumerable<TrakHoundApiInformation> GetRouteInformation();

        IEnumerable<TrakHoundApiInformation> GetPackageInformation();

        TrakHoundApiInformation GetInformation(string apiId);

        TrakHoundApiEndpointInformation GetEndpointInformation(string endpointType, string route);


        bool IsRouteValid(string route);


        Task<TrakHoundApiResponse> QueryByRoute(string route, Stream requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<TrakHoundApiResponse> QueryByPackage(string packageId, string packageVersion, string path, Stream requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null);


        Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeByRoute(string route, Stream requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeByPackage(string packageId, string packageVersion, string path, Stream requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null);


        Task<TrakHoundApiResponse> PublishByRoute(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<TrakHoundApiResponse> PublishByPackage(string packageId, string packageVersion, string path, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null);


        Task<TrakHoundApiResponse> Delete(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
    }
}
