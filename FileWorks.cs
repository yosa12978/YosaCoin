using System;
using System.IO;
using Newtonsoft.Json;

namespace YosaCoin
{
    public class FileWorks
    {
        public static void AddDataToFile(Block block)
        {
            BlockChain yosaCoinBase = JsonConvert.DeserializeObject<BlockChain>(File.ReadAllText("blockchain.json")); 
            string json = JsonConvert.SerializeObject(block);
            File.WriteAllText("blockchain.json",  json);
            yosaCoinBase.AddBlock(block);
        }

        public static BlockChain GetBlockChain()
        {
            BlockChain yosaCoinBase;
            using (FileStream fs = File.OpenRead("blockchain.json"))
            {
                byte[] file_bytes = new byte[fs.Length];
                fs.Read(file_bytes, 0, file_bytes.Length);
                string text = System.Text.Encoding.Default.GetString(file_bytes);
                yosaCoinBase = JsonConvert.DeserializeObject<BlockChain>(text);
            }
            return yosaCoinBase;
        }
    }
}