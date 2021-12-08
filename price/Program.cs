using System;
using price.PriceProviders;

namespace price
{
	class Program
	{
		static Config config;

		static void Main(string[] args)
		{
			config = DirectoryManager.LoadSpecificObject<Config>();
			if (config == null){
				config = new Config() { Ticker= new TickerPair[] { new TickerPair() { providerid="1", ticker="btc", convert="usd" } },
										PriceProviders= new string[][] {	new string[] { "0", "cmc", "API_KEY_HERE" }, new string[] { "1", "cod" } } };
				DirectoryManager.SaveSpecificObject(config);
				ThrowErrorMessageAndQuit("no config.json found! -> Created a new one!");
				return;
			}

			PriceProviderSentinel.Init(config.PriceProviders);

			foreach(TickerPair t in config.Ticker){
				PrintRow(t.ticker, t.convert, t.providerid);
			}
		}

		static void PrintRow(string ticker, string convert, string providerid)
		{
			Console.WriteLine("{0}/{1}:\t{2}", ticker.ToUpper(), convert.ToUpper(), PriceProviderSentinel.GetLatestPrice(providerid, ticker, convert));
		}

		static void ThrowErrorMessageAndQuit(string message)
		{
			throw new Exception("ERROR: " + message);
		}

		[DirectoryManager.SpecificFilePath("config.json")]
		public class Config
		{
			public TickerPair[] Ticker { get; set; }
			public string[][] PriceProviders{ get; set; }
		}

		public struct TickerPair
		{
			public string providerid { get; set; }
			public string ticker { get; set; }
			public string convert { get; set; }
		}

		[DirectoryManager.SpecificFilePath("cache.json")]
		public class Cache
		{
			
		}
	}

	
}
