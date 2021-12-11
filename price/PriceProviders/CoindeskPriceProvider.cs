using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Web;

namespace price.PriceProviders
{
	public class CoindeskPriceProvider : IPriceProvider
	{
		public void Configure(string[] parameters)
		{
			
		}

		public decimal GetLatestPrice(string ticker, string convert)
		{
			UriBuilder url = new UriBuilder("https://production.api.coindesk.com/v2/tb/price/ticker");
			NameValueCollection queryString = HttpUtility.ParseQueryString("");
			queryString["assets"] = ticker.ToUpper();

			if (convert.ToLower() != "usd")
				throw new Exception();

			url.Query = queryString.ToString();

			WebClient client = new WebClient();
			client.Headers.Add("Accepts", "application/json");
			client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

			string data = client.DownloadString(url.ToString());

			JToken obj = JObject.Parse(data)["data"][ticker.ToUpper()]["ohlc"];

			decimal o = (decimal)obj["o"];
			decimal h = (decimal)obj["h"];
			decimal l = (decimal)obj["l"];
			decimal c = (decimal)obj["c"];
			decimal price = (o + h + l + c) / 4;

			return price;
		}
	}
}
