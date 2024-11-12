// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using TrakHound.Api;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Json
{
    public class Controller : TrakHoundApiController
    {
        class QueryResults
        {
            public string Target { get; set; }
            public string Group { get; set; }
            public Dictionary<string, string> Properties { get; set; }
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> Query([FromQuery] string path, [FromQuery] string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var results = await GetResults(path, routerId);
                if (!results.IsNullOrEmpty())
                {
                    var pathType = TrakHoundPath.GetType(path);
                    if (pathType == TrakHoundPathType.Absolute)
                    {
                        return Ok(results.FirstOrDefault());
                    }
                    else
                    {
                        return Ok(results);
                    }
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

        //[TrakHoundApiSubscribe]
        //public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe([FromQuery] string path)
        //{
        //    if (!string.IsNullOrEmpty(path))
        //    {
        //        var initialResults = await GetResults( path);
        //        TrakHoundApiResponse initialValue = null;

        //        var pathType = TrakHoundObjectPath.GetType(path);
        //        if (pathType == TrakHoundObjectPathType.Absolute)
        //        {
        //            initialValue = Ok(initialResults.FirstOrDefault());
        //        }
        //        else
        //        {
        //            initialValue = Ok(initialResults);
        //        }

        //        var entitiesConsumer = await Client.System.Entities.SubscribeToContent(path);
        //        if (entitiesConsumer != null)
        //        {
        //            var consumer = new TrakHoundConsumer<IEnumerable<ITrakHoundEntity>, TrakHoundApiResponse>(entitiesConsumer);
        //            consumer.InitialValue = initialValue;
        //            consumer.OnReceivedAsync = async (entities) =>
        //            {
        //                var collection = new TrakHoundEntityCollection();
        //                collection.Add(entities);

        //                var results = await GetResults(collection, path);

        //                var pathType = TrakHoundObjectPath.GetType(path);
        //                if (pathType == TrakHoundObjectPathType.Absolute)
        //                {
        //                    return Ok(results.FirstOrDefault());
        //                }
        //                else
        //                {
        //                    return Ok(results);
        //                }

        //                //var targetModels = collection.Objects.QueryObjectModels(path);
        //                //if (!targetModels.IsNullOrEmpty())
        //                //{
        //                //    var resultsCollection = await Client.Entities.GetObjectContent(targetModels);
        //                //    var results = new List<Dictionary<string, object>>();

        //                //    foreach (var targetModel in targetModels)
        //                //    {
        //                //        results.Add(ProcessObject(resultsCollection, targetModel.Children));
        //                //    }

        //                //    var pathType = TrakHoundObjectPath.GetType(path);
        //                //    if (pathType == TrakHoundObjectPathType.Path)
        //                //    {
        //                //        return Ok(results.FirstOrDefault());
        //                //    }
        //                //    else
        //                //    {
        //                //        return Ok(results);
        //                //    }
        //                //}

        //                //return null;
        //            };
        //            return consumer;
        //        }
        //    }

        //    return null;
        //}

        //[TrakHoundApiQuery("query")]
        //public async Task<TrakHoundApiResponse> QueryEntities([FromBody(ContentType = "text/plain")] string query)
        //{
        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        var queryResponse = await Client.System.Entities.Query(query);
        //        if (queryResponse != null)
        //        {
        //            if (!queryResponse.Models.IsNullOrEmpty())
        //            {
        //                var customModels = new List<Dictionary<string, string>>();
        //                foreach (var model in queryResponse.Models)
        //                {
        //                    if (!model.Properties.IsNullOrEmpty())
        //                    {
        //                        var customModel = new Dictionary<string, string>();
        //                        foreach (var property in model.Properties)
        //                        {
        //                            customModel.Add(property.Name, property.Value);
        //                        }
        //                        customModels.Add(customModel);
        //                    }
        //                }
        //                return Ok(customModels);
        //            }
        //            else if (queryResponse.Entities != null)
        //            {
        //                var targets = queryResponse.Entities.GetTargetEntities();
        //                if (!targets.IsNullOrEmpty())
        //                {
        //                    var targetObjectUuids = targets.Select(o => o.Uuid);
        //                    var targetModels = queryResponse.Entities.Objects.GetObjectPartialModels(targetObjectUuids);

        //                    var contentModels = await Client.System.Entities.Objects.QueryModelsByParentUuid(targetObjectUuids, 0, 10000);
        //                    var dContentModels = contentModels?.ToListDictionary(o => o.ParentUuid);

        //                    var resultsCollection = await Client.Entities.GetObjectContent(targetModels, contentModels?.ToFlatList());
        //                    var results = new List<Dictionary<string, object>>();

        //                    foreach (var targetModel in targetModels)
        //                    {
        //                        results.Add(ProcessObject(resultsCollection, dContentModels.Get(targetModel.Uuid)));
        //                    }

        //                    return Ok(results);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return NotFound("Query Responded with No Results");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //    return InternalError();
        //}


        private async Task<List<Dictionary<string, object>>> GetResults(string path, string routerId = null)
        {
            var targetModels = await Client.System.Entities.Objects.Query(path, routerId: routerId);
            if (!targetModels.IsNullOrEmpty())
            {
                var targetObjectUuids = targetModels.Select(o => o.Uuid);
                var contentModels = await Client.System.Entities.Objects.QueryChildrenByRootUuid(targetObjectUuids, 0, 10000, routerId: routerId);
                var dContentModels = contentModels?.ToListDictionary(o => o.ParentUuid);

                var resultsCollection = await Client.Entities.GetObjectContent(targetModels, contentModels);
                var results = new List<Dictionary<string, object>>();

                foreach (var targetModel in targetModels)
                {
                    results.Add(ProcessObject(resultsCollection, dContentModels.Get(targetModel.Uuid)));
                }

                return results;
            }

            return null;
        }

        private static Dictionary<string, object> ProcessObject(TrakHoundEntityCollection collection, IEnumerable<ITrakHoundObjectEntity> objectEntities)
        {
            var results = new List<KeyValuePair<string, object>>();

            if (!objectEntities.IsNullOrEmpty())
            {
                foreach (var objectEntity in objectEntities)
                {
                    var key = objectEntity.Name.ToCamelCase();

                    var contentType = objectEntity.ContentType.ConvertEnum<TrakHoundObjectContentType>();
                    switch (contentType)
                    {
                        case TrakHoundObjectContentType.Directory:

                            var childObjects = collection.Objects.QueryObjectsByParentUuid(objectEntity.Uuid);
                            if (!childObjects.IsNullOrEmpty())
                            {
                                results.Add(new KeyValuePair<string, object>(key, ProcessObject(collection, childObjects)));
                            }
                            break;

                        case TrakHoundObjectContentType.Boolean:

                            var booleanEntity = collection.Objects.QueryBooleanByObjectUuid(objectEntity.Uuid);
                            if (booleanEntity != null)
                            {
                                results.Add(new KeyValuePair<string, object>(key, booleanEntity.Value));
                            }
                            break;

                        case TrakHoundObjectContentType.Duration:

                            var durationEntity = collection.Objects.QueryDurationByObjectUuid(objectEntity.Uuid);
                            if (durationEntity != null)
                            {
                                results.Add(new KeyValuePair<string, object>(key, durationEntity.Value));
                            }
                            break;

                        case TrakHoundObjectContentType.Hash:

                            var hashEntities = collection.Objects.QueryHashesByObjectUuid(objectEntity.Uuid);
                            if (!hashEntities.IsNullOrEmpty())
                            {
                                results.Add(new KeyValuePair<string, object>(key, hashEntities.ToDictionary(o => o.Key, o => o.Value)));
                            }
                            break;

                        case TrakHoundObjectContentType.Number:

                            var numberEntity = collection.Objects.QueryNumberByObjectUuid(objectEntity.Uuid);
                            if (numberEntity != null)
                            {
                                results.Add(new KeyValuePair<string, object>(key, numberEntity.Value));
                            }
                            break;

                        case TrakHoundObjectContentType.Observation:

                            var observationEntities = collection.Objects.QueryObservationsByObjectUuid(objectEntity.Uuid);
                            if (observationEntities != null)
                            {
                                var observations = new List<TrakHoundObservationValue>();
                                foreach (var observationEntity in observationEntities)
                                {
                                    var observation = new TrakHoundObservationValue();
                                    observation.Value = observationEntity.Value;
                                    observation.Timestamp = observationEntity.Timestamp.ToLocalDateTime();
                                    observations.Add(observation);
                                }

                                results.Add(new KeyValuePair<string, object>(key, observations));
                            }
                            break;

                        case TrakHoundObjectContentType.Set:

                            var setEntities = collection.Objects.QuerySetsByObjectUuid(objectEntity.Uuid);
                            if (!setEntities.IsNullOrEmpty())
                            {
                                results.Add(new KeyValuePair<string, object>(key, setEntities.Select(o => o.Value)));
                            }
                            break;

                        case TrakHoundObjectContentType.String:

                            var stringEntity = collection.Objects.QueryStringByObjectUuid(objectEntity.Uuid);
                            if (stringEntity != null)
                            {
                                results.Add(new KeyValuePair<string, object>(key, stringEntity.Value));
                            }
                            break;

                        case TrakHoundObjectContentType.Timestamp:

                            var timestampEntity = collection.Objects.QueryTimestampByObjectUuid(objectEntity.Uuid);
                            if (timestampEntity != null)
                            {
                                results.Add(new KeyValuePair<string, object>(key, timestampEntity.Value.ToDateTime().ToString("o")));
                            }
                            break;

                        case TrakHoundObjectContentType.Vocabulary:

                            var vocabularyEntity = collection.Objects.QueryVocabularyByObjectUuid(objectEntity.Uuid);
                            if (vocabularyEntity != null)
                            {
                                //results.Add(new KeyValuePair<string, object>(key, vocabularyEntity.DefinitionUuid?.Type));
                            }
                            break;
                    }
                }
            }

            return new Dictionary<string, object>(results.OrderBy(o => o.Key));
        }


        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> Publish(
            [FromBody("text/plain")] string content,
            [FromQuery] string basePath,
            [FromQuery] bool async = false
            )
        {
            if (!string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(content))
            {
                var success = false;

                try
                {
                    var objectResults = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                    if (objectResults != null)
                    {
                        success = true;

                        foreach (var key in objectResults.Keys)
                        {
                            var element = (JsonElement)objectResults[key];

                            await ProcessJson(basePath, key, element);
                        }
                    }
                }
                catch { }

                if (!success)
                {
                    try
                    {
                        var arrayResults = JsonSerializer.Deserialize<IEnumerable<object>>(content);
                        if (arrayResults != null)
                        {
                            success = true;

                            foreach (JsonElement result in arrayResults)
                            {
                                var keyProperty = result.EnumerateObject().FirstOrDefault(o => o.Value.ValueKind == JsonValueKind.String || o.Value.ValueKind == JsonValueKind.Number);
                                await ProcessJson(basePath, keyProperty.Value.ToString(), result);
                            }
                        }
                    }
                    catch { }
                }

                if (success) return Ok();
            }

            return BadRequest();
        }

        private async Task ProcessJson(string basePath, string propertyName, JsonElement element)
        {
            var path = TrakHoundPath.Combine(basePath, propertyName.ToTitleCase());

            switch (element.ValueKind)
            {
                case JsonValueKind.String: await Client.Entities.PublishString(path, element.ToString()); break;

                case JsonValueKind.Number: await Client.Entities.PublishNumber(path, element.ToString()); break;

                case JsonValueKind.Array:

                    foreach (var entry in element.EnumerateArray())
                    {
                        switch (entry.ValueKind)
                        {
                            case JsonValueKind.String: await Client.Entities.PublishSet(path, entry.ToString()); break;
                            case JsonValueKind.Number: await Client.Entities.PublishSet(path, entry.ToString()); break;

                            case JsonValueKind.Object:

                                var keyProperty = entry.EnumerateObject().FirstOrDefault(o => o.Value.ValueKind == JsonValueKind.String || o.Value.ValueKind == JsonValueKind.Number);
                                await ProcessJson(path, keyProperty.Value.ToString(), entry);
                                break;
                        }
                    }
                    break;

                case JsonValueKind.Object:

                    foreach (var property in element.EnumerateObject())
                    {
                        await ProcessJson(path, property.Name, property.Value);
                    }
                    break;
            }
        }
    }
}
