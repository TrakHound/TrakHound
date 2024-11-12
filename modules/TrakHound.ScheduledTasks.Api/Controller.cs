// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.ScheduledTasks.Api
{
    public class Controller : TrakHoundApiController
    {
        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> ListConfigurations()
        {
            var configurationFilePaths = await Volume.ListFiles("/");
            if (!configurationFilePaths.IsNullOrEmpty())
            {
                var configurations = new List<ScheduledTaskConfiguration>();

                foreach (var configurationFilePath in configurationFilePaths)
                {
                    var configuration = await Volume.ReadJson<ScheduledTaskConfiguration>(configurationFilePath);
                    if (configuration != null)
                    {
                        configurations.Add(configuration);
                    }
                }

                return Ok(configurations);
            }
            else
            {
                return NotFound();
            }
        }

        [TrakHoundApiQuery("{configurationId}")]
        public async Task<TrakHoundApiResponse> GetConfiguration([FromRoute] string configurationId)
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                var configurationFilePath = $"{configurationId}.json";
                var configuration = await Volume.ReadJson<ScheduledTaskConfiguration>(configurationFilePath);
                if (configuration != null)
                {
                    return Ok(configuration);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishConfiguration([FromBody] ScheduledTaskConfiguration configuration)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.Id))
            {
                var configurationFilePath = $"{configuration.Id}.json";

                if (await Volume.WriteJson(configurationFilePath, configuration))
                {
                    return Ok(configuration);
                }
                else
                {
                    return InternalError();
                }
            }
            else
            {
                return BadRequest();
            }
        }

		[TrakHoundApiDelete("{configurationId}")]
		public async Task<TrakHoundApiResponse> DeleteConfiguration([FromRoute] string configurationId)
		{
			if (!string.IsNullOrEmpty(configurationId))
			{
				var configurationFilePath = $"{configurationId}.json";

				if (await Volume.Delete(configurationFilePath))
				{
					return Ok();
				}
				else
				{
					return NotFound();
				}
			}
			else
			{
				return BadRequest();
			}
		}
	}
}
