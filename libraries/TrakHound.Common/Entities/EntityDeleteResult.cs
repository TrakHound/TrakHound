// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public struct EntityDeleteResult
    {
        public string Target { get; set; }

        public long Count { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Target);
            }
        }


        public EntityDeleteResult(string target, long count)
        {
            Target = target;
            Count = count;
        }
    }
}
