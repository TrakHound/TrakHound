// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpController : ControllerBase
    {
        public IActionResult ProcessJsonContentResponse(string json)
        {
            var result = new ContentResult();
            result.Content = json;
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(ITrakHoundEntity entity, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = TrakHoundEntity.ToArray(entity).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(TrakHoundCount count, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = new TrakHoundHttpCountResponse(count).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(IEnumerable<TrakHoundCount> counts, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = TrakHoundHttpCountResponse.Create(counts).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(TrakHoundAggregate aggregate, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = new TrakHoundHttpAggregateResponse(aggregate).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(IEnumerable<TrakHoundAggregate> aggregates, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = TrakHoundHttpAggregateResponse.Create(aggregates).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(TrakHoundAggregateWindow aggregateWindow, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = new TrakHoundHttpAggregateWindowResponse(aggregateWindow).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(IEnumerable<TrakHoundAggregateWindow> aggregateWindows, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = TrakHoundHttpAggregateWindowResponse.Create(aggregateWindows).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(TrakHoundTimeRangeSpan span, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = new TrakHoundHttpTimeRangeSpanResponse(span).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(IEnumerable<TrakHoundTimeRangeSpan> spans, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = TrakHoundHttpTimeRangeSpanResponse.Create(spans).ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse<TEntity>(IEnumerable<TEntity> entities, bool indent = false) where TEntity : ITrakHoundEntity
        {
            var content = new List<object[]>();
            foreach (var entity in entities.ToDistinct()) content.Add(TrakHoundEntity.ToArray(entity));

            var result = new ContentResult();
            result.Content = content.ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }

        public IActionResult ProcessJsonContentResponse(TrakHoundHttpEntityResponse response, bool indent = false)
        {
            var result = new ContentResult();
            result.Content = response.ToJson(indent);
            result.ContentType = "application/json";
            return result;
        }


        public static TOutput GetRequestBodyJson<TOutput>(Stream requestBody)
        {
            return Json.Convert<TOutput>(GetRequestBodyString(requestBody));
        }

        public static string GetRequestBodyString(Stream requestBody)
        {
            var requestBodyBytes = GetRequestBodyBytes(requestBody);
            if (requestBodyBytes != null)
            {
                try
                {
                    return Encoding.UTF8.GetString(requestBodyBytes);
                }
                catch { }
            }

            return null;
        }

        public static byte[] GetRequestBodyBytes(Stream requestBody)
        {
            if (requestBody != null)
            {
                try
                {
                    using (var readStream = new MemoryStream())
                    {
                        requestBody.CopyTo(readStream);
                        readStream.Seek(0, SeekOrigin.Begin);
                        return readStream.ToArray();
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
