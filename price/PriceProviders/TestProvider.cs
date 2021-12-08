using System;
using System.Collections.Generic;
using System.Text;

namespace price.PriceProviders
{
	public class TestProvider : IPriceProvider
	{
		public void Configure(string[] parameters)
		{
			
		}

		public double GetLatestPrice(string ticker, string convert)
		{
			return 42d;
		}
	}
}
