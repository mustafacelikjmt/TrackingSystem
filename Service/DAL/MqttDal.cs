using Core.Models;
using Service.MqttMessageManager;

namespace Service.DAL
{
    public class MqttDal
    {
        private readonly MqttClientService _mqttClientService;

        public event EventHandler<MqttMessageModel> MessageReceived;

        public MqttDal(MqttClientService mqttClientService)
        {
            _mqttClientService = mqttClientService;
        }

        public async Task GetMqttMessage(string topic) //client ten gelen mesajı alır
        {
            await _mqttClientService.SubscribeAsync(topic);
            _mqttClientService.MessageReceived();
            _mqttClientService.OnMessageReceived += MqttService_OnMessageReceived;
        }

        private void MqttService_OnMessageReceived(object sender, MqttMessageModel message) //event ile programa gönderir.
        {
            MessageReceived?.Invoke(this, message);
        }

        public async Task PostMqttMessage(string topic, string payload) //event ile mqtt ye gönderir
        {
        }
    }
}