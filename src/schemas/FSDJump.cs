namespace net.niceygy.eddatacollector.schemas
{
    using System.Numerics;
    using Newtonsoft.Json;
    using net.niceygy.eddatacollector.schemas.reused;
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class PowerplayConflictEntry
    {
        public decimal ConflictProgress { get; set; }
        [JsonProperty(nameof(Power))]
        private string? _power { get; set; }

        [JsonIgnore]
        public PowersInfo.Power Power
        {
            get => PowersInfo.PowerShortCodes[_power!];
            set => _power = PowersInfo.PowerShortCodes.FirstOrDefault(x => x.Value == value).Key;
        }
    }

    public class FSDMessage
    {
        public required string Body { get; set; }
        public int BodyID { get; set; }
        public required string BodyType { get; set; }
        public BigInteger Population { get; set; }
        public required List<double> StarPos { get; set; }
        public required string StarSystem
        {
            get; // => StarSystem;
            set; // => value.Replace("'", ".");
        }
        public long SystemAddress { get; set; }
        public required string SystemAllegiance { get; set; }
        public required string SystemEconomy { get; set; }
        public required string SystemGovernment { get; set; }
        public required string SystemSecondEconomy { get; set; }
        public required string SystemSecurity { get; set; }
        //powerdata
        public string? ControllingPower { get; set; }
        [JsonProperty(nameof(Powers))]
        private List<string>? _powers { get; set; }

        [JsonIgnore]
        public List<PowersInfo.Power>? Powers
        {
            get => _powers?.Select(p => PowersInfo.PowerShortCodes[p]).ToList();
            set => _powers = value?.Select(p => PowersInfo.PowerShortCodes.FirstOrDefault(x => x.Value == p).Key).ToList();
        }

        public float? PowerplayStateControlProgress { get; set; }
        public float? PowerplayStateReinforcement { get; set; }
        public float? PowerplayStateUndermining { get; set; }


        [JsonProperty(nameof(PowerplayState))]
        public required string _PowerPlayState { get; set; }
        [JsonIgnore]
        public SystemStates.SystemState PowerplayState
        {
            get => SystemStates.ConversionTable.ContainsKey(_PowerPlayState) ? SystemStates.ConversionTable[_PowerPlayState] : SystemStates.SystemState.Unoccupied;
            set => _PowerPlayState = SystemStates.ConversionTable.FirstOrDefault(x => x.Value == value).Key;
        }


        public List<PowerplayConflictEntry>? PowerplayConflictProgress { get; set; }
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
        public required FSDMessage message { get; set; }
    }


}