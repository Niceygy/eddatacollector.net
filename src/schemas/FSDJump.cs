namespace net.niceygy.eddatacollector.schemas.FSDJump
{
    using System.Numerics;
    using Newtonsoft.Json;
    using net.niceygy.eddatacollector.schemas.reused;
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    

    public class Message
    {
        public required string Body { get; set; }
        public int BodyID { get; set; }
        public required string BodyType { get; set; }
        public BigInteger Population { get; set; }
        public required List<double> StarPos { get; set; }
        public required string StarSystem { get; set; }
        public long SystemAddress { get; set; }
        public required string SystemAllegiance { get; set; }
        public required string SystemEconomy { get; set; }
        public required string SystemGovernment { get; set; }
        public required string SystemSecondEconomy { get; set; }
        public required string SystemSecurity { get; set; }
        public required string @event { get; set; }
        public bool horizons { get; set; }
        public bool odyssey { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class FSDJumpMessage
    {
        [JsonProperty("$schemaRef")]
        public required string schemaRef { get; set; }
        public required Header header { get; set; }
        public required Message message { get; set; }
    }


}