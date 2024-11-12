// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TrakHound.Http
{
    public static class ControllerBaseExtensions
    {
        public static async Task<string> GetBodyString(this ControllerBase controllerBase)
        {
            if (controllerBase != null && controllerBase.Request.Body != null)
            {
                try
                {
                    byte[] content = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        await controllerBase.Request.Body.CopyToAsync(memoryStream);
                        content = memoryStream.ToArray();
                    }

                    return Encoding.UTF8.GetString(content);
                }
                catch { }
            }

            return null;
        }
    }
}
