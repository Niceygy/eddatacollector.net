namespace net.niceygy.eddatacollector.database.schemas
{
    public class StarSystem
    {
        public required string system_name { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float height { get; set; }
        public bool isanarchy { get; set; }
        public int frequency { get; set; }
    }
}