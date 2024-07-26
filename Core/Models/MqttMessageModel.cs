namespace Core.Models
{
    public class MqttMessageModel : BaseEntity
    {
        public MqttMessageModel(string topic, string messagePayload)
        {
            Topic = topic;
            MessagePayload = messagePayload;
        }

        public string MessagePayload { get; set; }
        public string Topic { get; set; }
    }
}