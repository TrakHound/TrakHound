// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    public partial class Api : EntitiesApiControllerBase
    {
        [TrakHoundApiPublish("entries")]
        public async Task<TrakHoundApiResponse> PublishEntries(
            [FromBody("application/json")] TrakHoundEntityTransaction transaction,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (transaction != null && transaction.PublishOperations != null)
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();
                var indexes = new List<EntityIndexPublishRequest>();


                if (!transaction.PublishOperations.Object.IsNullOrEmpty())
                {
                    CreateObjects(publishCollection, transaction.PublishOperations.Object, now);

                    foreach (var entry in transaction.PublishOperations.Object)
                    {
                        if (!entry.Indexes.IsNullOrEmpty())
                        {
                            foreach (var index in entry.Indexes)
                            {
                                if (!string.IsNullOrEmpty(index.Index) && !string.IsNullOrEmpty(index.Name) && index.Value != null)
                                {
                                    var targetNamespace = !string.IsNullOrEmpty(entry.Namespace) ? entry.Namespace : TrakHoundPath.GetNamespace(entry.Path);
                                    if (string.IsNullOrEmpty(targetNamespace)) targetNamespace = TrakHoundNamespace.DefaultNamespace;

                                    var targetPath = TrakHoundPath.ToRoot(TrakHoundPath.GetPartialPath(entry.Path));

                                    var target = TrakHoundPath.Combine(index.Index, index.Name)?.ToLower().ToSHA256Hash();
                                    var subject = TrakHoundPath.GetUuid(targetNamespace, targetPath);

                                    var indexRequest = new EntityIndexPublishRequest(target, index.DataType, index.Value, subject, "DUMMY", now);
                                    if (indexRequest.IsValid) indexes.Add(indexRequest);
                                }
                            }
                        }
                    }
                }

                if (!transaction.PublishOperations.Definition.IsNullOrEmpty())
                {
                    var controller = new Definitions(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Definition, now);
                }


                if (!transaction.PublishOperations.Assignment.IsNullOrEmpty())
                {
                    var controller = new Assignments(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Assignment, now);
                }

                if (!transaction.PublishOperations.Blob.IsNullOrEmpty())
                {
                    var controller = new Blobs(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Blob, now);
                }

                if (!transaction.PublishOperations.Boolean.IsNullOrEmpty())
                {
                    var controller = new Booleans(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Boolean, now);
                }

                if (!transaction.PublishOperations.Duration.IsNullOrEmpty())
                {
                    var controller = new Durations(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Duration, now);
                }

                if (!transaction.PublishOperations.Event.IsNullOrEmpty())
                {
                    var controller = new Events(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Event, now);
                }

                if (!transaction.PublishOperations.Group.IsNullOrEmpty())
                {
                    var controller = new Groups(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Group, now);
                }

                if (!transaction.PublishOperations.Hash.IsNullOrEmpty())
                {
                    var controller = new Hashes(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Hash, now);
                }

                if (!transaction.PublishOperations.Log.IsNullOrEmpty())
                {
                    var controller = new Logs(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Log, now);
                }

                if (!transaction.PublishOperations.Number.IsNullOrEmpty())
                {
                    var controller = new Numbers(Configuration, Client);
                    await controller.CreateEntities(publishCollection, transaction.PublishOperations.Number, now);
                }

                if (!transaction.PublishOperations.Observation.IsNullOrEmpty())
                {
                    var controller = new Observations(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Observation, now);
                }

                if (!transaction.PublishOperations.Queue.IsNullOrEmpty())
                {
                    var controller = new Queues(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Queue, now);
                }

                if (!transaction.PublishOperations.Reference.IsNullOrEmpty())
                {
                    var controller = new References(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Reference, now);
                }

                if (!transaction.PublishOperations.Set.IsNullOrEmpty())
                {
                    var controller = new Sets(Configuration, Client);
                    await controller.CreateEntities(publishCollection, transaction.PublishOperations.Set, now);
                }

                if (!transaction.PublishOperations.State.IsNullOrEmpty())
                {
                    var controller = new States(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.State, now);
                }

                if (!transaction.PublishOperations.String.IsNullOrEmpty())
                {
                    var controller = new Strings(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.String, now);
                }

                if (!transaction.PublishOperations.Statistic.IsNullOrEmpty())
                {
                    var controller = new Statistics(Configuration, Client);
                    await controller.CreateEntities(publishCollection, transaction.PublishOperations.Statistic, now);
                }

                if (!transaction.PublishOperations.Timestamp.IsNullOrEmpty())
                {
                    var controller = new Timestamps(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Timestamp, now);
                }

                if (!transaction.PublishOperations.TimeRange.IsNullOrEmpty())
                {
                    var controller = new TimeRanges(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.TimeRange, now);
                }

                if (!transaction.PublishOperations.Vocabulary.IsNullOrEmpty())
                {
                    var controller = new Vocabularies(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.Vocabulary, now);
                }

                if (!transaction.PublishOperations.VocabularySet.IsNullOrEmpty())
                {
                    var controller = new VocabularySets(Configuration, Client);
                    controller.CreateEntities(publishCollection, transaction.PublishOperations.VocabularySet, now);
                }


                // Publish Entities
                if (await PublishEntities(publishCollection, now, async, transaction.Source, indexes, routerId))
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
    }
}
