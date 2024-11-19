// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public class TrakHoundAggregate
    {
        public string Target { get; set; }

        public double Value { get; set; }


        public TrakHoundAggregate(string target, double value)
        {
            Target = target;
            Value = value;
        }
    }
}
