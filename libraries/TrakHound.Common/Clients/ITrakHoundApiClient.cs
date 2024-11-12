// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.Clients
{
    public interface ITrakHoundApiClient
    {
        Task<TrakHoundApiResponse> Query(string route);
        Task<TrakHoundApiResponse> Query(string route, Dictionary<string, string> queryParameters);
        Task<TrakHoundApiResponse> Query(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<TrakHoundApiResponse> Query(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<TrakHoundApiResponse> Query(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<byte[]> QueryBytes(string route);
        Task<byte[]> QueryBytes(string route, Dictionary<string, string> queryParameters);
        Task<byte[]> QueryBytes(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<byte[]> QueryBytes(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<byte[]> QueryBytes(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<string> QueryString(string route);
        Task<string> QueryString(string route, Dictionary<string, string> queryParameters);
        Task<string> QueryString(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<string> QueryString(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<string> QueryString(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<TOutput> QueryJson<TOutput>(string route);
        Task<TOutput> QueryJson<TOutput>(string route, Dictionary<string, string> queryParameters);
        Task<TOutput> QueryJson<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<TOutput> QueryJson<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<TOutput> QueryJson<TOutput>(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, Dictionary<string, string> queryParameters);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route);
        Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, Dictionary<string, string> queryParameters);
        Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<ITrakHoundConsumer<string>> SubscribeString(string route);
        Task<ITrakHoundConsumer<string>> SubscribeString(string route, Dictionary<string, string> queryParameters);
        Task<ITrakHoundConsumer<string>> SubscribeString(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<string>> SubscribeString(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<string>> SubscribeString(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route);
        Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, Dictionary<string, string> queryParameters);
        Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<TrakHoundApiResponse> Publish(string route);
        Task<TrakHoundApiResponse> Publish(string route, Dictionary<string, string> queryParameters);
        Task<TrakHoundApiResponse> Publish(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<TrakHoundApiResponse> Publish(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<TrakHoundApiResponse> Publish(string route, Stream stream, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<TOutput> Publish<TOutput>(string route);
        Task<TOutput> Publish<TOutput>(string route, Dictionary<string, string> queryParameters);
        Task<TOutput> Publish<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<TOutput> Publish<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<TOutput> Publish<TOutput>(string route, Stream stream, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<TrakHoundApiResponse> Delete(string route);
        Task<TrakHoundApiResponse> Delete(string route, Dictionary<string, string> queryParameters);
        Task<TrakHoundApiResponse> Delete(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<TrakHoundApiResponse> Delete(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<TrakHoundApiResponse> Delete(string route, Stream stream, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);

        Task<TOutput> Delete<TOutput>(string route);
        Task<TOutput> Delete<TOutput>(string route, Dictionary<string, string> queryParameters);
        Task<TOutput> Delete<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null);
        Task<TOutput> Delete<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
        Task<TOutput> Delete<TOutput>(string route, Stream stream, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null);
    }
}
