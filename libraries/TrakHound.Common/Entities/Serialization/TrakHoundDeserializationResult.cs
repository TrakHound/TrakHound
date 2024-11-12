// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Serialization
{
    public struct TrakHoundDeserializationResult<TModel>
    {
        public string ObjectUuid { get; set; }

        public TModel Model { get; set; }


        public TrakHoundDeserializationResult(string objectUuid, TModel model)
        {
            ObjectUuid = objectUuid;
            Model = model;
        }
    }
}
