using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeManager
{
	[Serializable]
	public class Devise
	{
		private string symbol;
		private string alias;
		private string name;

		public Devise(string symbol, string alias, string name)
		{
			this.symbol = symbol;
			this.alias = alias.ToUpper();
			this.name = name;
		}

		public string Symbol
		{
			get => this.symbol;
			set { this.symbol = value; }
		}
		public string Alias
		{
			get => this.alias;
			set { this.alias = value; }
		}
		public string Name
		{
			get => this.name;
			set { this.name = value; }
		}

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
