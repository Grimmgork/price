using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace price.PriceProviders
{
	public static class PriceProviderSentinel
	{
		static Dictionary<string, IPriceProvider> providers;
		static Dictionary<string, ProviderData> data;
		
		public static void Config(IEnumerable<ProviderData> provs)
		{
			providers = new Dictionary<string, IPriceProvider>();
			data = new Dictionary<string, ProviderData>();

			foreach(ProviderData d in provs){
				AddProvider(d);
			}
		}
		
		public static IEnumerable<ProviderData> GetConfig()
		{
			return data.Values;
		}

		public static void RemoveProvider(string providerid)
		{
			providers.Remove(providerid);
			data.Remove(providerid);
		}

		public static void AddProvider(ProviderData prov)
		{
			data.Add(prov.id, prov);
			IPriceProvider p = CreateNewProvider(prov.providertype, prov.parameters);
			providers.Add(prov.id, p);
		}

		public static decimal GetLatestPrice(string providerid, string ticker, string exchange)
		{
			if (providers == null)
				return 0;

			return providers[providerid].GetLatestPrice(ticker, exchange);
		}

		public static IPriceProvider GetById(string providerid)
		{
			return providers[providerid];
		}

		private static IPriceProvider CreateNewProvider(string providertype, string[] parameters)
		{
			IPriceProvider p = null;
			switch (providertype)
			{
				case "cmc":
					p = new CoinMarketCapPriceProvider();
					break;
				case "cod":
					p = new CoindeskPriceProvider();
					break;
				case "cmd":
					p = new CommandProvider();
					break;
			}

			if (p != null)
				p.Configure(parameters);

			return p;
		}

	}

	public struct ProviderData
	{
		public string id { get; set; }
		public string providertype { get; set; }
		public string[] parameters { get; set; }
	}
}
