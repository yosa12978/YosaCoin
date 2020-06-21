using System.Drawing;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;


namespace YosaCoin
{
    public class Program
    {
        // Main Blockchain vars 
        public static string username;
        public static BlockChain yosaCoin;

        // To Exit Receive Task
        public static bool is_Active = true;

        // Vars for broadcast P2P connection  
        private static UdpClient udpClient = new UdpClient();
        private static IPEndPoint from = new IPEndPoint(0, 0);
        private static int PORT = 9876;
        private static string BROADCASTIP = "192.168.0.255";

        // SEND P2P Transaction Blockchain
        private static void SendTransaction(BlockChain obj)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            udpClient.Send(data, data.Length, BROADCASTIP, PORT);
        } 

        // RECEIVE P2P Blockchain
        private static void ReceiveTransaction()
        {
            while (is_Active)
            {
                var recvBuffer = udpClient.Receive(ref from);
                var message = Encoding.UTF8.GetString(recvBuffer);

                BlockChain chain = JsonConvert.DeserializeObject<BlockChain>(message);
                if (chain.isValid() && chain.Chain.Count > yosaCoin.Chain.Count)
                {
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(chain.PendingTransactions);
                    newTransactions.AddRange(yosaCoin.PendingTransactions);

                    chain.PendingTransactions = newTransactions;
                    yosaCoin = chain;
                }
            }
        }       

        static void Main(string[] args)
        {   
            Draw.draw_logo();

            Console.Write("Enter your username: ");
            username = Console.ReadLine();

            // Creating and Initialize Cryptocurrency Blockchain
            yosaCoin = new BlockChain();
            yosaCoin.InitializeChain();

            //p2p Settings  
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.ExclusiveAddressUse = false;
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            // Running Receive Task
            Task.Run(() => ReceiveTransaction());

            Draw.draw_menu();

            // Running main Cycle 
            while(true)
            {
                Console.Write("Option: ");
                int choice;
                try
                {
                    choice = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.Write("Press ENTER to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    Draw.draw_menu();
                    continue;
                }

                if(choice == 1)
                    Console.WriteLine(JsonConvert.SerializeObject(yosaCoin, Formatting.Indented));
                else if(choice == 2)
                {
                    // Console.Write("Enter the remote host: ");
                    // string serverhost = Console.ReadLine();
                    // Console.Write("Enter the remote port: ");
                    // int serverport = int.Parse(Console.ReadLine());
                    Console.Write("Enter Amount: ");
                    uint amount = uint.Parse(Console.ReadLine());
                    Console.Write("Enter the receiver username: ");
                    string receiver = Console.ReadLine();

                    yosaCoin.CreateTransaction(new Transaction { sender = username, receiver = receiver, amount = amount });
                    yosaCoin.ProcessPendingTransactions(username);
                    SendTransaction(yosaCoin);
                }
                else if(choice == 3)
                {
                    Console.Write("Coins count: ");
                    int coinscount = int.Parse(Console.ReadLine());
                    for(int i = 0; i < coinscount; i++)
                    {
                        yosaCoin.MineBlock(username);
                        SendTransaction(yosaCoin);
                    }
                }
                else if(choice == 4)
                {
                    is_Active = false;
                    break;
                }

                Console.Write("Press ENTER to continue...");
                Console.ReadKey();
                Console.Clear();
                Draw.draw_menu();
            }
        }
    }
}