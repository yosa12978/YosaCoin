using System;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace YosaCoin
{
    public class BlockChain
    {
        public List<Block> Chain { get; set; }
        public List<Transaction> PendingTransactions = new List<Transaction>();

        public BlockChain()
        {
            InitializeChain();
        }

        private void InitializeChain()
        {
            Chain = new List<Block>();
            Chain.Add( new Block(DateTime.Now, null, new List<Transaction>{}) );
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
            foreach (Block block in Chain)
            {
                foreach(Transaction transactions in block.data)
                {
                    var sender = transactions.sender;
                    var receiver = transactions.receiver;
                    if (name == sender)
                        balance -= transactions.amount;                    
                    if (name == receiver)
                        balance += transactions.amount;
                }
            }
            return balance;
        }

        public void MineBlock(string username)
        {
            Transaction minerRewardTransaction = new Transaction { sender = null, receiver = username, amount = 1};
            CreateTransaction(minerRewardTransaction);

            Block block = new Block(DateTime.Now, Chain[Chain.Count-1].hash, PendingTransactions);

            block.MineBlock(5);
            AddBlock(block);
            PendingTransactions = new List<Transaction>();
        }

        public void CreateTransaction(Transaction transaction)
        {
            PendingTransactions.Add(transaction);
        }

        public void ProcessPendingTransactions(string minerAddress)
        {
            Block block = new Block(DateTime.Now, Chain.Last().hash, PendingTransactions);
            AddBlock(block);
            PendingTransactions = new List<Transaction>();
            CreateTransaction(new Transaction{ sender = null, receiver = minerAddress, amount = 1 });  
        }
    }
}