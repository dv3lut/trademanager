using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeManager
{
	public class Position : INotifyPropertyChanged
	{
		private double moneyAtRisk;
		private double pipAtRisk;
		private Actif currencie;
		private string rate;
		private double rateValue;
		private Devise devise;
		private double commissionPerLot;
		private double lotSizeBeforeCom;
		private double lotSizeAfterCom;
		private double percentageOfMoneyEngage;
		public event PropertyChangedEventHandler PropertyChanged;


		public Position(double moneyAtRisk, double pipAtRisk, Actif currencie, Devise devise, double commissionPerLot)
		{
			this.moneyAtRisk = moneyAtRisk;
			this.pipAtRisk = pipAtRisk;
			this.currencie = currencie;
			this.devise = devise;
			this.commissionPerLot = commissionPerLot;
			this.rate = "1";
			if (!currencie.Ticker.Contains(devise.Alias) || currencie.Ticker.StartsWith(devise.Alias))
				this.rate = devise.Alias + currencie.Ticker.Substring(3, 3);
			this.lotSizeBeforeCom = this.GetLotSize();
			double commissionValue = this.commissionPerLot * this.lotSizeBeforeCom;
			double ajustement = 1;
			if (this.devise.Symbol == "€")
				ajustement = MainWindow.Actifs_("EURUSD").Quote;
			commissionValue /= ajustement;
			this.lotSizeAfterCom = Math.Round(this.moneyAtRisk * this.lotSizeBeforeCom / (this.moneyAtRisk + commissionValue), 3);
			this.percentageOfMoneyEngage = Math.Round(lotSizeAfterCom / lotSizeBeforeCom, 3);
		}

		public double GetLotSize()
		{
			double rateValue = 1;
			if (this.rate != "1")
				rateValue = MainWindow.Actifs_(this.Rate).Quote;
			this.rateValue = rateValue;


			string parameters = "{ \"moneyToRisk\": " + this.MoneyAtRisk + ", \"pipAtRisk\": " + this.PipAtRisk + ", \"lotSize\": 1, \"onePipSize\": " + (MainWindow.Actifs_(currencie.Ticker).PipSize * 100000).ToString() + ", \"rate\": " + rateValue.ToString().Replace(',', '.') + " }";
			return JObject.Parse(MainWindow.PostRequest(MainWindow.LOTSIZE_LINK, parameters).Result)["lots"].ToObject<double>();
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Actif Currencie
		{
			get => this.currencie;
		}
		public Devise Devise
		{
			get => this.devise;
		}
		public string MoneyAtRisk
		{
			get => this.moneyAtRisk.ToString().Replace(',', '.');
		}
		public string PipAtRisk
		{
			get => this.pipAtRisk.ToString().Replace(',', '.');
		}
		public string Rate
		{
			get => this.rate.ToString().Replace(',', '.');
		}

		public string RateValue
		{
			get => this.rateValue.ToString().Replace(',', '.');
		}

		public string LotSizeBeforeCom
		{
			get => Math.Round(this.lotSizeBeforeCom, 3).ToString().Replace(',', '.');
		}
		public string LotSizeAfterCom
		{
			get => Math.Round(this.lotSizeAfterCom, 3).ToString().Replace(',', '.');
		}
		public string PercentageOfMoneyEngage
		{
			get => Math.Round(this.percentageOfMoneyEngage * 100, 2).ToString().Replace(',', '.');
		}
		public string MoneyEngageValue
		{
			get => Math.Round(this.percentageOfMoneyEngage * this.moneyAtRisk, 2).ToString().Replace(',', '.');
		}
		public string PercentageOfCommission
		{
			get => Math.Round((1 - this.percentageOfMoneyEngage) * 100, 2).ToString().Replace(',', '.');
		}
		public string CommissionValue
		{
			get => Math.Round((1 - this.percentageOfMoneyEngage) * this.moneyAtRisk, 2).ToString().Replace(',', '.');
		}

		public string Resume
		{
			get
			{
				return String.Format($"{this.MoneyAtRisk} {this.devise.Symbol} - {this.Currencie.Ticker} - {this.PipAtRisk} pips").Replace(',', '.');
			}
			set
			{
				OnPropertyChanged("Resume");
			}
		}

		public string Engage
		{
			get
			{
				return String.Format($"{this.MoneyEngageValue} {this.devise.Symbol}  -  {this.PercentageOfMoneyEngage} %").Replace(',', '.');
			}
			set
			{
				OnPropertyChanged("Engage");
			}
		}

		public string Com
		{
			get
			{
				return String.Format($"{this.CommissionValue} {this.devise.Symbol}  -  {this.PercentageOfCommission} %").Replace(',', '.');
			}
			set
			{
				OnPropertyChanged("Com");
			}
		}


		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}