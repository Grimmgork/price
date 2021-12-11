using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Cache;
using System.Security;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using price.PriceProviders;

namespace price.PriceProviders
{
	public class CoinMarketCapPriceProvider : IPriceProvider
	{
		private string apikey;

		public void Configure(string[] parameters)
		{
			apikey = parameters[0];
		}

		public decimal GetLatestPrice(string ticker, string convert)
		{
			UriBuilder url = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest");
			NameValueCollection queryString = HttpUtility.ParseQueryString("");
			queryString["symbol"] = ticker;
			queryString["convert"] = convert;

			url.Query = queryString.ToString();

			WebClient client = new WebClient();
			client.Headers.Add("X-CMC_PRO_API_KEY", apikey);
			client.Headers.Add("Accepts", "application/json");
			client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

			string data = client.DownloadString(url.ToString());

			JObject obj = JObject.Parse(data);
			decimal price = (decimal)obj["data"][ticker.ToUpper()]["quote"][convert.ToUpper()]["price"];

			return price;
		}
	}
}