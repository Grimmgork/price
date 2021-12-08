using System;
using System.Collections.Generic;
using System.Text;

namespace price.PriceProviders
{
	public interface IPriceProvider
	{
		public double GetLatestPrice(string ticker, string convert);
		public void Configure(string[] parameters);
	}
}