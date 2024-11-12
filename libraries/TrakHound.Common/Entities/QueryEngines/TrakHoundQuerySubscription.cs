// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.QueryEngines
{
    public struct TrakHoundQuerySubscription
    {
        public string EntityCategory { get; set; }

        public string EntityClass { get; set; }

        public string Target { get; set; }

        public TrakHoundQuerySubscriptionTargetType TargetType { get; set; }


        public TrakHoundQuerySubscription(string entityCategory, string entityClass, string target)
        {
            EntityCategory = entityCategory;
            EntityClass = entityClass;
            Target = target;
            TargetType = GetTargetType(entityCategory, entityClass);
        }


        public static TrakHoundQuerySubscription Create<TEntity>(string target) where TEntity : ITrakHoundEntity
        {
            var subscription = new TrakHoundQuerySubscription();
            subscription.EntityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            subscription.EntityClass = TrakHoundEntity.GetEntityClass<TEntity>();
            subscription.Target = target;
            subscription.TargetType = GetTargetType<TEntity>();
            return subscription;
        }


        public static TrakHoundQuerySubscriptionTargetType GetTargetType<TEntity>() where TEntity : ITrakHoundEntity
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            return GetTargetType(entityCategory, entityClass);
        }

        public static TrakHoundQuerySubscriptionTargetType GetTargetType(string entityCategory, string entityClass)
        {
            var eCategory = entityCategory.ConvertEnum<TrakHoundEntityCategory>();
            switch (eCategory)
            {
                case TrakHoundEntityCategory.Objects:
                    
                    var objectsClass = entityClass.ConvertEnum<TrakHoundObjectsEntityClass>();
                    switch (objectsClass)
                    {
                        case TrakHoundObjectsEntityClass.Object: return TrakHoundQuerySubscriptionTargetType.Expression;
                        default: return TrakHoundQuerySubscriptionTargetType.AssemblyUuid;
                    }
            }

            return TrakHoundQuerySubscriptionTargetType.Uuid;
        }
    }
}
