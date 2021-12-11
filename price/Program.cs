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
			Dictionary<int,Task<decimal>> tasks = new Dictionary<int, Task<decimal>>();

			decimal[] results = new decimal[tickers.Length];
			HashSet<int> error = new HashSet<int>();

			int[] rowLengths = new int[tickers.Length];

			for (int i = 0; i < tickers.Length; i++){
				TickerRow t = tickers[i];
				tasks.Add(i, GetPriceAsync(t.providerid, t.ticker, t.convert));
			}

			while(true)
			{
				string rows = "";
				for (int i = 0; i < tickers.Length; i++)
				{
					string value = "...";
					if (tasks.ContainsKey(i))
					{
						Task<decimal> task = tasks[i];
						if (task.IsCompleted)
						{
							if(task.IsFaulted)
							{
								tasks.Remove(i);
								error.Add(i);
								value = "ERROR";
							}
							else
							{
								results[i] = task.Result;
								tasks.Remove(i);
								value = results[i].ToString();
							}
						}
					}
					else
					{
						if (error.Contains(i))
							value = "ERROR";
						else
							value = results[i].ToString();
					}

					int lastLength = rowLengths[i];
					string row = FormatRow(tickers[i].ticker, tickers[i].convert, value) + "\n";
					rowLengths[i] = row.Length;
					row.PadRight(lastLength, ' ');
					rows += row;
				}

				Console.SetCursorPosition(0, Console.GetCursorPosition().Top - tickers.Length);
				Console.Write(rows);

				if (tasks.Count == 0)
					break;

				Task.WaitAny(tasks.Values.ToArray());
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