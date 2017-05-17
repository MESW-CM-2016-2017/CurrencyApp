using System.Collections.Generic;
using SQLite;
using System;

namespace Janus.Models
{
    public class Wallet
    {
        public Wallet()
        {
            Symbol = "EUR";
        }

        public Wallet(double qtd, string symbol)
        {
            Quantity = Math.Round(qtd, 2);
            Symbol = symbol;
        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Symbol { get; set; }
        public double Quantity { get; set; }

        public override string ToString()
        {
            return Quantity + " " + Symbol;
        }

        public string GetText()
        {
            return Quantity + " " + Symbol;
        }

        public string Print
        {
            get { return GetText(); }
        }
    }
}
