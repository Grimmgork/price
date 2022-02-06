using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using price.PriceProviders;
using System.Linq;
using System.Threading;

namespace price
{
	class Program
	{
		static int cursorStartX;
		static int curserStartY;
		
		static void Main(string[] args)
		{
			cursorStartX = Console.CursorLeft;
			curserStartY = Console.CursorTop;
			
			//load configuration file
			Config config = DirectoryManager.LoadSpecificObject<Config>();
			if (config == null) {
				config = new Config() {
					Ticker = new TickerRow[] { new TickerRow() { providerid = "1", ticker = "btc", convert = "usd" } },
					PriceProviders = new ProviderData[] { new ProviderData() { id = "0", providertype = "cmc", parameters = new string[] { "API_KEY_HERE" } }, new ProviderData() { id = "1", providertype = "cod", parameters = null } }
				};
				DirectoryManager.SaveSpecificObject(config);
				PrintErrorMessage("no config.json found! -> Created a new one!");
				return;
			}

			//configure the wrapper for the price providers
			PriceProviderSentinel.Config(config.PriceProviders);

			//fetch the table
			RenderTickersToConsole(config.Ticker);
		}

		static void RenderTickersToConsole(TickerRow[] tickers)
		{
			Task<decimal>[] tasks = new Task<decimal>[tickers.Length];
			for (int i = 0; i < tickers.Length; i++){
				TickerRow t = tickers[i];
				tasks[i] = GetPriceAsync(t.providerid, t.ticker, t.convert);
			}

			Console.WriteLine("fetching ...");
			Task.WaitAll(tasks);
			Console.WriteLine("done!");
			Console.WriteLine();
			for(int i = 0; i < tickers.Length; i++){
				Task<decimal> task = tasks[i];
				TickerRow ticker = tickers[i];

				Console.WriteLine(FormatRow(ticker.ticker, ticker.convert, task.Result.ToString()));
			}

		}

		static string FormatRow(string ticker, string convert, string value)
		{
			string s = String.Format("{0}/{1}:\t{2}", ticker.ToUpper(), convert.ToUpper(), value);
			return s;
		}

		static async Task<decimal> GetPriceAsync(string providerid, string ticker, string convert)
		{
			return await Task.Run(() => PriceProviderSentinel.GetLatestPrice(providerid, ticker, convert));
		}

		static void PrintErrorMessage(string message)
		{
			Console.WriteLine("ERROR: " + message);
		}

		[DirectoryManager.SpecificFilePath("config.json")]
		public class Config
		{
			public TickerRow[] Ticker { get; set; }
			public ProviderData[] PriceProviders{ get; set; }
		}

		public struct TickerRow
		{
			public string providerid { get; set; }
			public string ticker { get; set; }
			public string convert { get; set; }
		}
	}
}