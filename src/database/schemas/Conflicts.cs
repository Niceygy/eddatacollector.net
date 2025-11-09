namespace net.niceygy.eddatacollector.database.schemas
{
    public class Conflict
    {
        public required string system_name { get; set; }
        public required Power first_place { get; set; }
        public required Power second_place { get; set; }
    }
}