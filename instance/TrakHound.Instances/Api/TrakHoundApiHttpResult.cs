// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TrakHound.Http;

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
                    // Set Parameter Headers
                    if (!_response.Parameters.IsNullOrEmpty())
                    {
                        foreach (var parameter in _response.Parameters)
                        {
                            if (!string.IsNullOrEmpty(parameter.Key) && !string.IsNullOrEmpty(parameter.Value))
                            {
                                var headerName = $"{HttpConstants.ApiParameterHeaderPrefix}-{parameter.Key.ToKebabCase()}";
                                context.HttpContext.Response.Headers.Add(headerName, parameter.Value);
                            }
                        }
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
