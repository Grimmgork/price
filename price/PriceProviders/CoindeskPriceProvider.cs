using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;

namespace price.PriceProviders
{
	public class CoindeskPriceProvider : IPriceProvider
	{
		public void Configure(string[] parameters)
		{
			
		}

		public double GetLatestPrice(string ticker, string convert)
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

			double o = (double)obj["o"];
			double h = (double)obj["h"];
			double l = (double)obj["l"];
			double c = (double)obj["c"];
			double price = (o + h + l + c) / 4;

			return price;
		}
	}
}
