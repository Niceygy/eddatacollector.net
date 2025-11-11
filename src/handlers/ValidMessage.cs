namespace net.niceygy.eddatacollector.handlers
{
    using Semver;
    using Serilog;
    using net.niceygy.eddatacollector.schemas.reused;
    static class MessageCheck
    {
        public static readonly string MIN_GAME_VERSION = "4.2.2.0";
        private static readonly Version GAME_VER = new(MIN_GAME_VERSION);
        // public static readonly SemVersion MIN_ACCEPTED_VERSION = SemVersion.Parse(MIN_GAME_VERSION);
        public static readonly string[] ACCEPTED_SENDERS = [
            "EDDiscovery",
            "E:D Market Connector [Windows]",
            "E:D Market Connector [Linux]",
            "EDO Materials Helper",
            "EDDLite"
        ];

        public static readonly string[] REJECTED_NAMES = [
            "System Colonisation Ship",
            "Stronghold Carrier",
            "OnFootSettlement",
            "Colonisation",
            "$EXT_PANEL_ColonisationShip; [inactive]",
            "'$EXT_PANEL_ColonisationShip:#index=",
            "Carrier",
            "EXT_PANEL",
            "Construction Site",
            "$EXT_PANEL_ColonisationShip:#index=",
            "$EXT_PANEL_ColonisationShip:#index=1;",
            "$EXT_PANEL_ColonisationShip:#index=2;",
            "$EXT_PANEL_ColonisationShip:#index=3;",
        ];
        public static bool IsValid(Header data)
        {
            bool result = true;

            result = result && IsValidTimeGap(data.gatewayTimestamp, 5 * 60);

            result = result && IsValidCheckSender(data.softwareName);

            result = result && IsValidGameVersion(data.gameversion);

            return result;
        }

        private static bool IsValidTimeGap(DateTime time, int acceptableDelaySeconds)
        {
            var now = DateTime.Now;
            TimeSpan delay = now - time;
            Log.Debug($"CheckTimeStamp delay of {delay.TotalSeconds}s was {(acceptableDelaySeconds >= delay.Seconds ? "accepted." : "rejected.")}");
            return acceptableDelaySeconds >= delay.Seconds;
        }

        private static bool IsValidCheckSender(string SenderString)
        {
            bool res = ACCEPTED_SENDERS.Contains(SenderString);
            Log.Debug($"CheckSender {(res ? "accepted" : "rejected")} '{SenderString}'");
            return res;
        }

        private static bool CheckName(string SenderString)
        {
            return !REJECTED_NAMES.Contains(SenderString);
        }

        private static bool IsValidGameVersion(string ver)
        {
            try
            {
                Version incoming = new(ver);
                Log.Debug($"Version {ver} {(incoming >= GAME_VER ? "accepted" : "rejected")}");
                return incoming >= GAME_VER;
            } catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }
    }
}