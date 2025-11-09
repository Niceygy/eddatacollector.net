namespace net.niceygy.eddatacollector.database.schemas
{
    public class PowerData
    {
        public required string system_name { get; set; }
        public required SystemState state { get; set; }
        public required Power shortcode { get; set; }
        public float control_points { get; set; }
        public float points_change { get; set; }
    }
}