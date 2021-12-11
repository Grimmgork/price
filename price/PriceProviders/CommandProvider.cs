using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace price.PriceProviders
{
	public class CommandProvider : IPriceProvider
	{
		public void Configure(string[] parameters)
		{
			
		}

		public decimal GetLatestPrice(string ticker, string convert)
		{
			
			return 13;
		}
	}
}
