using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound;
using TrakHound.Api;
using TrakHound.Entities;

public class Controller : TrakHoundApiController
{
    private const string _basePath = "TrakHound:/.cache";


    [TrakHoundApiQuery("{name}/{key}")]
    public async Task<TrakHoundApiResponse> Get([FromRoute] string name, [FromRoute] string key)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(key))
        {
            var path = TrakHoundPath.Combine(_basePath, name);
            var uuid = TrakHoundObjectHashEntity.GenerateUuid(TrakHoundPath.GetUuid(path), key);
            var entity = await Client.System.Entities.Objects.Hash.ReadByUuid(uuid);
            if (entity != null)
            {
                return Ok(entity.Value);
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

    [TrakHoundApiQuery("{name}")]
    public async Task<TrakHoundApiResponse> Get([FromRoute] string name, [FromBody] IEnumerable<string> keys)
    {
        if (!string.IsNullOrEmpty(name) && !keys.IsNullOrEmpty())
        {
            var path = TrakHoundPath.Combine(_basePath, name);
            var uuids = new List<string>(keys.Count());
            foreach (var key in keys)
            {
                uuids.Add(TrakHoundObjectHashEntity.GenerateUuid(TrakHoundPath.GetUuid(path), key));
            }

            var entities = await Client.System.Entities.Objects.Hash.ReadByUuid(uuids);
            if (!entities.IsNullOrEmpty())
            {
                return Ok(entities.ToDictionary(o => o.Key, o => o.Value));
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


    [TrakHoundApiPublish("{name}/{key}")]
    public async Task<TrakHoundApiResponse> Publish([FromRoute] string name, [FromRoute] string key, [FromBody] string value)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            var path = TrakHoundPath.Combine(_basePath, name);
            var entities = new List<ITrakHoundEntity>();
            entities.Add(new TrakHoundObjectEntity(_basePath));
            entities.Add(new TrakHoundObjectEntity(path, TrakHoundObjectContentType.Hash));
            entities.Add(new TrakHoundObjectHashEntity(TrakHoundPath.GetUuid(path), key, value));
            if (await Client.System.Entities.Publish(entities, TrakHoundOperationMode.Sync))
            {
                return Created();
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

    [TrakHoundApiPublish("{name}")]
    public async Task<TrakHoundApiResponse> Publish([FromRoute] string name, [FromBody] Dictionary<string, string> values, [FromQuery] bool async = false)
    {
        if (!string.IsNullOrEmpty(name) && !values.IsNullOrEmpty())
        {
            var path = TrakHoundPath.Combine(_basePath, name);

            var entities = new List<ITrakHoundEntity>(values.Count + 1);
            entities.Add(new TrakHoundObjectEntity(_basePath));
            entities.Add(new TrakHoundObjectEntity(path, TrakHoundObjectContentType.Hash));
            foreach (var value in values)
            {
                entities.Add(new TrakHoundObjectHashEntity(TrakHoundPath.GetUuid(path), value.Key, value.Value));
            }

            if (await Client.System.Entities.Publish(entities, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync))
            {
                return Created();
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


    [TrakHoundApiDelete("{name}/{key}")]
    public async Task<TrakHoundApiResponse> Delete([FromRoute] string name, [FromRoute] string key)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(key))
        {
            var path = TrakHoundPath.Combine(_basePath, name);
            var uuid = TrakHoundObjectHashEntity.GenerateUuid(TrakHoundPath.GetUuid(path), key);

            if (await Client.System.Entities.Objects.Hash.Delete(uuid))
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

    [TrakHoundApiDelete("{name}")]
    public async Task<TrakHoundApiResponse> Delete([FromRoute] string name, [FromBody] IEnumerable<string> keys)
    {
        if (!string.IsNullOrEmpty(name) && !keys.IsNullOrEmpty())
        {
            var path = TrakHoundPath.Combine(_basePath, name);
            var uuids = new List<string>(keys.Count());
            foreach (var key in keys)
            {
                uuids.Add(TrakHoundObjectHashEntity.GenerateUuid(TrakHoundPath.GetUuid(path), key));
            }

            if (await Client.System.Entities.Objects.Hash.Delete(uuids, TrakHoundOperationMode.Sync))
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
