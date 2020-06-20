using System;
using System.Text;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;

namespace YosaCoin
{
  public class PeerServer: WebSocketBehavior
    {
        bool chainSynched = false;
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
                BlockChain chain = JsonConvert.DeserializeObject<BlockChain>(e.Data);

                if (chain.isValid() && chain.Chain.Count > Program.yosaCoin.Chain.Count)
                {
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(chain.PendingTransactions);
                    newTransactions.AddRange(Program.yosaCoin.PendingTransactions);

                    chain.PendingTransactions = newTransactions;
                    Program.yosaCoin = chain;
                }

                if (!chainSynched)
                {
                    Send(JsonConvert.SerializeObject(Program.yosaCoin));
                    chainSynched = true;
                }
            }
        }
    }
}