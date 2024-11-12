using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.ScheduledTasks
{
    public class ScheduledTaskConfiguration
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("functionId")]
        public string FunctionId { get; set; }

        [JsonPropertyName("schedule")]
        public string Schedule { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }
    }
}
