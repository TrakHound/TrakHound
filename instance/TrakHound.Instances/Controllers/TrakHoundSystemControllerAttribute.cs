using System;

namespace TrakHound.Http
{
    public class TrakHoundSystemControllerAttribute : Attribute
    {
        public string Id { get; set; }


        public TrakHoundSystemControllerAttribute(string id)
        {
            Id = id;
        }
    }
}
