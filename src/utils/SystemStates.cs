namespace net.niceygy.eddatacollector
{
    public static class SystemStates
    {
        /// <summary>
        /// All powerplay system states
        /// </summary>
        public enum SystemState
        {
            Unoccupied,
            War,
            Exploited,
            Fortified,
            Stronghold
        }

        public static readonly Dictionary<string, SystemState> ConversionTable = new()
        {
            {"Unoccupied", SystemState.Unoccupied},
            {"War", SystemState.War},
            {"Exploited", SystemState.Exploited},
            {"Fortified", SystemState.Fortified},
            {"Stronghold", SystemState.Stronghold}
        };
    }
}