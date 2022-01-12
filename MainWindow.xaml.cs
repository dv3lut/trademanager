using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Reactive.Linq;
using System.Windows.Threading;
using System.Windows.Resources;
using System.Net.Mail;
using System.Net;

namespace TradeManager
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		const string API_LINK = "https://fxmarketapi.com/apilive";
		public const string LOTSIZE_LINK = "https://www.cashbackforex.com/widgets/calculation/position-size";
		public static string API_KEY;
		public static HttpClient client = new HttpClient();
		public static ObservableCollection<Actif> rates = new ObservableCollection<Actif>() {
			new Actif("AUDCAD", 0.0001),
			new Actif("AUDCHF", 0.0001),
			new Actif("AUDJPY", 0.01),
			new Actif("AUDNZD", 0.0001),
			new Actif("AUDUSD", 0.0001),
			new Actif("CADCHF", 0.0001),
			new Actif("CADJPY", 0.01),
			new Actif("CHFJPY", 0.01),
			new Actif("EURAUD", 0.0001),
			new Actif("EURCAD", 0.0001),
			new Actif("EURCHF", 0.0001),
			new Actif("EURGBP", 0.0001),
			new Actif("EURJPY", 0.01),
			new Actif("EURNZD", 0.0001),
			new Actif("EURUSD", 0.0001),
			new Actif("GBPAUD", 0.0001),
			new Actif("GBPCAD", 0.0001),
			new Actif("GBPCHF", 0.0001),
			new Actif("GBPJPY", 0.01),
			new Actif("GBPNZD", 0.0001),
			new Actif("GBPUSD", 0.0001),
			new Actif("NZDCAD", 0.0001),
			new Actif("NZDCHF", 0.0001),
			new Actif("NZDJPY", 0.01),
			new Actif("NZDUSD", 0.0001),
			new Actif("USDAUD", 0.0001),
			new Actif("USDCAD", 0.0001),
			new Actif("USDCHF", 0.0001),
			new Actif("USDEUR", 0.0001),
			new Actif("USDGBP", 0.0001),
			new Actif("USDJPY", 0.01),
			new Actif("USDNZD", 0.0001)
		};
		public static Configuration configuration;
		public static Position position;


		public static void GetQuotes(ref ObservableCollection<Actif> actifs)
		{
			string MergeListOfCurrencies(ObservableCollection<Actif> actifs_)
			{
				string list = actifs_[0].Ticker;
				for (int i = 1; i < actifs_.Count; i++)
					list += "," + actifs_[i].Ticker;
				return list;
			}
			string currencies = MergeListOfCurrencies(actifs);
			Dictionary<string, double> quotes;
			try
			{
				quotes = JObject.Parse(GetRequest($"http://trademanager.fr/quotes").Result)["price"].ToObject<Dictionary<string, double>>();
			}
			catch (Exception e)
			{
				SendErrorMail();
				quotes = JObject.Parse(GetRequest($"{API_LINK}?api_key={configuration.GetAleaToken}&currency={currencies}").Result)["price"].ToObject<Dictionary<string, double>>();
			}
			foreach (Actif actif in actifs)
				actif.Quote = quotes[actif.Ticker];
		}

		#region Requests Functions
		public async static Task<string> GetRequest(string url)
		{
			var response = await client.GetAsync(url).ConfigureAwait(false);
			return await response.Content.ReadAsStringAsync();
		}

		public async static Task<string> PostRequest(string url, string parameters)
		{
			var httpRequestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(url)
			};

			httpRequestMessage.Content = new StringContent(parameters, Encoding.UTF8, "application/json");
			var response = await client.SendAsync(httpRequestMessage).ConfigureAwait(false);
			return await response.Content.ReadAsStringAsync();
		}
		#endregion



		#region Loading / Saving Configuration
		private static Configuration LoadConfiguration()
		{
			Configuration config = null;
			try
			{
				using (StreamReader file = File.OpenText("configuration.json"))
				{
					JsonSerializer serializer = new JsonSerializer();
					config = (Configuration)serializer.Deserialize(file, typeof(Configuration));
				}
			}
			catch (Exception e)
			{
				config = new Configuration(new Devise("€", "EUR", "euro"), 10000, 1, 3, new ObservableCollection<Actif>() { Actifs_("EURUSD") }, "Sombre", new List<string>() { "mtKDSf_o48z4IzurNmPt", "yNwj1pSqNXXAmhA_tDvA", "srStScv0biCOfcfiPgIo", "DPfErEetkddCcGPsia9g", "xHNMuV3fTASMYD4Jop6M" }); ;
			}

			string output = JsonConvert.SerializeObject(config);
			return config;
		}

		private static void SaveConfiguration(Configuration config)
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.NullValueHandling = NullValueHandling.Ignore;

			using (StreamWriter sw = new StreamWriter("configuration.json"))
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				serializer.Serialize(writer, config);
			}
		}
		#endregion

		public static void SendErrorMail()
		{
			try
			{

				SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
				smtpClient.EnableSsl = true;
				smtpClient.UseDefaultCredentials = false;
				smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtpClient.Credentials = new NetworkCredential("dorian110620@gmail.com", "qammtugyixdfoilw");
				MailAddress from = new MailAddress("dorian110620@gmail.com");
				MailAddress to = new MailAddress("dorian110620@gmail.com");
				MailMessage myMail = new System.Net.Mail.MailMessage(from, to);
				myMail.Subject = "SERVER DOWN ?";
				myMail.Body = "SERVER DOWN";
				myMail.IsBodyHtml = true;
				myMail.Priority = MailPriority.Normal;
				if (smtpClient != null)
				{
					smtpClient.Send(myMail);
					Console.WriteLine("mail send");
				}
				myMail.Dispose();
				smtpClient = null;
			}

			catch (SmtpException ex)
			{
				Console.WriteLine("SmtpException has occured: " + ex.Message);
				if (ex.InnerException != null)
				{
					Console.WriteLine("InnerException is: {0}", ex.InnerException);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static Actif Actifs_(string a)
		{
			foreach (Actif actif in MainWindow.rates)
				if (actif.Ticker == a)
					return actif;
			return null;
		}

		public MainWindow()
		{
			configuration = LoadConfiguration();
			this.DataContext = configuration;
			API_KEY = "5v8y/B?E(G+KbPeShVmYq3t6w9z$C&F)";
			client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", API_KEY);

			DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
			dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
			dispatcherTimer.Start();

			GetQuotes(ref rates);
			InitializeComponent();
			ComboBoxDevise.SelectedValue = configuration.Devise.Symbol;
			gridActifs.ItemsSource = configuration.Actifs;
			TextBoxStop.Text = String.Empty;
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			Debug.WriteLine("GetQuotes");
			GetQuotes(ref rates);
			// Forcing the CommandManager to raise the RequerySuggested event
			CommandManager.InvalidateRequerySuggested();
		}

		private bool CheckFields()
		{
			if (gridActifs.SelectedItem == null)
			{
				gridActifs.BorderBrush = Brushes.Red;
			}
			else if (double.TryParse(TextBoxStop.Text.Replace('.', ','), out double n) && n > 0 && double.TryParse(TextBoxRisqueDevise.Text.Replace('.', ','), out n) && n > 0 && double.TryParse(TextBoxCommissions.Text.Replace('.', ','), out n))
			{
				TextLotSize.Visibility = Visibility.Visible;
				TextLot.Visibility = Visibility.Visible;
				TextResume.Visibility = Visibility.Visible;
				TextEngage.Visibility = Visibility.Visible;
				TextCom.Visibility = Visibility.Visible;
				TextDetailResume.Visibility = Visibility.Visible;
				TextDetailEngage.Visibility = Visibility.Visible;
				TextDetailCom.Visibility = Visibility.Visible;
				return true;
			}
			TextLotSize.Visibility = Visibility.Hidden;
			TextLot.Visibility = Visibility.Hidden;
			TextResume.Visibility = Visibility.Hidden;
			TextEngage.Visibility = Visibility.Hidden;
			TextCom.Visibility = Visibility.Hidden;
			TextDetailResume.Visibility = Visibility.Hidden;
			TextDetailEngage.Visibility = Visibility.Hidden;
			TextDetailCom.Visibility = Visibility.Hidden;
			return false;
		}
		private void Calculation()
		{
			position = new Position(configuration.RisqueMonetaire, configuration.StopSize, gridActifs.SelectedItem as Actif, configuration.Devise, configuration.Commissions);
			TextLotSize.DataContext = position;
			TextDetailResume.DataContext = position;
			TextDetailEngage.DataContext = position;
			TextDetailCom.DataContext = position;
		}


		private void Win_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var value = (Win.Width + Win.Height) / 2;
			//TextTheme.FontSize = 11 * value / 650;
			//TextTheme.Margin = new Thickness(0, 0, 20 * value / 650, 0);
			//ComboBoxTheme.Height = 20 * value / 650;
			//ComboBoxTheme.Width = 75 * value / 650;
			//ComboBoxTheme.FontSize = 11 * value / 650;
			TextDevise.FontSize = 17 * value / 650;
			ComboBoxDevise.Height = 25 * value / 650;
			ComboBoxDevise.Width = 35 * value / 650;
			ComboBoxDevise.FontSize = 17 * value / 650;
			TextCapital.FontSize = 17 * value / 650;
			VBoxTextBoxCapital.Height = 20 * value / 650;
			TextBoxCapital.FontSize = 17 * value / 650;
			TextCapitalDevise.FontSize = 17 * value / 650;
			TextRisque.FontSize = 17 * value / 650;
			VBoxTextBoxRisqueDevise.Height = 20 * value / 650;
			TextBoxRisqueDevise.FontSize = 17 * value / 650;
			TextRisqueDevise.FontSize = 17 * value / 650;
			VBoxTextBoxRisquePourcent.Height = 20 * value / 650;
			TextBoxRisquePourcent.FontSize = 17 * value / 650;
			TextRisquePourcent.FontSize = 17 * value / 650;
			TextCommissions.FontSize = 17 * value / 650;
			VBoxTextBoxCommissions.Height = 20 * value / 650;
			TextBoxCommissions.FontSize = 17 * value / 650;
			TextCommissionsUnit.FontSize = 17 * value / 650;
			gridActifs.FontSize = 13 * value / 650;
			gridActifs.RowHeight = 34 * value / 650;
			TextStop.FontSize = 17 * value / 650;
			VBoxTextBoxStop.Height = 20 * value / 650;
			TextPips.FontSize = 11 * value / 650;
			TextLotSize.FontSize = 35 * value / 650;
			TextLot.FontSize = 12 * value / 650;
			TextResume.FontSize = 15 * value / 650;
			TextEngage.FontSize = 15 * value / 650;
			TextCom.FontSize = 15 * value / 650;
			VBoxTextDetailResume.Height = 22 * value / 650;

			VBoxTextDetailEngage.Height = 22 * value / 650;

			VBoxTextDetailCom.Height = 22 * value / 650;

		}


		#region Themes
		public void BlackTheme()
		{
			ComboBoxDevise.Foreground = Brushes.White;
			Win.Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
			string logoImg = "pack://application:,,,/TradeManager;component/Images/logo_white.png";
			image_logo.Source = new ImageSourceConverter().ConvertFromString(logoImg) as ImageSource;
			TextDevise.Foreground = Brushes.White;
			TextCapital.Foreground = Brushes.White;
			TextBoxCapital.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
			TextCapitalDevise.Foreground = Brushes.White;
			TextRisque.Foreground = Brushes.White;
			TextBoxRisqueDevise.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
			TextRisqueDevise.Foreground = Brushes.White;
			TextBoxRisquePourcent.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
			TextRisquePourcent.Foreground = Brushes.White;
			TextCommissions.Foreground = Brushes.White;
			TextBoxCommissions.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
			TextCommissionsUnit.Foreground = Brushes.White;
			TextStop.Foreground = Brushes.White;
			TextPips.Foreground = Brushes.White;
			TextBoxStop.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
			TextLotSize.Foreground = Brushes.YellowGreen;
			TextLot.Foreground = Brushes.YellowGreen;
			TextResume.Foreground = Brushes.YellowGreen;
			TextEngage.Foreground = Brushes.YellowGreen;
			TextCom.Foreground = Brushes.YellowGreen;
			TextDetailResume.Foreground = Brushes.White;
			TextDetailEngage.Foreground = Brushes.White;
			TextDetailCom.Foreground = Brushes.White;
		}
		#endregion


		#region ComboBox Events Selection Changed
		private void ComboBoxDevise_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBoxItem _theme = (ComboBoxItem)ComboBoxDevise.SelectedItem;
			string theme = _theme.Content.ToString();
			configuration.Devise.Symbol = theme;
			configuration.DeviseSymbol = theme;
			if (theme == "€")
			{
				configuration.Devise.Alias = "EUR";
				configuration.Devise.Name = "euro";
			}
			else
			{
				configuration.Devise.Alias = "USD";
				configuration.Devise.Name = "dollar";
			}
			TextBoxStop.Focus();

		}
		#endregion


		#region Events Focus On Hidden TBox
		private void Win_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!TextBoxStop.IsFocused)
				TextBoxStop.Focus();
			else
				TextBoxHidden.Focus();
		}

		private void TextBoxCapital_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				TextBoxStop.Focus();
		}
		private void TextBoxRisquePourcent_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				TextBoxStop.Focus();
		}


		private void TextBoxCommissions_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				TextBoxStop.Focus();
		}

		private void TextBoxRisqueDevise_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				TextBoxStop.Focus();
		}
		private void TextBoxStop_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				TextBoxHidden.Focus();
		}
	

		private void gridActifs_MouseUp(object sender, MouseButtonEventArgs e)
		{
			gridActifs.BorderBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
			if (CheckFields())
			{
				Calculation();
			}
			TextBoxStop.Focus();
		}
		#endregion


		#region Events Lost Focus Calculations
		private void TextBoxStop_LostFocus(object sender, RoutedEventArgs e)
		{
			if (CheckFields())
			{
				configuration.StopSize = double.Parse(TextBoxStop.Text.Replace('.', ','));
				Calculation();
			}
		}

		private void TextBoxCapital_LostFocus(object sender, RoutedEventArgs e)
		{
			if (CheckFields())
			{
				configuration.Capital = double.Parse(TextBoxCapital.Text.Replace('.', ','));
				Calculation();
			}
		}

		private void TextBoxRisquePourcent_LostFocus(object sender, RoutedEventArgs e)
		{
			if (CheckFields())
			{
				configuration.RisquePourcent = double.Parse(TextBoxRisquePourcent.Text.Replace('.', ','));
				Calculation();
			}
		}

		private void TextBoxRisqueDevise_LostFocus(object sender, RoutedEventArgs e)
		{
			if (CheckFields())
			{
				configuration.RisqueMonetaire = double.Parse(TextBoxRisqueDevise.Text.Replace('.', ','));
				Calculation();
			}
		}

		private void ComboBoxDevise_LostFocus(object sender, RoutedEventArgs e)
		{
			if (CheckFields())
			{
				Calculation();
			}
		}

		private void TextBoxCommissions_LostFocus(object sender, RoutedEventArgs e)
		{
			if (CheckFields())
			{
				configuration.Commissions = double.Parse(TextBoxCommissions.Text.Replace('.', ','));
				Calculation();
			}
		}
		#endregion


		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveConfiguration(configuration as Configuration);
		}

		private void ButtonConfigActifs_Click(object sender, RoutedEventArgs e)
		{
			ConfigActifsWindow win = new ConfigActifsWindow(ref configuration);
			win.Show();
		}
		private void ButtonConfigActifs_MouseEnter(object sender, MouseEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/engrenage_rouge.png";
			ButtonConfigActifs.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}

		private void ButtonConfigActifs_MouseLeave(object sender, MouseEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/engrenage_blanc.png";
			ButtonConfigActifs.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}

		private void ButtonReport_Click(object sender, RoutedEventArgs e)
		{
			ReportWindow win = new ReportWindow(configuration);
			win.Show();

		}
	}
}
