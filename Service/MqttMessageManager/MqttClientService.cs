using MQTTnet;
using MQTTnet.Client;
using Core.Data;
using Core.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Service.MqttMessageManager
{
    public class MqttClientService
    {
        private readonly IMqttClient mqttClient;
        private readonly MqttClientOptions options;

        public MqttClientService()
        {
            var mqttOptionsData = new MqttOptionsData();
            var Certificate = new X509Certificate2(mqttOptionsData.CertificatePath, mqttOptionsData.Password);
            var Server = mqttOptionsData.Server;
            var Port = mqttOptionsData.Port;

            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            options = new MqttClientOptionsBuilder()
            .WithTcpServer(Server, Port)
            .WithTls(new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true,
                Certificates = new[] { Certificate }
            })
            .WithClientId("AspClient")
            .Build(); //mqtt options
        }

        public async Task SubscribeAsync(string topic)
        {
            await mqttClient.ConnectAsync(options);
            await mqttClient.SubscribeAsync(topic);
        }

        public async Task PublishAsync(string topic, string payload) //mesaj yolluyo.
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public event EventHandler<MqttMessageModel> OnMessageReceived;

        public void MessageReceived()
        {
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var messagePayload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var messageEvent = new MqttMessageModel(topic, messagePayload);
                OnMessageReceived?.Invoke(this, messageEvent);
                return Task.CompletedTask;
            };
        }
    }
}