using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrakHound;
using TrakHound.Api;
using TrakHound.Requests;
using TrakHound.Serialization;

public class Controller : TrakHoundApiController
{
    private const string _directoryName = "Notes";

    [TrakHoundObject]
    class NoteModel
    {
        [TrakHoundName]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [TrakHoundPath]
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [TrakHoundString]
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [TrakHoundReference]
        [JsonPropertyName("author")]
        public string Author { get; set; }

        [TrakHoundTimestamp]
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }


    [TrakHoundApiQuery]
    public async Task<TrakHoundApiResponse> Query([FromQuery] string basePath)
    {
        if (!string.IsNullOrEmpty(basePath))
        {
            var path = TrakHoundPath.Combine(basePath, _directoryName, "*");
            var models = await Client.Entities.Get<NoteModel>(path);
            if (!models.IsNullOrEmpty())
            {
                return Ok(models);
            }
            else
            {
                return NotFound();
            }
        }
        else
        {
            return BadRequest();
        }
    }

    [TrakHoundApiPublish]
    public async Task<TrakHoundApiResponse> Publish(
        [FromQuery] string basePath,
        [FromBody] string text,
        [FromQuery] string authorPath = null,
        [FromQuery] int noteId = 0,
        [FromQuery] DateTime? created = null
        )
    {
        if (!string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(text))
        {
            string path;

            if (noteId > 0)
            {
                path = TrakHoundPath.Combine(basePath, _directoryName, noteId.ToString());
            }
            else
            {
                var sequenceId = $"NOTES:{basePath}".ToLower().ToSHA256Hash();
                var nextNodeId = (await Client.System.Sequences.Publish(sequenceId, 1)).Value;
                if (nextNodeId < 1) nextNodeId = 1;

                path = TrakHoundPath.Combine(basePath, _directoryName, nextNodeId.ToString());
            }

            var model = new NoteModel();
            model.Path = path;
            model.Text = text;
            model.Author = authorPath;
            model.Created = created.HasValue ? created.Value : DateTime.Now;

            if (await Client.Entities.Publish(model))
            {
                // Return Note ID
                return Created(TrakHoundPath.GetObject(path));
            }
            else
            {
                return InternalError();
            }
        }
        else
        {
            return BadRequest();
        }
    }
}
