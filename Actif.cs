using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeManager
{
	public class Actif
	{
		private string ticker;
		private double pipSize;
		private double quote;

		public Actif(string ticker, double pipSize)
		{
			this.ticker = ticker;
			this.pipSize = pipSize;
		}

		public string Ticker
		{
			get => ticker;
		}
		public double PipSize
		{
			get => pipSize;
		}

		public double Quote
		{
			get => quote;
			set
			{
				this.quote = value;
			}
		}


		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
