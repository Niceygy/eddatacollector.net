namespace net.niceygy.eddatacollector.handlers
{
    using Serilog;
    using net.niceygy.eddatacollector.schemas.reused;
    static class MessageCheck
    {
        /// <summary>
        /// Oldest release that the program will accept
        /// </summary>
        private static readonly Version MINIMUM_GAME_VERSION = new("4.2.2.1");

        /// <summary>
        /// Latest game version that has appeared in EDDN messages.
        /// </summary>
        private static Version LATEST_SEEN_VERSION = MINIMUM_GAME_VERSION;

        /// <summary>
        /// Known well-behaved senders
        /// </summary>
        private static readonly string[] ACCEPTED_SENDERS = [
            "EDDiscovery",
            "E:D Market Connector [Windows]",
            "E:D Market Connector [Linux]",
            "EDO Materials Helper",
            "EDDLite",
            "EDDI"
        ];

        /// <summary>
        /// Known things that interfere with the code, 
        /// so are rejected
        /// </summary>
        private static readonly string[] REJECTED_NAMES = [
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

        /// <summary>
        /// Is the message header valid?
        /// </summary>
        /// <param name="data">Message Header</param>
        /// <returns>true if ok, false if not</returns>
        public static bool IsValid(Header data)
        {
            bool result = true;

            result = result && IsValidTimeGap(data.gatewayTimestamp, 5 * 60);

            result = result && IsValidCheckSender(data.softwareName);

            result = result && IsValidGameVersion(data.gameversion);

            return result;
        }

        /// <summary>
        /// Is the DateTime within acceptableDelaySeconds of now?
        /// </summary>
        /// <param name="time">Time to be checked</param>
        /// <param name="acceptableDelaySeconds">Seconds of accepted delay</param>
        /// <returns></returns>
        private static bool IsValidTimeGap(DateTime time, int acceptableDelaySeconds)
        {
            var now = DateTime.Now;
            TimeSpan delay = now - time;
            Log.Verbose($"CheckTimeStamp delay of {delay.TotalSeconds}s was {(acceptableDelaySeconds >= delay.Seconds ? "accepted." : "rejected.")}");
            return acceptableDelaySeconds >= delay.Seconds;
        }

        /// <summary>
        /// Is the sender whitelisted?
        /// </summary>
        /// <param name="SenderString">Sender's ID string</param>
        /// <returns></returns>
        private static bool IsValidCheckSender(string SenderString)
        {
            bool res = ACCEPTED_SENDERS.Contains(SenderString);
            Log.Verbose($"CheckSender {(res ? "accepted" : "rejected")} '{SenderString}'");
            return res;
        }

        public static bool IsNameBlocked(string SenderString)
        {
            return !REJECTED_NAMES.Contains(SenderString);
        }

        /// <summary>
        /// Is the game version above (or the same as)
        /// the MINIMUM_GAME_VERSION
        /// </summary>
        /// <param name="ver">Version string</param>
        /// <returns></returns>
        private static bool IsValidGameVersion(string ver)
        {
            try
            {
                Version incoming = new(ver);
                Log.Verbose($"Version {ver} {(incoming >= MINIMUM_GAME_VERSION ? "accepted" : "rejected")}");
                if (incoming > MINIMUM_GAME_VERSION && incoming > LATEST_SEEN_VERSION)
                {
                    Log.Information($"New game version seen: {incoming}");
                    LATEST_SEEN_VERSION = incoming;
                }
                return incoming >= MINIMUM_GAME_VERSION;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }
    }
}