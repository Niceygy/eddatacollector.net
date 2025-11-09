namespace net.niceygy.eddatacollector.handlers
{
    using Semver;
    using net.niceygy.eddatacollector.schemas.reused;
    static class ValidMessage
    {
        public static readonly string MIN_GAME_VERSION = "4.2.1.2";
        public static readonly SemVersion MIN_ACCEPTED_VERSION = SemVersion.Parse("4.2.1.2");
        public static readonly string[] REJECTED_SENDERS = [
            "GameGlass",
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
        public static bool Check(Header data)
        {
            bool result = true;

            result = result && CheckTimeGap(data.gatewayTimestamp, 5 * 60);

            result = result && CheckSender(data.softwareName);

            result = result && CheckGameVersion(data.gameversion);



            return result;
        }

        private static bool CheckTimeGap(DateTime time, int acceptableDelaySeconds)
        {
            var now = DateTime.Now;
            TimeSpan delay = now - time;
            return acceptableDelaySeconds >= delay.Seconds;
        }

        private static bool CheckSender(string SenderString)
        {
            return !REJECTED_SENDERS.Contains(SenderString);
        }

        private static bool CheckName(string SenderString)
        {
            return !REJECTED_NAMES.Contains(SenderString);
        }

        private static bool CheckGameVersion(string ver)
        {
            // var version = SemVersion.Parse(ver);
            // MIN_ACCEPTED_VERSION.ComparePrecedenceTo(version);
            return ver == MIN_GAME_VERSION;
        }
    }
}