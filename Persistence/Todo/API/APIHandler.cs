using System;
using System.Collections.Generic;
using CurrencyApp.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace CurrencyApp.API
{
    public class APIHandler
    {
        private String API_URL_ALL = "https://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=json";
        //private String API_URL_SINGLE = "http://download.finance.yahoo.com/d/quotes?f=sl1d1t1&s=";

        public IAsyncResult GetAllCurrencies(AsyncCallback callback)
        {
            HTTPHandler client = new HTTPHandler();
            if (callback == null) return client.Get(API_URL_ALL, null);
            var cb = new AsyncCallback(callback);
            return client.Get(API_URL_ALL, cb);
        }

        public static async Task<Double> Convert(String symbolFrom, String symbolTo,Wallet wallet, Double toConvertoQuantity)
        {
            CurrencyDTO to = await App.DatabaseCurrencies.GetLastUpdateTimeString(symbolTo + "=X");
            CurrencyDTO from = await App.DatabaseCurrencies.GetLastUpdateTimeString(symbolFrom + "=X");
            if (to != null && from != null)
            {
                Double convertedQuantity = new APIHandler().Convert(from, to, toConvertoQuantity);
                Double diff = wallet.Quantity - toConvertoQuantity;
                wallet.Quantity = wallet.Quantity - diff;
                if (diff >= 0.1)
                {
                    await App.Database.SaveItemAsync(wallet);
                }
                else
                {
                    await App.Database.DeleteItemAsync(wallet);
                }
                await App.Database.SaveOrUpdateItemAsync(new Wallet(convertedQuantity, symbolTo));
                return convertedQuantity;
            }
            return -1;
        }

        public static async Task<Double> ConvertWithoutSaving(String symbolTo, Wallet wallet, Double toConvertoQuantity)
        {
            CurrencyDTO to = await App.DatabaseCurrencies.GetLastUpdateTimeString(symbolTo + "=X");
            CurrencyDTO from = await App.DatabaseCurrencies.GetLastUpdateTimeString(wallet.Symbol + "=X");
            if (to != null && from != null)
            {
                Double convertedQuantity = new APIHandler().Convert(from, to, toConvertoQuantity);
                Double diff = wallet.Quantity - toConvertoQuantity;
                wallet.Quantity = wallet.Quantity - diff;                
                return convertedQuantity;
            }
            return -1;
        }

        public Double Convert(String symbolFrom, String symbolTo, Double qtd, List<CurrencyDTO> list)
        {
            CurrencyDTO from = null;
            CurrencyDTO to = null;
            foreach (CurrencyDTO currency in list)
            {
                if (currency.Symbol != null && currency.Symbol.Length > 0)
                {
                    var symbol = currency.Symbol.Substring(0, currency.Symbol.Length - 2);
                    if (symbol.CompareTo(symbolFrom) == 0)
                    {
                        from = currency;
                        if (to != null) break;
                        continue;
                    }
                    if (symbol.CompareTo(symbolTo) == 0)
                    {
                        to = currency;
                        if (from != null) break;
                        continue;
                    }
                }
            }

            return Convert(from, to, qtd);
        }

        public String GetAllCurrenciesSync()
        {
            IAsyncResult ar = GetAllCurrencies(null);
            var state = (Tuple<WebRequest>)ar.AsyncState;
            var request = state.Item1;

            using (HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse)
            {
                //Device.BeginInvokeOnMainThread(() => state.Item1.Text = "Status: " + response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        return content;
                        //Device.BeginInvokeOnMainThread(() => state.Item2.Text = content);
                    }
            }
            return null;
        }

        public Double Convert(CurrencyDTO symbolFrom, CurrencyDTO symbolTo, Double qtd)
        {
            return (symbolTo.Price / symbolFrom.Price) * qtd;
        }

        public String UpdateCurrencies()
        {
            try
            {
                List<CurrencyDTO> currencies = GetCurrenciesDTO();
                if (currencies != null)
                {
                    // filter list of currencies
                    App.DatabaseCurrencies.SaveItemsAsync(CurrencyDTO.filterList(currencies));
                    return "Last Update: " + Utils.Utils.getCurrentDateString();
                }
            }
            catch (Exception e)
            {
                return "Error Updating";
            }
            return "";
        }

        public List<CurrencyDTO> GetCurrenciesDTO()
        {
            String response = GetAllCurrenciesSync();
            List<CurrencyDTO> list = new List<CurrencyDTO>();

            var fdate = JObject.Parse(response)["list"];
            String resources = fdate.ToString();
            JToken resources1 = JObject.Parse(resources)["resources"];

            foreach (JToken item in resources1)
            {
                JToken subItem = item["resource"]["fields"].Value<JToken>();
                String symbol = subItem["symbol"].Value<String>();
                String name = subItem["name"].Value<String>();
                String ts = subItem["ts"].Value<String>();
                String utctime = subItem["utctime"].Value<String>();
                Double price = subItem["price"].Value<Double>();
                String type = subItem["type"].Value<String>();
                list.Add(new CurrencyDTO(name, type, ts, utctime, price, symbol));
            }
            return list;
        }
    }
}
