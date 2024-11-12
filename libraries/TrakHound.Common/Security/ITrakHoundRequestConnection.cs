// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Security
{
    public interface ITrakHoundRequestConnection
    {
        string Type { get; }

        string Address { get; }
    }

    public class TrakHoundRequestConnection : ITrakHoundRequestConnection
    {
        public string Type { get; set; }

        public string Address { get; set; }


        public TrakHoundRequestConnection(string type, string address)
        {
            Type = type;
            Address = address;
        }
    }
}
