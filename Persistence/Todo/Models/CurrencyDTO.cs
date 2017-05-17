using System;
using System.Collections.Generic;
using SQLite;
namespace Janus.Models
{
    public class CurrencyDTO
    {

        public static String[] top10Currencies = {
            "USD",
            "EUR",
            "JPY",
            "GBP",
            "CHF",
            "CNY",
            "SEK",
            "MXN",
            "NOK",
            "KRW" };

        public static List<CurrencyDTO> filterList(List<CurrencyDTO> list)
        {
            List<CurrencyDTO> filteredList = new List<CurrencyDTO>();
            foreach(CurrencyDTO currency in list)
            {
                foreach (String symbol in top10Currencies)
                {
                    if (currency.Symbol.Contains(symbol))
                    {
                        filteredList.Add(currency);
                    }
                }           
            }
            return filteredList;
        }

        public CurrencyDTO()
        {

        }

        public CurrencyDTO(String name, String type, String ts, String utc, Double price, String symbol)
        {
            this.Name = name;
            this.Timestamp = ts;
            this.UTCTime = utc;
            this.Price = price;
            this.Symbol = symbol;
            this.Type = type;
            //
            this.RequestTimestamp = Utils.Utils.getCurrentTimestamp();
            this.RequestTimestampString = Utils.Utils.getCurrentDateString();
        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public bool Done { get; set; }
        public String Timestamp { get; set; }
        public String UTCTime { get; set; }
        public Double Price { get; set; }
        public String Symbol { get; set; }
        public String Type { get; set; }

        public Int32 RequestTimestamp { get; set; }
        public String RequestTimestampString { get; set; }
    }
}
