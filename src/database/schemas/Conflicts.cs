namespace net.niceygy.eddatacollector.database.schemas
{
    public class Conflict
    {
        public required string system_name { get; set; }
        public required PowersInfo.Power first_place { get; set; }
        public required PowersInfo.Power second_place { get; set; }
    }
}