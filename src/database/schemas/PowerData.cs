using System.ComponentModel.DataAnnotations.Schema;
using Serilog;

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
            get
            {
                try
                {
                    return _state != null ? SystemStates.ConversionTable[_state] : SystemStates.SystemState.Unoccupied;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return SystemStates.SystemState.Unoccupied;
                }
            }
            set
            {
                try
                {
                    _state = SystemStates.ConversionTable.FirstOrDefault(x => x.Value == value).Key;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
        }
        //PowersInfo.Power
        private string? _shortcode { get; set; }
        public PowersInfo.Power shortcode
        {
            get => _shortcode != null && PowersInfo.PowerShortCodes.ContainsKey(_shortcode)
                ? PowersInfo.PowerShortCodes[_shortcode]
                : default;
            set => _shortcode = PowersInfo.PowerShortCodes.FirstOrDefault(x => x.Value == value).Key;
        }
        private float _control_points;
        public float? control_points
        {
            get => _control_points;
            set => _control_points = value ?? 0;
        }
        private float _points_change { get; set; }
        public float? points_change
        {
            get => _points_change;
            set => _points_change = (float?)value ?? 0;
        }
    }
}