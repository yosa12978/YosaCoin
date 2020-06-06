using System.Drawing;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace YosaCoin
{
    public class Transaction
    {
        public string sender { get; set; }
        public string receiver { get; set; }
        public int amount { get; set; }

        public override string ToString()
        {
            return $"sender: {sender}, receiver: {receiver}, amount: {amount}";
        }
    }
    public class Block
    {
        public int id { get; set; }
        public DateTime timestamp { get; set; }  
        public string previousHash { get; set; }  
        public string hash { get; set; }  
        public Transaction data { get; set; }  
        public long nonce { get; private set; } 

        public Block(DateTime timestamp, string previousHash, Transaction data)
        {
            id = 0;
            this.timestamp = timestamp;
            this.previousHash = previousHash;
            this.data = data;
            hash = ComputeHash();
        }

        public string ComputeHash()  
        {  
            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}-{previousHash ?? ""}-{JsonConvert.SerializeObject(data)}-{nonce}"));  
            StringBuilder builder = new StringBuilder();  
            for (int i = 0; i < bytes.Length; i++)  
                builder.Append(bytes[i].ToString("x2"));  
            return builder.ToString();   
        } 

        public void MineBlock(int proofOfWorkDifficulty)
        {
            string hashValidationTemplate = new String('0', proofOfWorkDifficulty);
            while (hash.Substring(0, proofOfWorkDifficulty) != hashValidationTemplate)
            {
                nonce++;
                hash = ComputeHash();
            }   
            Console.WriteLine($"Block with hash = {hash} nonce = {nonce}  successfully mined!");
        }
    }

    public class BlockChain
    {
        public List<Block> Chain { get; set; }

        public BlockChain()
        {
            Chain = new List<Block>();
            Chain.Add( new Block(DateTime.Now, null, new Transaction()) );
        }

        public Block getLatest()
        {
            return Chain[Chain.Count - 1];
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = getLatest();
            block.id = latestBlock.id + 1;
            block.previousHash = latestBlock.hash;
            block.hash = block.ComputeHash();
            Chain.Add(block);
        }

        public bool isValid()
        {
            for(int i = 1; i < Chain.Count; i++)
            {
                Block current = Chain[i];
                Block previous = Chain[i-1];
                if(current.hash != current.ComputeHash())
                    return false;
                if(current.previousHash != previous.ComputeHash())
                    return false;
            }
            return true;
        }

        public double GetBalance(string name) {

            double balance = 0;
            double spending = 0;
            double income = 0;
            foreach (Block block in Chain)
            {
                // foreach(Transaction transactions in block.data)
                // {
                var transaction =block.data; // transactions;
                var sender = transaction.sender;
                var receiver = transaction.receiver;
                if (name == sender)
                    spending += transaction.amount;                    
                if (name == receiver)
                    income += transaction.amount;
                balance = income - spending;
                // }
            }
            return balance;
        }
        
        public bool isBlockValid(int id)
        {
            Block currentBlock = Chain.FirstOrDefault(m => m.id == id);
            Block previousBlock = Chain.FirstOrDefault(m => m.id == id-1);
            if(currentBlock.hash != currentBlock.ComputeHash())
                return false;
            if(currentBlock.previousHash != previousBlock.hash)
                return false;
            return true;
        }

        public void MineBlock(string username)
        {
            Transaction minerRewardTransaction = new Transaction { sender = null, receiver = username, amount = 1};
            Block block = new Block(DateTime.Now, Chain[Chain.Count-1].hash, minerRewardTransaction);
            block.MineBlock(5);
            Chain.Add(block);
        }
    }

    class Program
    {
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



            BlockChain yosaCoin = new BlockChain();
            
            // yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction[] { new Transaction {sender = "yosa", receiver = "falcon", amount = 1} }));
            // yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction[] { new Transaction {sender = "yosa", receiver = "falcon", amount = 2} }));
            // yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction[] { new Transaction {sender = "yosa", receiver = "falcon", amount = 3} }));
            // yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction[] { new Transaction {sender = "falcon", receiver = "yosa", amount = 3} }));
            // yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction[] { new Transaction {sender = "yosa", receiver = "falcon", amount = 3} }));
            // yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction[] { new Transaction {sender = "yosa", receiver = "falcon", amount = 3} }));

            string username = "falcon";
            Console.WriteLine($"Welcome, {username}!\n");
            for (int i = 0; i < 3; i++)
            {
                yosaCoin.MineBlock(username);    
            }

            Console.WriteLine($"Balance: {yosaCoin.GetBalance(username)} YosaCoin's\n");
            Console.WriteLine(!yosaCoin.isValid() ? "One block isn't valid!" : "All blocks is valid!");

            Console.WriteLine(JsonConvert.SerializeObject(yosaCoin, Formatting.Indented));
            Console.ReadKey();
        }
    }
}