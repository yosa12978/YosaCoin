using System;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace YosaCoin
{
    public class Block
    {
        public int id { get; set; }
        public DateTime timestamp { get; set; }  
        public string previousHash { get; set; }  
        public string hash { get; set; }  
        public List<Transaction> data { get; set; }  
        public long nonce { get; private set; } 

        public Block(DateTime timestamp, string previousHash, List<Transaction> data)
        {
            //id = 0;
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
            Console.WriteLine($"Block with hash = {hash} nonce = {nonce} successfully mined!");
        }
    }
}