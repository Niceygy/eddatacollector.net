using System.ComponentModel;
using Newtonsoft.Json;
using Serilog;

namespace net.niceygy.eddatacollector
{
    /// <summary>
    /// Class for response from ed-server-status.orerve.net
    /// </summary>
    public class EDStatusReponse
    {
        public required string status;
        public required string message;
        public int code;
        public required string product;
    }

    public static class EDStatus
    {
        /// <summary>
        /// Checks the game's status JSON to see if it is online.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsEliteOnline()
        {
            HttpClient client = new();
            var data = await client.GetAsync("https://ed-server-status.orerve.net");
            EDStatusReponse response = JsonConvert.DeserializeObject<EDStatusReponse>(await data.Content.ReadAsStringAsync()!)!;
            Log.Debug($"ED Status: {response.status}");
            return response.status == "Good";
        }
    }
}