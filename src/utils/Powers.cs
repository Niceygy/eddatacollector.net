using System.ComponentModel;
using System.Reflection;

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
            [Description("ALD")]
            ALD,
            /// <summary>
            /// Archon Delaine
            /// </summary>
            [Description("ARD")]
            ARD,
            /// <summary>
            /// Aisling Duval
            /// </summary>
            [Description("ASD")]
            ASD,
            /// <summary>
            /// Denton Patreus
            /// </summary>
            [Description("DPT")]
            DPT,
            /// <summary>
            /// Edmund Mahon
            /// </summary>
            [Description("EMH")]
            EMH,
            /// <summary>
            /// Felicia Winters
            /// </summary>
            [Description("FLW")]
            FLW,
            /// <summary>
            /// Jerome Archer
            /// </summary>
            [Description("JRA")]
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

        public static readonly Dictionary<string, Power> PowerShortCodes = new()
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

        public static string ToString(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}