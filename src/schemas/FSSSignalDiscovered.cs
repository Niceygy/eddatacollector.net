namespace net.niceygy.eddatacollector.schemas.FSSSignalDiscovered
{
    using Newtonsoft.Json;
    using net.niceygy.eddatacollector.schemas.reused;
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);


    public class Message
    {
        public required List<double> StarPos { get; set; }
        public required string StarSystem { get; set; }
        public long SystemAddress { get; set; }
        public required string @event { get; set; }
        public bool horizons { get; set; }
        public bool odyssey { get; set; }
        public required List<Signal> signals { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class FSSSignalMessage
    {
        [JsonProperty("$schemaRef")]
        public required string schemaRef { get; set; }
        public required Header header { get; set; }
        public required Message message { get; set; }
    }

    public class Signal
    {
        public required string SignalName { get; set; }
        public required string SignalType { get; set; }
        public DateTime timestamp { get; set; }
        public bool? IsStation { get; set; }
    }


}