using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace price.PriceProviders
{
	public static class PriceProviderSentinel
	{
		static Dictionary<string, IPriceProvider> providers;
		
		public static void Init(string[][] cmds)
		{
			providers = new Dictionary<string, IPriceProvider>();

			foreach(string[] a in cmds)
			{
				IPriceProvider p = GetProviderByShortcut(a[1], a.Skip(2).ToArray());
				if (p == null)
					throw new Exception();
				providers.Add(a[0], p);
			}
		}
		
		public static double GetLatestPrice(string providerid, string ticker, string exchange)
		{
			return providers[providerid].GetLatestPrice(ticker, exchange);
		}

		private static IPriceProvider GetProviderByShortcut(string shortcut, string[] parameters)
		{
			IPriceProvider p = null;
			switch (shortcut)
			{
				case "cmc":
					p = new CoinMarketCapPriceProvider();
					break;
				case "tst":
					p = new TestProvider();
					break;
				case "cod":
					p = new CoindeskPriceProvider();
					break;
			}

			if (p != null)
				p.Configure(parameters);

			return p;
		}
	}
}
