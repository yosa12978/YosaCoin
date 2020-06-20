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
        public static string username;
        public static BlockChain yosaCoin;
        public static int prt;
        public static string Addr;
        public static PeerServer Server = null;
        public static PeerClient Client = new PeerClient();
        public static bool is_Active = true;
        static bool chainSynched = false;

        static void draw_menu()
        {
            Console.Title = "YosaCoin CLI";
            ConsoleColor classic_console_color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ll      ll   llllllll    lllllll     llllllll    ||    llllllll   lllllllll   ll  ll      ll ");
            Console.WriteLine("   ll    ll   ll      ll  ll          ll      ll   ||   ll         ll       ll      llll    ll ");
            Console.WriteLine("    ll  ll    ll      ll  ll          ll      ll   ||   ll         ll       ll  ll  ll ll   ll ");
            Console.WriteLine("     llll     ll      ll   llllllll   llllllllll   ||   ll         ll       ll  ll  ll  ll  ll ");
            Console.WriteLine("      ll      ll      ll          ll  ll      ll   ||   ll         ll       ll  ll  ll   ll ll ");
            Console.WriteLine("      ll      ll      ll          ll  ll      ll   ||   ll         ll       ll  ll  ll    llll ");
            Console.WriteLine("      ll       llllllll    llllllll   ll      ll   ||    llllllll   lllllllll   ll  ll     lll\n ");
            Console.ForegroundColor = classic_console_color;
            Console.WriteLine($"\nWelcome {username}");
            Console.WriteLine($"Balance: {yosaCoin.GetBalance(username)} YosaCoin's\n");

            // Console.WriteLine($"Current listening host: {Addr}");
            // Console.WriteLine($"Current listening port: {prt}");


            Console.Write("\n[MAIN MENU]\n1.Get Blockchain\n2.Create Transaction\n3.Mine Coins\n4.Exit\n");

        }

        private static void SendMessage(BlockChain send_data, string remoteAddress, int remotePort)
        {
            UdpClient client = new UdpClient();
            byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(send_data));
            client.Send(data, data.Length, remoteAddress, remotePort);
        }
 
        private static void ReceiveMessage()
        {
            UdpClient client = new UdpClient(prt);
            IPEndPoint remoteIp = null;
            while(is_Active)
            {
                byte[] data = client.Receive(ref remoteIp);
                string message = Encoding.Unicode.GetString(data);
                BlockChain chain = JsonConvert.DeserializeObject<BlockChain>(message);

                if (chain.isValid() && chain.Chain.Count > yosaCoin.Chain.Count)
                {
                    Console.WriteLine("I AM HERE!");
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(chain.PendingTransactions);
                    newTransactions.AddRange(yosaCoin.PendingTransactions);

                    chain.PendingTransactions = newTransactions;
                    yosaCoin = chain;
                }

                // if (!chainSynched)
                // {
                //     try
                //     {
                //         SendMessage(yosaCoin, remoteIp.Address.ToString(), remoteIp.Port);
                //     }catch(SocketException e)
                //     {
                //         Console.WriteLine(e.Message);
                //     }
                //     chainSynched = true;
                // }
            }
        }

        static void Main(string[] args)
        {
            Console.Clear();
            
            Console.Title = "YosaCoin CLI";
            ConsoleColor classic_console_color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ll      ll   llllllll    lllllll     llllllll    ||    llllllll   lllllllll   ll  ll      ll ");
            Console.WriteLine("   ll    ll   ll      ll  ll          ll      ll   ||   ll         ll       ll      llll    ll ");
            Console.WriteLine("    ll  ll    ll      ll  ll          ll      ll   ||   ll         ll       ll  ll  ll ll   ll ");
            Console.WriteLine("     llll     ll      ll   llllllll   llllllllll   ||   ll         ll       ll  ll  ll  ll  ll ");
            Console.WriteLine("      ll      ll      ll          ll  ll      ll   ||   ll         ll       ll  ll  ll   ll ll ");
            Console.WriteLine("      ll      ll      ll          ll  ll      ll   ||   ll         ll       ll  ll  ll    llll ");
            Console.WriteLine("      ll       llllllll    llllllll   ll      ll   ||    llllllll   lllllllll   ll  ll     lll\n ");
            Console.ForegroundColor = classic_console_color;

            Console.Write("Enter your username: ");
            username = Console.ReadLine();
            // Console.Write("Current listening host: ");
            // Addr = Console.ReadLine();
            // Console.Write("Current listening port: ");
            // prt = int.Parse(Console.ReadLine());

            yosaCoin = new BlockChain();


            int PORT = 9876;
            UdpClient udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.ExclusiveAddressUse = false;
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            var from = new IPEndPoint(0, 0);
            Task.Run(() =>
            {
                while (is_Active)
                {
                    var recvBuffer = udpClient.Receive(ref from);
                    var message = Encoding.UTF8.GetString(recvBuffer);
                    BlockChain chain = JsonConvert.DeserializeObject<BlockChain>(message);
                    // Блокчейн chain ПОСТОЯННО не валидна
                    // так как создаётся 2 генезиз блока
                    if (chain.isValid() && chain.Chain.Count > yosaCoin.Chain.Count)
                    {
                        List<Transaction> newTransactions = new List<Transaction>();
                        newTransactions.AddRange(chain.PendingTransactions);
                        newTransactions.AddRange(yosaCoin.PendingTransactions);

                        chain.PendingTransactions = newTransactions;
                        yosaCoin = chain;
                    }    
                }
            });

            Console.Clear();

            draw_menu();

            while(true)
            {

                Console.Write("Option: ");
                int choice;
                try
                {
                    choice = int.Parse(Console.ReadLine());
                }catch
                {
                    Console.Write("Press ENTER to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    draw_menu();
                    continue;
                }
                if(choice == 1)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(yosaCoin, Formatting.Indented));
                }
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
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(yosaCoin));
                    udpClient.Send(data, data.Length, "192.168.0.255", PORT);
                }
                else if(choice == 3)
                {
                    Console.Write("Coins count: ");
                    int coinscount = int.Parse(Console.ReadLine());
                    for(int i = 0; i < coinscount; i++)
                        yosaCoin.MineBlock(username);
                }
                else if(choice == 4)
                {
                    is_Active = false;
                    break;
                }
                Console.Write("Press ENTER to continue...");
                Console.ReadKey();
                Console.Clear();
                draw_menu();
            }
        }
    }
}