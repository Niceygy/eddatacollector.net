namespace net.niceygy.eddatacollector
{
    public static class PowersInfo
    {
        /// <summary>
        /// All PowerPlay leaders and their shortcodes
        /// </summary>
        public enum Power
        {
            /// <summary>
            /// A. Lavigny-Duval
            /// </summary>
            ALD,
            /// <summary>
            /// Archon Delaine
            /// </summary>
            ARD,
            /// <summary>
            /// Aisling Duval
            /// </summary>
            ASD,
            /// <summary>
            /// Denton Patreus
            /// </summary>
            DPT,
            /// <summary>
            /// Edmund Mahon
            /// </summary>
            EMH,
            /// <summary>
            /// Felicia Winters
            /// </summary>
            FLW,
            /// <summary>
            /// Jerome Archer
            /// </summary>
            JRA,
            /// <summary>
            /// Li Yong-Rui
            /// </summary>
            LYR,
            /// <summary>
            /// Nakato Kaine
            /// </summary>
            NAK,
            /// <summary>
            /// Pranav Antal
            /// </summary>
            PRA,
            /// <summary>
            /// Yuri Grom
            /// </summary>
            YRG,
            /// <summary>
            /// Zemina Torval
            /// </summary>
            ZMT,
        }

        public static  readonly Dictionary<string, Power> PowerShortCodes = new()
        {
            {"A. Lavigny-Duval", Power.ALD },
            {"Archon Delaine", Power.ARD},
            {"Aisling Duval", Power.ASD},
            {"Denton Patreus", Power.DPT},
            {"Edmund Mahon", Power.EMH},
            {"Felicia Winters", Power.FLW},
            {"Jerome Archer", Power.JRA},
            {"Li Yong-Rui", Power.LYR},
            {"Nakato Kaine", Power.NAK},
            {"Pranav Antal", Power.PRA},
            {"Yuri Grom", Power.YRG},
            {"Zemina Torval", Power.ZMT},

        };
    }
}