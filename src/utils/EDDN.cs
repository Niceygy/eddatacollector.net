using System.ComponentModel;
using Newtonsoft.Json;

namespace net.niceygy.eddatacollector
{
    public class EDStatusReponse
    {
        public required string status;
        public required string message;
        public int code;
        public required string product;
    }
    public static class EDDN
    {
        public static async Task<EDStatusReponse> IsEDDNOnline()
        {
            HttpClient client = new();
            var data = await client.GetAsync("https://ed-server-status.orerve.net");
            EDStatusReponse response = JsonConvert.DeserializeObject<EDStatusReponse>(data.ToString())!;
            return response;
        }
    }
}