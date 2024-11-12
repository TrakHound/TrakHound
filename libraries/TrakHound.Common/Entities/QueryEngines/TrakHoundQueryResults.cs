// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using TrakHound.Entities.Collections;

namespace TrakHound.Entities.QueryEngines
{
    public static class TrakHoundQueryResults
    {
        public static TrakHoundQueryResult CreateObjectsResult(ITrakHoundObjectEntity[] targetObjects, bool target = false)
        {
            var result = new TrakHoundQueryResult();
            if (target) result.Schema = "trakhound.entities.objects.target";
            else result.Schema = "trakhound.entities.objects";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEntity.Namespace),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEntity.Path),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEntity.ContentType),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEntity.DefinitionUuid),
                DataType = TrakHoundQueryDataType.Definition
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEntity.Priority),
                DataType = TrakHoundQueryDataType.Number
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            result.Columns = columns.ToArray();

            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    rows[rowIndex, 0] = targetObjects[i].Namespace;
                    rows[rowIndex, 1] = targetObjects[i].Path;
                    rows[rowIndex, 2] = targetObjects[i].ContentType;
                    rows[rowIndex, 3] = targetObjects[i].DefinitionUuid;
                    rows[rowIndex, 4] = targetObjects[i].Priority;
                    rows[rowIndex, 5] = targetObjects[i].SourceUuid;
                    rows[rowIndex, 6] = targetObjects[i].Created;
                    rowIndex++;
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsAssignmentResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.assignment";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectAssignmentEntity.AssigneeUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectAssignmentEntity.MemberUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectAssignmentEntity.AddTimestamp),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectAssignmentEntity.AddSourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectAssignmentEntity.RemoveTimestamp),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectAssignmentEntity.RemoveSourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectAssignmentEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectAssignmentEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QueryAssignmentsByAssigneeUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }

                if (rowCount > 0)
                {
                    var rows = new object[rowCount, result.Columns.Length];
                    var rowIndex = 0;

                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                        if (!contentEntities.IsNullOrEmpty())
                        {
                            for (var j = 0; j < contentEntities.Count(); j++)
                            {
                                rows[rowIndex, 0] = contentEntities[j].AssigneeUuid;
                                rows[rowIndex, 1] = contentEntities[j].MemberUuid;
                                rows[rowIndex, 2] = contentEntities[j].AddTimestamp;
                                rows[rowIndex, 3] = contentEntities[j].AddSourceUuid;
                                rows[rowIndex, 4] = contentEntities[j].RemoveTimestamp;
                                rows[rowIndex, 5] = contentEntities[j].RemoveSourceUuid;
                                rows[rowIndex, 6] = contentEntities[j].Created;
                                rowIndex++;
                            }
                        }
                    }

                    result.Rows = rows;
                }
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsBooleanResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.boolean";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectBooleanEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectBooleanEntity.Value),
                DataType = TrakHoundQueryDataType.Boolean
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectBooleanEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectBooleanEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntity = collection.Objects.QueryBooleanByObjectUuid(targetObjects[i].Uuid);
                    if (contentEntity != null)
                    {
                        rows[rowIndex, 0] = contentEntity.ObjectUuid;
                        rows[rowIndex, 1] = contentEntity.Value;
                        rows[rowIndex, 2] = contentEntity.SourceUuid;
                        rows[rowIndex, 3] = contentEntity.Created;
                        rowIndex++;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsDurationResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.duration";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectDurationEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectDurationEntity.Value),
                DataType = TrakHoundQueryDataType.Duration
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectDurationEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectDurationEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntity = collection.Objects.QueryDurationByObjectUuid(targetObjects[i].Uuid);
                    if (contentEntity != null)
                    {
                        rows[rowIndex, 0] = contentEntity.ObjectUuid;
                        rows[rowIndex, 1] = contentEntity.Value;
                        rows[rowIndex, 2] = contentEntity.SourceUuid;
                        rows[rowIndex, 3] = contentEntity.Created;
                        rowIndex++;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsEventResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.event";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEventEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEventEntity.TargetUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEventEntity.Timestamp),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEventEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectEventEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectEventEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QueryEventsByObjectUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }

                if (rowCount > 0)
                {
                    var rows = new object[rowCount, result.Columns.Length];
                    var rowIndex = 0;

                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                        if (!contentEntities.IsNullOrEmpty())
                        {
                            for (var j = 0; j < contentEntities.Count(); j++)
                            {
                                rows[rowIndex, 0] = contentEntities[j].ObjectUuid;
                                rows[rowIndex, 1] = contentEntities[j].TargetUuid;
                                rows[rowIndex, 2] = contentEntities[j].Timestamp;
                                rows[rowIndex, 3] = contentEntities[j].SourceUuid;
                                rows[rowIndex, 4] = contentEntities[j].Created;
                                rowIndex++;
                            }
                        }
                    }

                    result.Rows = rows;
                }
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsGroupResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.group";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectGroupEntity.GroupUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectGroupEntity.MemberUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectGroupEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectGroupEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectGroupEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetContentEntities = collection.Objects.QueryGroupsByGroupUuid(targetObject.Uuid);
                    if (!targetContentEntities.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetContentEntities);
                        rowCount += targetContentEntities.Count();
                    }
                }

                if (rowCount > 0)
                {
                    var rows = new object[rowCount, result.Columns.Length];
                    var rowIndex = 0;

                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                        if (!contentEntities.IsNullOrEmpty())
                        {
                            for (var j = 0; j < contentEntities.Count(); j++)
                            {
                                rows[rowIndex, 0] = contentEntities[j].GroupUuid;
                                rows[rowIndex, 1] = contentEntities[j].MemberUuid;
                                rows[rowIndex, 2] = contentEntities[j].SourceUuid;
                                rows[rowIndex, 3] = contentEntities[j].Created;
                                rowIndex++;
                            }
                        }
                    }

                    result.Rows = rows;
                }
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsHashResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.hash";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectHashEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectHashEntity.Key),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectHashEntity.Value),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectHashEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectHashEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectHashEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetContentEntities = collection.Objects.QueryHashesByObjectUuid(targetObject.Uuid);
                    if (!targetContentEntities.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetContentEntities);
                        rowCount += targetContentEntities.Count();
                    }
                }

                if (rowCount > 0)
                {
                    var rows = new object[rowCount, result.Columns.Length];
                    var rowIndex = 0;

                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                        if (!contentEntities.IsNullOrEmpty())
                        {
                            for (var j = 0; j < contentEntities.Count(); j++)
                            {
                                rows[rowIndex, 0] = contentEntities[j].ObjectUuid;
                                rows[rowIndex, 1] = contentEntities[j].Key;
                                rows[rowIndex, 2] = contentEntities[j].Value;
                                rows[rowIndex, 3] = contentEntities[j].SourceUuid;
                                rows[rowIndex, 4] = contentEntities[j].Created;
                                rowIndex++;
                            }
                        }
                    }

                    result.Rows = rows;
                }
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsLogResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.log";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectLogEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectLogEntity.LogLevel),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectLogEntity.Message),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectLogEntity.Code),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectLogEntity.Timestamp),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectLogEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectLogEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectLogEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QueryLogsByObjectUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }


                var rows = new object[rowCount, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                    if (!contentEntities.IsNullOrEmpty())
                    {
                        for (var j = 0; j < contentEntities.Count(); j++)
                        {
                            rows[rowIndex, 0] = contentEntities[j].ObjectUuid;
                            rows[rowIndex, 1] = contentEntities[j].LogLevel;
                            rows[rowIndex, 2] = contentEntities[j].Message;
                            rows[rowIndex, 3] = contentEntities[j].Code;
                            rows[rowIndex, 4] = contentEntities[j].Timestamp;
                            rows[rowIndex, 5] = contentEntities[j].SourceUuid;
                            rows[rowIndex, 6] = contentEntities[j].Created;
                            rowIndex++;
                        }
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsMessageResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.message";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.Uuid),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.Topic),
                DataType = TrakHoundQueryDataType.String
            });
            //columns.Add(new TrakHoundQueryColumnDefinition
            //{
            //    Name = nameof(TrakHoundObjectMessageEntity.Content),
            //    DataType = TrakHoundQueryDataType.Bytes
            //});
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.ContentType),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.Retain),
                DataType = TrakHoundQueryDataType.Boolean
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.Qos),
                DataType = TrakHoundQueryDataType.Number
            });
            //columns.Add(new TrakHoundQueryColumnDefinition
            //{
            //    Name = nameof(TrakHoundObjectMessageEntity.Timestamp),
            //    DataType = TrakHoundQueryDataType.Timestamp
            //});
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectMessageEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var observationEntities = new ListDictionary<string, ITrakHoundObjectMessageEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QueryMessagesByObjectUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        observationEntities.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }


                var rows = new object[rowCount, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntities = observationEntities.Get(targetObjects[i].Uuid)?.ToArray();
                    if (!contentEntities.IsNullOrEmpty())
                    {
                        for (var j = 0; j < contentEntities.Count(); j++)
                        {
                            rows[rowIndex, 0] = contentEntities[j].Uuid;
                            rows[rowIndex, 1] = contentEntities[j].ObjectUuid;
                            rows[rowIndex, 2] = contentEntities[j].Topic;
                            rows[rowIndex, 3] = contentEntities[j].ContentType;
                            rows[rowIndex, 4] = contentEntities[j].Retain;
                            rows[rowIndex, 5] = contentEntities[j].Qos;
                            rows[rowIndex, 6] = contentEntities[j].SourceUuid;
                            rows[rowIndex, 7] = contentEntities[j].Created;
                            rowIndex++;

                            //rows[rowIndex, 0] = contentEntities[j].Uuid;
                            //rows[rowIndex, 1] = contentEntities[j].ObjectUuid;
                            ////rows[rowIndex, 2] = TrakHoundObjectMessageEntity.CreateBase64String(contentEntities[j].Content);
                            //rows[rowIndex, 3] = contentEntities[j].ContentType;
                            //rows[rowIndex, 4] = contentEntities[j].Retain;
                            //rows[rowIndex, 5] = contentEntities[j].QoS;
                            //rows[rowIndex, 6] = contentEntities[j].Timestamp;
                            //rows[rowIndex, 7] = contentEntities[j].SourceUuid;
                            //rows[rowIndex, 8] = contentEntities[j].Created;
                            //rowIndex++;
                        }
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsNumberResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.number";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectNumberEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectNumberEntity.Value),
                DataType = TrakHoundQueryDataType.Number
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectNumberEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectNumberEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntity = collection.Objects.QueryNumberByObjectUuid(targetObjects[i].Uuid);
                    if (contentEntity != null)
                    {
                        rows[rowIndex, 0] = contentEntity.ObjectUuid;
                        rows[rowIndex, 1] = contentEntity.Value;
                        rows[rowIndex, 2] = contentEntity.SourceUuid;
                        rows[rowIndex, 3] = contentEntity.Created;
                        rowIndex++;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsObservationResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.observation";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.Value),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.DataType),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.BatchId),
                DataType = TrakHoundQueryDataType.Number
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.Sequence),
                DataType = TrakHoundQueryDataType.Number
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.Timestamp),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectObservationEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectObservationEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QueryObservationsByObjectUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }

                if (rowCount > 0)
                {
                    var rows = new object[rowCount, result.Columns.Length];
                    var rowIndex = 0;

                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                        if (!contentEntities.IsNullOrEmpty())
                        {
                            for (var j = 0; j < contentEntities.Count(); j++)
                            {
                                rows[rowIndex, 0] = contentEntities[j].ObjectUuid;
                                rows[rowIndex, 1] = contentEntities[j].Value;
                                rows[rowIndex, 2] = contentEntities[j].DataType;
                                rows[rowIndex, 3] = contentEntities[j].BatchId;
                                rows[rowIndex, 4] = contentEntities[j].Sequence;
                                rows[rowIndex, 5] = contentEntities[j].Timestamp;
                                rows[rowIndex, 6] = contentEntities[j].SourceUuid;
                                rows[rowIndex, 7] = contentEntities[j].Created;
                                rowIndex++;
                            }
                        }
                    }

                    result.Rows = rows;
                }
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsSetResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.set";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectSetEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectSetEntity.Value),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectSetEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectSetEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();

            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectSetEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QuerySetsByObjectUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }


                var rows = new object[rowCount, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                    if (!contentEntities.IsNullOrEmpty())
                    {
                        for (var j = 0; j < contentEntities.Count(); j++)
                        {
                            rows[rowIndex, 0] = contentEntities[j].ObjectUuid;
                            rows[rowIndex, 1] = contentEntities[j].Value;
                            rows[rowIndex, 2] = contentEntities[j].SourceUuid;
                            rows[rowIndex, 3] = contentEntities[j].Created;
                            rowIndex++;
                        }
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsStateResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.state";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStateEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStateEntity.DefinitionUuid),
                DataType = TrakHoundQueryDataType.Definition
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStateEntity.TTL),
                DataType = TrakHoundQueryDataType.Number
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStateEntity.Timestamp),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStateEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStateEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectStateEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QueryStatesByObjectUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }


                var rows = new object[rowCount, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                    if (!contentEntities.IsNullOrEmpty())
                    {
                        for (var j = 0; j < contentEntities.Count(); j++)
                        {
                            rows[rowIndex, 0] = contentEntities[j].ObjectUuid;
                            rows[rowIndex, 1] = contentEntities[j].DefinitionUuid;
                            rows[rowIndex, 2] = contentEntities[j].TTL;
                            rows[rowIndex, 3] = contentEntities[j].Timestamp;
                            rows[rowIndex, 4] = contentEntities[j].SourceUuid;
                            rows[rowIndex, 5] = contentEntities[j].Created;
                            rowIndex++;
                        }
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsStatisticResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.statistic";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.Value),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.DataType),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.TimeRangeStart),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.TimeRangeEnd),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.Timestamp),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStatisticEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rowCount = 0;
                var entitiesByTarget = new ListDictionary<string, ITrakHoundObjectStatisticEntity>();
                foreach (var targetObject in targetObjects)
                {
                    var targetObservations = collection.Objects.QueryStatisticsByObjectUuid(targetObject.Uuid);
                    if (!targetObservations.IsNullOrEmpty())
                    {
                        entitiesByTarget.Add(targetObject.Uuid, targetObservations);
                        rowCount += targetObservations.Count();
                    }
                }

                if (rowCount > 0)
                {
                    var rows = new object[rowCount, result.Columns.Length];
                    var rowIndex = 0;

                    for (var i = 0; i < targetObjects.Length; i++)
                    {
                        var contentEntities = entitiesByTarget.Get(targetObjects[i].Uuid)?.ToArray();
                        if (!contentEntities.IsNullOrEmpty())
                        {
                            for (var j = 0; j < contentEntities.Count(); j++)
                            {
                                rows[rowIndex, 0] = contentEntities[j].ObjectUuid;
                                rows[rowIndex, 1] = contentEntities[j].Value;
                                rows[rowIndex, 2] = contentEntities[j].DataType;
                                rows[rowIndex, 3] = contentEntities[j].TimeRangeStart;
                                rows[rowIndex, 4] = contentEntities[j].TimeRangeEnd;
                                rows[rowIndex, 5] = contentEntities[j].Timestamp;
                                rows[rowIndex, 6] = contentEntities[j].SourceUuid;
                                rows[rowIndex, 7] = contentEntities[j].Created;
                                rowIndex++;
                            }
                        }
                    }

                    result.Rows = rows;
                }
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsStringResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.string";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStringEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStringEntity.Value),
                DataType = TrakHoundQueryDataType.String
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStringEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectStringEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntity = collection.Objects.QueryStringByObjectUuid(targetObjects[i].Uuid);
                    if (contentEntity != null)
                    {
                        rows[rowIndex, 0] = contentEntity.ObjectUuid;
                        rows[rowIndex, 1] = contentEntity.Value;
                        rows[rowIndex, 2] = contentEntity.SourceUuid;
                        rows[rowIndex, 3] = contentEntity.Created;
                        rowIndex++;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsTimeRangeResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.time-range";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimeRangeEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimeRangeEntity.Start),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimeRangeEntity.End),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimeRangeEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimeRangeEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntity = collection.Objects.QueryTimeRangeByObjectUuid(targetObjects[i].Uuid);
                    if (contentEntity != null)
                    {
                        rows[rowIndex, 0] = contentEntity.ObjectUuid;
                        rows[rowIndex, 1] = contentEntity.Start;
                        rows[rowIndex, 2] = contentEntity.End;
                        rows[rowIndex, 3] = contentEntity.SourceUuid;
                        rows[rowIndex, 4] = contentEntity.Created;
                        rowIndex++;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsTimestampResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.timestamp";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimestampEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimestampEntity.Value),
                DataType = TrakHoundQueryDataType.Timestamp
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimestampEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectTimestampEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntity = collection.Objects.QueryTimestampByObjectUuid(targetObjects[i].Uuid);
                    if (contentEntity != null)
                    {
                        rows[rowIndex, 0] = contentEntity.ObjectUuid;
                        rows[rowIndex, 1] = contentEntity.Value;
                        rows[rowIndex, 2] = contentEntity.SourceUuid;
                        rows[rowIndex, 3] = contentEntity.Created;
                        rowIndex++;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        public static TrakHoundQueryResult CreateObjectsVocabularyResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();
            result.Schema = "trakhound.entities.objects.vocabulary";

            var columns = new List<TrakHoundQueryColumnDefinition>();
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectVocabularyEntity.ObjectUuid),
                DataType = TrakHoundQueryDataType.Object
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectVocabularyEntity.DefinitionUuid),
                DataType = TrakHoundQueryDataType.Definition
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectVocabularyEntity.SourceUuid),
                DataType = TrakHoundQueryDataType.Source
            });
            columns.Add(new TrakHoundQueryColumnDefinition
            {
                Name = nameof(TrakHoundObjectVocabularyEntity.Created),
                DataType = TrakHoundQueryDataType.Timestamp
            });

            result.Columns = columns.ToArray();


            if (!targetObjects.IsNullOrEmpty())
            {
                var rows = new object[targetObjects.Length, result.Columns.Length];
                var rowIndex = 0;

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    var contentEntity = collection.Objects.QueryVocabularyByObjectUuid(targetObjects[i].Uuid);
                    if (contentEntity != null)
                    {
                        rows[rowIndex, 0] = contentEntity.ObjectUuid;
                        rows[rowIndex, 1] = contentEntity.DefinitionUuid;
                        rows[rowIndex, 2] = contentEntity.SourceUuid;
                        rows[rowIndex, 3] = contentEntity.Created;
                        rowIndex++;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }
    }
}
