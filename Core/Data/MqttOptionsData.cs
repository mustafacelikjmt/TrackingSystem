namespace Core.Data
{
    public class MqttOptionsData
    {
        public string CertificatePath { get; set; } = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName, "Core\\Certificate.pfx");
        public string Password { get; set; } = "1010";
        public int Port { get; set; } = 8883;
        public string Server { get; set; } = "an726pjx0w8v9-ats.iot.eu-north-1.amazonaws.com";
    }
}