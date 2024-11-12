// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Controllers.Http.Responses;

namespace TrakHound.Controllers.Http
{
    public class ObjectArrayResponse : IJsonArrayResponse 
    {
        [JsonArrayProperty("ID", "The Identifier that identifies what this Object represents")]
        public string Id { get; set; }

        [JsonArrayProperty("Version", "The version of the Object")]
        public string Version { get; set; }

        [JsonArrayProperty("Name", "he friendly name of the Object. This is used when constucting the Name Path.")]
        public string Name { get; set; }

        [JsonArrayProperty("Content Type", "The type of content that this Object represents. This must be one of the predefined Object ContentTypes.")]
        public string ContentType { get; set; }

        [JsonArrayProperty("Definition UUID", "The UUID of the Definition Entity that describes this Object")]
        public string DefinitionUuid { get; set; }

        [JsonArrayProperty("Parent UUID", "The UUID of the Parent Object Entity. This is used to create the Object's hierarchy when read as a Model.")]
        public string ParentUuid { get; set; }

        [JsonArrayProperty("Author UUID", "The UUID of the Author Entity that created this Entity")]
        public string AuthorUuid { get; set; }

        [JsonArrayProperty("Created", "The timestamp in UNIX Ticks (1 / 10,000) of when the Entity was created.")]
        public long Created { get; set; }
    }
}
