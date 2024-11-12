// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities.Collections;

namespace TrakHound.Http
{
    public class TrakHoundHttpEntityResponse : TrakHoundJsonEntityCollection
    {
        public TrakHoundHttpEntityResponse() { }

        public TrakHoundHttpEntityResponse(TrakHoundEntityCollection collection) : base(collection) { }
    }
}
