using System.Threading.Tasks;
using TrakHound;
using TrakHound.Api;
using TrakHound.Requests;

public class Controller : TrakHoundApiController
{
    private const string _messagesPath = "TrakHound:/.scan-select/requests";


    class MessageData
    {
        public string TargetType { get; set; }
        public string Target { get; set; }
        public string SourceType { get; set; }
        public string Source { get; set; }
        public string DataType { get; set; }
        public string Data { get; set; }
    }

    protected override void AfterClientSet()
    {
        Client.Entities.PublishMessageMapping(_messagesPath, "localhost", "trakhound/scan-select/requests", "application/json", false, 0, false);
    }


    [TrakHoundApiSubscribe("{targetType}/{target}/listen")]
    public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Listen(
        [FromRoute] string targetType,
        [FromRoute] string target
        )
    {
        if (!string.IsNullOrEmpty(targetType) && !string.IsNullOrEmpty(target))
        {
            var messageConsumer = await Client.Entities.SubscribeMessageContent(_messagesPath);
            if (messageConsumer != null)
            {
                var outputConsumer = new TrakHoundConsumer<TrakHoundMessage, TrakHoundApiResponse>(messageConsumer);
                outputConsumer.OnReceived = (msg) =>
                {
                    var json = msg.GetContentUtf8String();
                    var messageData = Json.Convert<MessageData>(json);
                    if (messageData != null && messageData.TargetType.ToLower() == targetType.ToLower() && messageData.Target == target)
                    {
                        return new TrakHoundApiResponse(200, json.ToUtf8Bytes(), "application/json");
                    }
                    else
                    {
                        return default;
                    }
                };
                return outputConsumer;
            }
        }

        return null;
    }

    [TrakHoundApiPublish("{targetType}/{target}/send")]
    public async Task<TrakHoundApiResponse> Send(
        [FromRoute] string targetType,
        [FromRoute] string target,
        [FromQuery] string sourceType,
        [FromQuery] string source,
        [FromQuery] string dataType,
        [FromBody] string data
        )
    {
        if (!string.IsNullOrEmpty(targetType) && !string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(sourceType) && !string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(dataType) && !string.IsNullOrEmpty(data))
        {
            var messageData = new MessageData();
            messageData.TargetType = targetType;
            messageData.Target = target;
            messageData.SourceType = sourceType;
            messageData.Source = source;
            messageData.DataType = dataType;
            messageData.Data = data;

            if (await Client.Entities.PublishMessageContent(_messagesPath, messageData.ToJson()))
            {
                return Created();
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
