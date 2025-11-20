using System.ComponentModel.DataAnnotations.Schema;

namespace net.niceygy.eddatacollector.database.schemas
{
    public class PowerData
    {
        public required string system_name { get; set; }
        /*SystemStates.SystemState*/
        [NotMapped]
        private string? _state { get; set; }
        public required SystemStates.SystemState state 
        { 
            get { return SystemStates.ConversionTable[_state]; } 
            set { _state = SystemStates.ConversionTable.FirstOrDefault(x => x.Value == value).Key;} 
        }
        //PowersInfo.Power
        public required PowersInfo.Power shortcode { get; set; }
        public float? control_points { get; set; }
        public float? points_change { get; set; }
    }
}