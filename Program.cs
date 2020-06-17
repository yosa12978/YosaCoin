using System.Drawing;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading;
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

            Console.WriteLine($"Current listening host: {Addr}");
            Console.WriteLine($"Current listening port: {prt}");


            Console.Write("\n[MAIN MENU]\n1.Get Blockchain\n2.Send Coins\n3.Mine Coins\n4.Exit\n");

        }

        private static void SendMessage(Block send_data, string remoteAddress, int remotePort)
        {
            UdpClient client = new UdpClient();
            byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(send_data));
            client.Send(data, data.Length, remoteAddress, remotePort);
        }
 
        private static void ReceiveMessage()
        {
            is_Active = true;
            UdpClient client = new UdpClient(prt);
            IPEndPoint remoteIp = null;
            while(is_Active)
            {
                byte[] data = client.Receive(ref remoteIp);
                string message = Encoding.Unicode.GetString(data);
                Block block = JsonConvert.DeserializeObject<Block>(message);
                block.previousHash = yosaCoin.getLatest().hash;
                Program.yosaCoin.AddBlock(block);
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
            Console.Write("Current listening host: ");
            Addr = Console.ReadLine();
            Console.Write("Current listening port: ");
            prt = int.Parse(Console.ReadLine());

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();

            Console.Clear();

            
            yosaCoin = new BlockChain();

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
                    Console.Write("Enter the remote host: ");
                    string serverhost = Console.ReadLine();
                    Console.Write("Enter the remote port: ");
                    int serverport = int.Parse(Console.ReadLine());
                    Console.Write("Enter Amount: ");
                    int amount = int.Parse(Console.ReadLine());
                    Console.Write("Enter the receiver username: ");
                    string receiver = Console.ReadLine();

                    //List<Transaction> transactions = new List<Transaction> {new Transaction { sender = username, receiver = receiver, amount = amount }};
                    yosaCoin.CreateTransaction(new Transaction { sender = username, receiver = receiver, amount = amount });
                    Block newblock = new Block(DateTime.Now, yosaCoin.getLatest().hash, yosaCoin.PendingTransactions);
                    SendMessage(newblock, serverhost, serverport);
                    yosaCoin.AddBlock(newblock);
                    yosaCoin.PendingTransactions = new List<Transaction>();
                    
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

            // Console.WriteLine($"Welcome, {username}!\n");
            // for (int i = 0; i < 3; i++)
            // {
            //     yosaCoin.MineBlock(username);    
            // }

            // Console.WriteLine($"Balance: {yosaCoin.GetBalance(username)} YosaCoin's\n");
            // Console.WriteLine(!yosaCoin.isValid() ? "One block isn't valid!" : "All blocks is valid!");

            // Console.WriteLine(JsonConvert.SerializeObject(yosaCoin, Formatting.Indented));
            // Console.ReadKey();
        }
    }
}