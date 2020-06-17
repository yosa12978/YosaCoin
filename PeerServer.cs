using System;
using System.Text;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace YosaCoin
{
  public class PeerServer: WebSocketBehavior
    {
        WebSocketServer wss = null;

        public void Start()
        {
            wss = new WebSocketServer($"ws://{Program.Addr}:{Program.prt}");
            wss.AddWebSocketService<PeerServer>("/Blockchain");
            wss.Start();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data == "Client_Connected")
            {
                Console.WriteLine(e.Data);
                Send("Connected_To_Server");
            }
            else
            {
                Block block = JsonConvert.DeserializeObject<Block>(e.Data);
                block.previousHash = Program.yosaCoin.getLatest().hash;
                foreach(Transaction transaction in block.data)
                    transaction.receiver = Program.username;
                Program.yosaCoin.AddBlock(block);
                Start();
            }
        }
    }
}