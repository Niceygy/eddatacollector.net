namespace net.niceygy.eddatacollector.schemas
{
    using Newtonsoft.Json;
    using net.niceygy.eddatacollector.schemas.reused;
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class UnknownMessage
    {
        [JsonProperty("$schemaRef")]
        public required string schemaRef { get; set; }
        public required Header header { get; set; }
        public required dynamic message { get; set; }
    }



}