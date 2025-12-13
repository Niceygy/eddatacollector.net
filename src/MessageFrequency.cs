using WebSocketSharp;
using WebSocketSharp.Server;

namespace net.niceygy.eddatacollector.messageFrequency
{

    public class EDAMServer : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
        }

        protected override void OnOpen()
        {

            base.OnOpen();
        }

    }

    public static class messageFrequency
    {
        public static async Task Start()
        {
            
            var wssv = new WebSocketServer("wss://edam.niceygy.net/");
            wssv.AddWebSocketService<EDAMServer>("/wss");
            wssv.Start();

            
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
    enum MessageTypes
    {
        DOCK,
        JUMP,
        OTHER,
        BAD
    }


    class WSMessage
    {
        public MessageTypes type;
        public required string Software;
        public required string UploaderID;

    }
}