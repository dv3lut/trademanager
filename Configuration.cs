using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Newtonsoft.Json;

namespace TradeManager
{

	[Serializable]
	public class Configuration: INotifyPropertyChanged
	{
		private Devise devise;
		private double capital;
		private double risquePourcent;
		private double risqueMonetaire;
		private double commissions;
		private ObservableCollection<Actif> actifs = new ObservableCollection<Actif>();
		private string theme;
		private double stopSize;
		private List<string> tokens;
		public event PropertyChangedEventHandler PropertyChanged;

		public Configuration(Devise devise, double capital, double risquePourcent, double commissions, ObservableCollection<Actif> actifs, string theme, List<string> tokens)
		{
			this.devise = devise;
			this.capital = capital;
			this.risquePourcent = risquePourcent;
			this.risqueMonetaire = this.capital * this.risquePourcent / 100;
			this.commissions = commissions;
			this.actifs = actifs;
			this.theme = theme;
			this.tokens = tokens;
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Devise Devise
		{
			get => this.devise;
			set
			{
				this.devise = value;
				OnPropertyChanged("Devise");
			}
		}
		public string DeviseSymbol
		{
			get => this.devise.Symbol;
			set
			{
				this.devise.Symbol = value;
				OnPropertyChanged("DeviseSymbol");
			}
		}

		public double Capital
		{
			get => this.capital;
			set
			{
				capital = value;
				this.risqueMonetaire = this.capital * this.risquePourcent / 100;
				OnPropertyChanged("Capital");
				OnPropertyChanged("RisqueMonetaire");
			}
		}

		public double RisquePourcent
		{
			get => this.risquePourcent;
			set
			{
				this.risquePourcent = value;
				this.risqueMonetaire = this.capital * this.risquePourcent / 100;
				OnPropertyChanged("RisquePourcent");
				OnPropertyChanged("RisqueMonetaire");
			}
		}

		public double RisqueMonetaire
		{
			get => this.risqueMonetaire;
			set
			{
				this.risqueMonetaire = value;
				this.risquePourcent = this.risqueMonetaire * 100 / this.capital;
				OnPropertyChanged("RisquePourcent");
				OnPropertyChanged("RisqueMonetaire");
			}
		}
		public double Commissions
		{
			get => this.commissions;
			set
			{
				commissions = value;
				OnPropertyChanged("Commissions");
			}
		}
		public ObservableCollection<Actif> Actifs
		{
			get => this.actifs;
			set
			{
				actifs = value;
				OnPropertyChanged("Actifs");
			}
		}
		public string Theme
		{
			get => this.theme;
			set
			{
				theme = value;
				OnPropertyChanged("Theme");
			}
		}

		public double StopSize
		{
			get => this.stopSize;
			set
			{
				stopSize = value;
				OnPropertyChanged("StopSize");
			}
		}

		public List<string> Tokens
		{
			get => this.tokens;
		}

		public string GetAleaToken
		{
			get
			{
				Random rnd = new Random();
				return this.tokens[rnd.Next(this.Tokens.Count)];
			}
		}




		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
