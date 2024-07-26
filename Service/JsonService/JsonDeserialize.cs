using Core.Models;
using Newtonsoft.Json;
using Service.DAL;

//using System.Text.Json;

namespace Service.JsonService
{
    public class JsonDeserialize
    {
        private readonly MqttDal _mqttDal;
        private LocationModel _locationModel;

        public event EventHandler<MqttMessageModel> MessageReceivedMqtt;

        public event EventHandler<LocationModel> MessageReceivedLocation;

        public JsonDeserialize(MqttDal mqttDal)
        {
            _mqttDal = mqttDal;
        }

        public async Task GetMqttMessage(string topic)
        {
            await _mqttDal.GetMqttMessage(topic);
            _mqttDal.MessageReceived += Deserialize;
        }

        private void Deserialize(object sender, MqttMessageModel model)
        {
            try
            {
                _locationModel = JsonConvert.DeserializeObject<LocationModel>(model.MessagePayload);
                //_locationModel = JsonSerializer.Deserialize<LocationModel>(model.MessagePayload); // bu system kütüphanesinin katı kuralları sebebiyle location nesnesine çevirmede sıkıntı oluyor ve boş nesne dönüyor.
                MessageReceivedLocation?.Invoke(this, _locationModel);
            }
            catch (Exception)
            {
                MessageReceivedMqtt?.Invoke(this, model);
            }
        }
    }
}