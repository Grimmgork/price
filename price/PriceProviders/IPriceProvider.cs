using System;
using System.Collections.Generic;
using System.Text;

namespace price.PriceProviders
{
	public interface IPriceProvider
	{
		public decimal GetLatestPrice(string ticker, string convert);
		public void Configure(string[] parameters);
	}
}