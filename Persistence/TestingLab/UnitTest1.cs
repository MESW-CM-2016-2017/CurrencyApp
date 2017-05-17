using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Janus.API;
using Janus.Models;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Janus.Utils;

namespace TestingLab
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PedidoAPITest()
        {
            string json = @"{
	'list': {
		'meta': {
			'type': 'resource-list',
			'start': 0,
			'count': 188
		},
		'resources': [{
				'resource': {
					'classname': 'Quote',
					'fields': {
						'name': 'USD/KRW',
						'price': '1135.489990',
						'symbol': 'KRW=X',
						'ts': '1493641857',
						'type': 'currency',
						'utctime': '2017-05-01T12:30:57+0000',
						'volume': '0'
					}
				}
			},
			{
				'resource': {
					'classname': 'Quote',
					'fields': {
						'name': 'SILVER 1 OZ 999 NY',
						'price': '0.058292',
						'symbol': 'XAG=X',
						'ts': '1493641873',
						'type': 'currency',
						'utctime': '2017-05-01T12:31:13+0000',
						'volume': '36'
					}
				}
			}
		]
	}
}";

            var fdate = JObject.Parse(json)["list"];
            String resources = fdate.ToString();
            JToken resources1 = JObject.Parse(resources)["resources"];


            foreach (JToken item in resources1)
            {
                JToken subItem = item["resource"]["fields"].Value<JToken>();
                //JToken subsubItem = subItem["fields"].Value<JToken>();

                String symbol = subItem["symbol"].Value<String>();
                String name = subItem["name"].Value<String>();
                String ts = subItem["ts"].Value<String>();
                String utctime = subItem["utctime"].Value<String>();
                Double price = subItem["price"].Value<Double>();
                String type = subItem["type"].Value<String>();

                //JsonListBox.Items.Add(fileName);
            }


            Assert.IsTrue(true);
        }

        [TestMethod]
        public void LeituraDeCurrenciesTest()
        {
            APIHandler api = new APIHandler();
            //List<CurrencyDTO> list = 
            IAsyncResult ar = api.GetAllCurrencies(null);
            var state = (Tuple<WebRequest>)ar.AsyncState;
            var request = state.Item1;

            using (HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse)
            {
                //Device.BeginInvokeOnMainThread(() => state.Item1.Text = "Status: " + response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        Assert.IsNotNull(content);
                        //Device.BeginInvokeOnMainThread(() => state.Item2.Text = content);
                    }
            }

        }

        [TestMethod]
        public void ConvertTest()
        {
            APIHandler api = new APIHandler();
            List<CurrencyDTO> list = api.GetCurrenciesDTO();
            Double selectedQuantity = 10;
            Double response = api.Convert("EUR", "USD", selectedQuantity, list);
            Assert.IsTrue(response > selectedQuantity);
        }

        [TestMethod]
        public void ConvertTest2()
        {
            APIHandler api = new APIHandler();
            List<CurrencyDTO> list = api.GetCurrenciesDTO();
            Double response = api.Convert("USD", "EUR", 1, list);
            Assert.IsTrue(response < 1);
        }

        [TestMethod]
        public void ConvertTest3()
        {
            APIHandler api = new APIHandler();
            List<CurrencyDTO> list = api.GetCurrenciesDTO();
            Double response = api.Convert("CHF", "EUR", 1, list);
            Assert.IsTrue(response < 1);
        }       

        [TestMethod]
        public void GetCurrentDateString()
        {
            String nowDate = Utils.getCurrentDateString();
            Assert.IsNotNull(nowDate);
        }

        [TestMethod]
        public void GetCurrentTimestamp()
        {
            Int32 ts = Utils.getCurrentTimestamp();
            Assert.IsNotNull(ts);
        }
    }
}
