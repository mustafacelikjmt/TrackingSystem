using TrackingSystemWeb.Hubs;
using Core.Models;
using Service.JsonService;

namespace TrackingSystemWeb.BackgroundServices
{
    public class MqttClientAPI : BackgroundService
    {
        private readonly JsonDeserialize _jsonDeserialize;
        private readonly LocationHub _locationHub;

        public MqttClientAPI(JsonDeserialize jsonDeserialize, LocationHub locationHub)
        {
            _jsonDeserialize = jsonDeserialize;
            _locationHub = locationHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{nameof(MqttClientAPI)} Service started...");
            await _jsonDeserialize.GetMqttMessage("#");
            _jsonDeserialize.MessageReceivedLocation += _jsonDeserialize_Location;
        }

        private void _jsonDeserialize_Location(object sender, LocationModel e)
        {
            _locationHub.SendName(e);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(MqttClientAPI)} Service stopped...");
        }
    }
}