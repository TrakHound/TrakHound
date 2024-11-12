// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Clipboard
{
    public partial class Controller : TrakHoundApiController
	{
        private const string _clipboardBasePath = "Main:/.clipboard";


        [TrakHoundApiQuery("copy")]
        public async Task<TrakHoundApiResponse> Copy([FromQuery] string path, [FromQuery] string clipboardId)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(clipboardId))
            {
                var uuids = (await Client.System.Entities.Objects.QueryUuids(path, 0, long.MaxValue))?.Select(o => o.Uuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var clipboardPath = TrakHoundPath.Combine(_clipboardBasePath, clipboardId);
                    if (await Client.Entities.PublishSet(clipboardPath, uuids))
                    {
                        return Ok($"{uuids.Count()} Objects Copied to Clipboard");
                    }
                    else
                    {
                        return InternalError();
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

        [TrakHoundApiQuery("cut")]
        public async Task<TrakHoundApiResponse> Cut([FromQuery] string clipboardId, [FromQuery] string path)
        {
            if (!string.IsNullOrEmpty(clipboardId) && !string.IsNullOrEmpty(path))
            {
                var obj = await Client.Entities.GetObject(path);
                if (obj != null)
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

        [TrakHoundApiPublish("paste")]
        public async Task<TrakHoundApiResponse> Paste([FromQuery] string clipboardId, [FromQuery] string destinationBasePath)
        {
            if (!string.IsNullOrEmpty(clipboardId) && !string.IsNullOrEmpty(destinationBasePath))
            {
                var destinationPath = TrakHoundPath.GetPartialPath(destinationBasePath);
                var destinationNamespace = TrakHoundPath.GetNamespace(destinationBasePath);

                var clipboardPath = TrakHoundPath.Combine(_clipboardBasePath, clipboardId);
                var targetUuids = (await Client.Entities.GetSets(clipboardPath))?.Select(o => o.Value);
                if (!targetUuids.IsNullOrEmpty())
                {
                    var targetObjects = await Client.System.Entities.Objects.ReadByUuid(targetUuids);
                    if (!targetObjects.IsNullOrEmpty())
                    {
                        var contentObjects = await Client.System.Entities.Objects.QueryChildrenByRootUuid(targetObjects.Select(o => o.Uuid));

                        var originalCollection = await GetObjectContent(contentObjects);
                        if (originalCollection != null)
                        {
                            var transaction = new TrakHoundEntityTransaction();

                            foreach (var targetObject in targetObjects)
                            {
                                var originalUuid = targetObject.Uuid;
                                var originalPath = targetObject.Path;

                                var outputPath = TrakHoundPath.Combine(destinationPath, targetObject.Name);
                                outputPath = TrakHoundPath.SetNamespace(destinationNamespace, outputPath);

                                var outputUuid = TrakHoundPath.GetUuid(outputPath);

                                var outputObject = new TrakHoundObjectEntry();
                                outputObject.Path = outputPath;
                                outputObject.ContentType = targetObject.ContentType;
                                outputObject.DefinitionId = targetObject.DefinitionUuid;
                                transaction.Add(outputObject);

                                var targetContentObjects = GetContentObjects(ref originalCollection, originalUuid);
                                if (!targetContentObjects.IsNullOrEmpty())
                                {
                                    foreach (var targetContentObject in targetContentObjects)
                                    {
                                        var targetContentRelativePath = TrakHoundPath.GetRelativeTo(originalPath, targetContentObject.Path);
                                        if (!string.IsNullOrEmpty(targetContentRelativePath))
                                        {
                                            var targetContentPath = TrakHoundPath.Combine(outputPath, targetContentRelativePath);

                                            var contentOutputObject = new TrakHoundObjectEntry();
                                            contentOutputObject.Path = targetContentPath;
                                            contentOutputObject.ContentType = targetContentObject.ContentType;
                                            contentOutputObject.DefinitionId = targetContentObject.DefinitionUuid;
                                            transaction.Add(contentOutputObject);

                                            var contentType = targetContentObject.ContentType.ConvertEnum<TrakHoundObjectContentType>();
                                            switch (contentType)
                                            {
                                                case TrakHoundObjectContentType.Assignment:
                                                    var originalAssignments = originalCollection.Objects.QueryAssignmentsByAssigneeUuid(targetContentObject.Uuid);
                                                    if (!originalAssignments.IsNullOrEmpty())
                                                    {
                                                        foreach (var originalAssignment in originalAssignments)
                                                        {
                                                            transaction.Add(new TrakHoundAssignmentEntry(targetContentPath, $"uuid={originalAssignment.MemberUuid}", originalAssignment.AddTimestamp, originalAssignment.RemoveTimestamp));
                                                        }
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Blob:
                                                    var originalBlob = originalCollection.Objects.QueryBlobByObjectUuid(targetContentObject.Uuid);
                                                    if (originalBlob != null)
                                                    {
                                                        transaction.Add(new TrakHoundBlobEntry(targetContentPath, originalBlob.BlobId, originalBlob.ContentType, originalBlob.Size, originalBlob.Filename));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Boolean:
                                                    var originalBoolean = originalCollection.Objects.QueryBooleanByObjectUuid(targetContentObject.Uuid);
                                                    if (originalBoolean != null)
                                                    {
                                                        transaction.Add(new TrakHoundBooleanEntry(targetContentPath, originalBoolean.Value));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Duration:
                                                    var originalDuration = originalCollection.Objects.QueryDurationByObjectUuid(targetContentObject.Uuid);
                                                    if (originalDuration != null)
                                                    {
                                                        transaction.Add(new TrakHoundDurationEntry(targetContentPath, originalDuration.Value));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Event:
                                                    var originalEvents = originalCollection.Objects.QueryEventsByObjectUuid(targetContentObject.Uuid);
                                                    if (!originalEvents.IsNullOrEmpty())
                                                    {
                                                        foreach (var originalEvent in originalEvents)
                                                        {
                                                            transaction.Add(new TrakHoundEventEntry(targetContentPath, $"uuid={originalEvent.TargetUuid}", originalEvent.Timestamp));
                                                        }
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Group:
                                                    var originalGroups = originalCollection.Objects.QueryGroupsByGroupUuid(targetContentObject.Uuid);
                                                    if (!originalGroups.IsNullOrEmpty())
                                                    {
                                                        foreach (var originalGroup in originalGroups)
                                                        {
                                                            var memberObject = originalCollection.Objects.GetObject(originalGroup.MemberUuid);                                                       
                                                            if (memberObject != null) transaction.Add(new TrakHoundGroupEntry(targetContentPath, memberObject.Path));
                                                        }
                                                    }
                                                    break;


                                                case TrakHoundObjectContentType.Hash:
                                                    var originalHashes = originalCollection.Objects.QueryHashesByObjectUuid(targetContentObject.Uuid);
                                                    if (!originalHashes.IsNullOrEmpty())
                                                    {
                                                        foreach (var originalHash in originalHashes)
                                                        {
                                                            transaction.Add(new TrakHoundHashEntry(targetContentPath, originalHash.Key, originalHash.Value));
                                                        }
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Number:
                                                    var originalNumber = originalCollection.Objects.QueryNumberByObjectUuid(targetContentObject.Uuid);
                                                    if (originalNumber != null)
                                                    {
                                                        transaction.Add(new TrakHoundNumberEntry(targetContentPath, originalNumber.Value, (TrakHoundNumberDataType)originalNumber.DataType));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Observation:
                                                    var originalObservations = originalCollection.Objects.QueryObservationsByObjectUuid(targetContentObject.Uuid);
                                                    if (!originalObservations.IsNullOrEmpty())
                                                    {
                                                        foreach (var originalObservation in originalObservations)
                                                        {
                                                            transaction.Add(new TrakHoundObservationEntry(targetContentPath, originalObservation.Value, originalObservation.Timestamp, (TrakHoundObservationDataType)originalObservation.DataType));
                                                        }
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Reference:
                                                    var originalReference = originalCollection.Objects.QueryReferenceByObjectUuid(targetContentObject.Uuid);
                                                    if (originalReference != null)
                                                    {
                                                        var referenceTargetObject = originalCollection.Objects.GetObject(originalReference.TargetUuid);
                                                        if (referenceTargetObject != null) transaction.Add(new TrakHoundReferenceEntry(targetContentPath, referenceTargetObject.Path));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.State:
                                                    var originalStates = originalCollection.Objects.QueryStatesByObjectUuid(targetContentObject.Uuid);
                                                    if (originalStates != null)
                                                    {
                                                        foreach (var originalState in originalStates)
                                                        {
                                                            transaction.Add(new TrakHoundStateEntry(targetContentPath, originalState.DefinitionUuid, originalState.Timestamp, (int)originalState.TTL));
                                                        }
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.String:
                                                    var originalString = originalCollection.Objects.QueryStringByObjectUuid(targetContentObject.Uuid);
                                                    if (originalString != null)
                                                    {
                                                        transaction.Add(new TrakHoundStringEntry(targetContentPath, originalString.Value));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.TimeRange:
                                                    var originalTimeRange = originalCollection.Objects.QueryTimeRangeByObjectUuid(targetContentObject.Uuid);
                                                    if (originalTimeRange != null)
                                                    {
                                                        transaction.Add(new TrakHoundTimeRangeEntry(targetContentPath, originalTimeRange.Start.ToDateTime(), originalTimeRange.End.ToDateTime()));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Timestamp:
                                                    var originalTimestamp = originalCollection.Objects.QueryTimestampByObjectUuid(targetContentObject.Uuid);
                                                    if (originalTimestamp != null)
                                                    {
                                                        transaction.Add(new TrakHoundTimestampEntry(targetContentPath, originalTimestamp.Value));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.Vocabulary:
                                                    var originalVocabulary = originalCollection.Objects.QueryVocabularyByObjectUuid(targetContentObject.Uuid);
                                                    if (originalVocabulary != null)
                                                    {
                                                        transaction.Add(new TrakHoundVocabularyEntry(targetContentPath, originalVocabulary.DefinitionUuid));
                                                    }
                                                    break;

                                                case TrakHoundObjectContentType.VocabularySet:
                                                    var originalVocabularySets = originalCollection.Objects.QueryVocabularySetsByObjectUuid(targetContentObject.Uuid);
                                                    if (originalVocabularySets != null)
                                                    {
                                                        foreach (var originalVocabularySet in originalVocabularySets)
                                                        {
                                                            transaction.Add(new TrakHoundVocabularySetEntry(targetContentPath, originalVocabularySet.DefinitionUuid));
                                                        }
                                                    }
                                                    break;
                                            }
                                        }                       
                                    }
                                }
                            }


                            if (await Client.Entities.Publish(transaction))
                            {
                                return Ok($"{targetUuids.Count()} Objects Pasted Successfully");
                            }
                        }
                    }

                    return InternalError();
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

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> Delete([FromQuery] string clipboardId)
        {
            if (!string.IsNullOrEmpty(clipboardId))
            {
                var clipboardPath = TrakHoundPath.Combine(_clipboardBasePath, clipboardId);
                if (await Client.Entities.DeleteObject(clipboardPath))
                {
                    return Ok($"{clipboardId} Clipboard Deleted");
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



        private static IEnumerable<ITrakHoundObjectEntity> GetContentObjects(ref TrakHoundEntityCollection collection, string parentUuid)
        {
            if (collection != null && !string.IsNullOrEmpty(parentUuid))
            {
                var objs = new List<ITrakHoundObjectEntity>();
                
                var childObjectUuids = collection.Objects.QueryObjectsByParentUuid(parentUuid)?.Select(o => o.Uuid);
                if (!childObjectUuids.IsNullOrEmpty())
                {
                    foreach (var childObjectUuid in childObjectUuids)
                    {
                        var childObject = collection.Objects.GetObject(childObjectUuid);
                        if (childObject != null)
                        {
                            objs.Add(childObject);

                            var childContentObjs = GetContentObjects(ref collection, childObject.Uuid);
                            if (!childContentObjs.IsNullOrEmpty())
                            {
                                objs.AddRange(childContentObjs);
                            }
                        }
                    }
                }

                return objs;
            }

            return null;
        }

        public async Task<TrakHoundEntityCollection> GetObjectContent(IEnumerable<ITrakHoundObjectEntity> targetObjs)
        {
            if (!targetObjs.IsNullOrEmpty())
            {
                var collection = new TrakHoundEntityCollection();
                collection.Add(targetObjs);

                var assignmentObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Assignment);
                if (!assignmentObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Assignment.CurrentByAssigneeUuid(assignmentObjs?.Select(o => o.Uuid)), false);

                var blobObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Blob);
                if (!blobObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Blob.QueryByObjectUuid(blobObjs?.Select(o => o.Uuid)), false);

                var booleanObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Boolean);
                if (!booleanObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Boolean.QueryByObjectUuid(booleanObjs?.Select(o => o.Uuid)), false);

                var durationObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Duration);
                if (!durationObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Duration.QueryByObjectUuid(durationObjs?.Select(o => o.Uuid)), false);

                var eventObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Event);
                if (!eventObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Event.LatestByObjectUuid(eventObjs?.Select(o => o.Uuid)), false);

                var groupObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Group);
                if (!groupObjs.IsNullOrEmpty())
                {
                    var groupEntities = await Client.System.Entities.Objects.Group.QueryByGroupUuid(groupObjs?.Select(o => o.Uuid));
                    if (!groupEntities.IsNullOrEmpty()) collection.Add(groupEntities, false);
                }

                var hashObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Hash);
                if (!hashObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Hash.QueryByObjectUuid(hashObjs?.Select(o => o.Uuid)), false);

                var messageObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Message);
                if (!messageObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Message.QueryByObjectUuid(messageObjs?.Select(o => o.Uuid)), false);

                var messageQueueObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.MessageQueue);
                if (!messageQueueObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.MessageQueue.QueryByObjectUuid(messageQueueObjs?.Select(o => o.Uuid)), false);

                var numberObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Number);
                if (!numberObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Number.QueryByObjectUuid(numberObjs?.Select(o => o.Uuid)), false);

                var observationObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Observation);
                if (!observationObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Observation.LatestByObjectUuid(observationObjs?.Select(o => o.Uuid)), false);

                var queueObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Queue);
                if (!queueObjs.IsNullOrEmpty())
                {
                    var queueEntities = await Client.System.Entities.Objects.Queue.QueryByQueueUuid(queueObjs?.Select(o => o.Uuid));
                    if (!queueEntities.IsNullOrEmpty()) collection.Add(queueEntities, false);
                }

                var referenceObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Reference);
                if (!referenceObjs.IsNullOrEmpty())
                {
                    var referenceEntities = await Client.System.Entities.Objects.Reference.QueryByObjectUuid(referenceObjs?.Select(o => o.Uuid));
                    if (!referenceEntities.IsNullOrEmpty()) collection.Add(referenceEntities, false);
                }

                var setObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Set);
                if (!setObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Set.QueryByObjectUuid(setObjs?.Select(o => o.Uuid)), false);

                //var statisticObjs = fObjs.Where(o => o.ContentType == TrakHoundObjectContentType.Statistic.ToString());
                //if (!statisticObjs.IsNullOrEmpty()) collection.Add(await entitiesClient.Objects.Statistics.LatestByObjectUuid(statisticObjs?.Select(o => o.Uuid), 0, long.MaxValue), false);

                var stateObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.State);
                if (!stateObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.State.LatestByObjectUuid(stateObjs?.Select(o => o.Uuid)), false);

                var stringObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.String);
                if (!stringObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.String.QueryByObjectUuid(stringObjs?.Select(o => o.Uuid)), false);

                var timeRangeObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.TimeRange);
                if (!timeRangeObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.TimeRange.QueryByObjectUuid(timeRangeObjs?.Select(o => o.Uuid)), false);

                var timestampObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Timestamp);
                if (!timestampObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Timestamp.QueryByObjectUuid(timestampObjs?.Select(o => o.Uuid)), false);

                var vocabularyObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.Vocabulary);
                if (!vocabularyObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.Vocabulary.QueryByObjectUuid(vocabularyObjs?.Select(o => o.Uuid)), false);

                var vocabularySetObjs = targetObjs.Where(o => o.ContentType == TrakHoundObjectContentTypes.VocabularySet);
                if (!vocabularySetObjs.IsNullOrEmpty()) collection.Add(await Client.System.Entities.Objects.VocabularySet.QueryByObjectUuid(vocabularySetObjs?.Select(o => o.Uuid)), false);

                return collection;
            }

            return default;
        }
    }
}
