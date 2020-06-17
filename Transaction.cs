using System;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
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
}