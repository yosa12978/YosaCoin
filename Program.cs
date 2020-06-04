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
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.ASCII.GetBytes($"{timestamp}-{previousHash ?? ""}-{data}");  
            byte[] output = sha256.ComputeHash(bytes);  
            return Convert.ToBase64String(output);  
        } 
    }

    public class BlockChain
    {
        public List<Block> Chain { get; set; }
        public BlockChain()
        {
            Chain = new List<Block>();
            Chain.Add(new Block(DateTime.Now, null, new Transaction()));
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
                var transaction = block.data;
                var sender = transaction.sender;
                var receiver = transaction.receiver;
                if (name == sender)
                    spending += transaction.amount;                    
                if (name == receiver)
                    income += transaction.amount;
                balance = income - spending;
            }
            return balance;
        }
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            BlockChain yosaCoin = new BlockChain();
            
            yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction {sender = "yosa", receiver = "falcon", amount = 1} ));
            yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction {sender = "yosa", receiver = "falcon", amount = 2} ));
            yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction {sender = "yosa", receiver = "falcon", amount = 3} ));
            yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction {sender = "falcon", receiver = "yosa", amount = 3} ));
            yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction {sender = "yosa", receiver = "falcon", amount = 3} ));
            yosaCoin.AddBlock( new Block(DateTime.Now, null, new Transaction {sender = "yosa", receiver = "falcon", amount = 3} ));


            Console.WriteLine(yosaCoin.GetBalance("falcon"));

            Console.WriteLine(JsonConvert.SerializeObject(yosaCoin));
        }
    }
}