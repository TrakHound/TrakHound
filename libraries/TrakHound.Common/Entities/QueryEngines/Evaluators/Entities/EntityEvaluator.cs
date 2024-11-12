// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;

namespace TrakHound.Entities.QueryEngines.Evaluators.Entities
{
    public abstract class EntityEvaluator
    {
        public TrakHoundObjectContentType ContentType { get; set; }

        public ITrakHoundObjectEntityModel TargetObject { get; set; }

        public ITrakHoundObjectEntityModel ConditionObject { get; set; }

        public QueryConditionGroup ConditionGroup { get; set; }

        public QueryCondition Condition { get; set; }


        public static async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundSystemEntitiesClient client, IEnumerable<IEvaluator> evaluators)
        {
            var results = new List<IEvaluatorResult>();

            if (client != null && !evaluators.IsNullOrEmpty())
            {
                results.AddRange(await ObjectAssignmentEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectAssignmentEvaluator>()));
                results.AddRange(await ObjectBooleanEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectBooleanEvaluator>()));
                results.AddRange(await ObjectDurationEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectDurationEvaluator>()));
                results.AddRange(await ObjectEventEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectEventEvaluator>()));
                results.AddRange(await ObjectFeedEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectFeedEvaluator>()));
                results.AddRange(await ObjectGroupEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectGroupEvaluator>()));
                results.AddRange(await ObjectHashEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectHashEvaluator>()));
                results.AddRange(await ObjectNumberEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectNumberEvaluator>()));
                results.AddRange(await ObjectObservationEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectObservationEvaluator>()));
                results.AddRange(await ObjectQueueEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectQueueEvaluator>()));
                results.AddRange(await ObjectReferenceEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectReferenceEvaluator>()));
                results.AddRange(await ObjectSetEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectSetEvaluator>()));
                results.AddRange(await ObjectStatusEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectStatusEvaluator>()));
                results.AddRange(await ObjectStringEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectStringEvaluator>()));
                results.AddRange(await ObjectTimestampEvaluator.Evaluate(statement, client, evaluators.OfType<ObjectTimestampEvaluator>()));
            }

            return results;
        }

        public static IEvaluator Create(ConditionObjectQuery query, ITrakHoundObjectEntityModel conditionObject)
        {
            if (conditionObject != null)
            {
                IEvaluator evaluator = null;

                switch (conditionObject.ContentType.ConvertEnum<TrakHoundObjectContentType>())
                {
                    case TrakHoundObjectContentType.Assignment: evaluator = new ObjectAssignmentEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Boolean: evaluator = new ObjectBooleanEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Duration: evaluator = new ObjectDurationEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Event: evaluator = new ObjectEventEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Feed: evaluator = new ObjectFeedEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Group: evaluator = new ObjectGroupEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Hash: evaluator = new ObjectHashEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Observation: evaluator = new ObjectObservationEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Number: evaluator = new ObjectNumberEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Queue: evaluator = new ObjectQueueEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Reference: evaluator = new ObjectReferenceEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Set: evaluator = new ObjectSetEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.State: evaluator = new ObjectStatusEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.String: evaluator = new ObjectStringEvaluator(query, conditionObject); break;
                    case TrakHoundObjectContentType.Timestamp: evaluator = new ObjectTimestampEvaluator(query, conditionObject); break;
                }

                return evaluator;
            }

            return null;
        }
    }
}
