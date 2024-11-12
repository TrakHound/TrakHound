// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;

namespace TrakHound.Api
{
    public class TrakHoundApiHttpResult : ActionResult
    {
        private readonly TrakHoundApiResponse _response;


        public TrakHoundApiHttpResult(TrakHoundApiResponse response)
        {
            _response = response;
        }


        public async override void ExecuteResult(ActionContext context)
        {
            if (_response.IsValid())
            {
                try
                {
                    // Set Path Header (used in TrakHound API)
                    if (!string.IsNullOrEmpty(_response.Path))
                    {
                        context.HttpContext.Response.Headers.Add("Path", _response.Path);
                    }

                    // Set Status Code
                    context.HttpContext.Response.StatusCode = _response.StatusCode;

                    // Set Content
                    if (_response.Content != null)
                    {
                        context.HttpContext.Response.Headers.Add("Content-Type", _response.ContentType);
                        context.HttpContext.Response.Headers.Add("Content-Length", _response.Content.Length.ToString());

                        // Write to Response Body
                        await _response.Content.CopyToAsync(context.HttpContext.Response.Body);
                    }
                }
                catch { }
            }
        }
    }
}
