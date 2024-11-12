// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public class TrakHoundObjectContentTypeDictionary
    {
        private readonly ListDictionary<TrakHoundObjectContentType, ITrakHoundObjectEntity> _objects = new ListDictionary<TrakHoundObjectContentType, ITrakHoundObjectEntity>();


        public IEnumerable<TrakHoundObjectContentType> ContentTypes => _objects.Keys;


        public void Add(ITrakHoundObjectEntity entity)
        {
            if (entity != null)
            {
                var contentType = entity.ContentType.ConvertEnum<TrakHoundObjectContentType>();
                _objects.Add(contentType, entity);
            }
        }

        public void Add(IEnumerable<ITrakHoundObjectEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    var contentType = entity.ContentType.ConvertEnum<TrakHoundObjectContentType>();
                    _objects.Add(contentType, entity);
                }
            }
        }

        public IEnumerable<ITrakHoundObjectEntity> Get(TrakHoundObjectContentType contentType)
        {
            return _objects.Get(contentType);
        }
    }
}
