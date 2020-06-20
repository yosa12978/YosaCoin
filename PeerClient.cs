using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using YosaCoin;

namespace YosaCoin
{
    public class PeerClient
    {
        IDictionary<string, WebSocket> wsDict = new Dictionary<string, WebSocket>();

        public void Connect(string url)
        {
            if (!wsDict.ContainsKey(url))
            {
                WebSocket ws = new WebSocket(url);
                ws.OnMessage += (sender, e) => 
                {
                    if (e.Data == "Connected_To_Server")
                        Console.WriteLine(e.Data);
                    else
                    {
                        BlockChain newChain = JsonConvert.DeserializeObject<BlockChain>(e.Data);
                        if (newChain.isValid() && newChain.Chain.Count > Program.yosaCoin.Chain.Count)
                        {
                            List<Transaction> newTransactions = new List<Transaction>();
                            newTransactions.AddRange(newChain.PendingTransactions);
                            newTransactions.AddRange(Program.yosaCoin.PendingTransactions);

                            newChain.PendingTransactions = newTransactions;
                            Program.yosaCoin = newChain;
                        }
                    }
                };
                ws.Connect();
                ws.Send("Client_Connected");
                ws.Send(JsonConvert.SerializeObject(Program.yosaCoin));
                wsDict.Add(url, ws);
            }
        }

        public void Send(string url, string data)
        {
            foreach (var item in wsDict)
            {
                if (item.Key == url)
                {
                    item.Value.Send(data);
                }
            }
        }

        public void Broadcast(string data)
        {
            foreach (var item in wsDict)
            {
                item.Value.Send(data);
            }
        }

        public IList<string> GetServers()
        {
            IList<string> servers = new List<string>();
            foreach (var item in wsDict)
            {
                servers.Add(item.Key);
            }
            return servers;
        }

        public void Close()
        {
            foreach (var item in wsDict)
            {
                item.Value.Close();
            }
        }
    }
}