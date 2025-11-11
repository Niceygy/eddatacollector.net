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
        
    }
}