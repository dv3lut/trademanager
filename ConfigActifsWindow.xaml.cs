using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TradeManager
{
	/// <summary>
	/// Logique d'interaction pour ConfigActifsWindow.xaml
	/// </summary>
	public partial class ConfigActifsWindow : Window
	{
		public static ObservableCollection<Actif> actifsDispo = new ObservableCollection<Actif>() {
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
			new Actif("USDCAD", 0.0001),
			new Actif("USDCHF", 0.0001),
			new Actif("USDJPY", 0.01)
		};
		Configuration configuration;
		private void UpdateActifsDispo(ref ObservableCollection<Actif> actifsDispo)
		{
			for (int i = 0; i < actifsDispo.Count; i++)
				foreach (Actif actifFavori in this.configuration.Actifs)
					if (actifsDispo[i].Ticker == actifFavori.Ticker)
					{
						actifsDispo.Remove(actifsDispo[i]);
						i--;
						break;
					}
		}

		public ConfigActifsWindow(ref Configuration configuration)
		{
			this.configuration = configuration;
			InitializeComponent();
			UpdateActifsDispo(ref actifsDispo);
			gridFavoris.ItemsSource = configuration.Actifs;
			gridDisponibles.ItemsSource = actifsDispo;
		}


		#region Functions multiple selections
		private void MouseEnterHandler(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed &&
			    e.OriginalSource is DataGridRow row)
			{
				row.IsSelected = !row.IsSelected;
				e.Handled = true;
			}
		}

		private void PreviewMouseDownHandler(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed &&
			    e.OriginalSource is FrameworkElement element &&
			    GetVisualParentOfType<DataGridRow>(element) is DataGridRow row)
			{
				row.IsSelected = !row.IsSelected;
				e.Handled = true;
			}
		}

		private static DependencyObject GetVisualParentOfType<T>(DependencyObject startObject)
		{
			DependencyObject parent = startObject;

			while (IsNotNullAndNotOfType<T>(parent))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}

			return parent is T ? parent : throw new Exception($"Parent of type {typeof(T)} could not be found");
		}

		private static bool IsNotNullAndNotOfType<T>(DependencyObject obj)
		{
			return obj != null && !(obj is T);
		}
		#endregion

		private void ImgFavToDispo_MouseEnter(object sender, MouseEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/arrow_right_green.png";
			ImgFavToDispo.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}

		private void ImgFavToDispo_MouseLeave(object sender, MouseEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/arrow_right_yellowgreen.png";
			ImgFavToDispo.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}

		private void ImgFavToDispo_Click(object sender, RoutedEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/arrow_right_blue.png";
			ImgFavToDispo.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
			for (int i = 0; i < gridFavoris.SelectedItems.Count; i++)
			{
				Actif item = gridFavoris.SelectedItems[i] as Actif;
				actifsDispo.Add(item);
				configuration.Actifs.Remove(item);
				i--;
			}
			actifsDispo = new ObservableCollection<Actif>(actifsDispo.OrderBy(i => i.Ticker));
			configuration.Actifs = new ObservableCollection<Actif>(configuration.Actifs.OrderBy(i => i.Ticker));
			gridFavoris.ItemsSource = configuration.Actifs;
			gridDisponibles.ItemsSource = actifsDispo;
			packUri = "pack://application:,,,/TradeManager;component/Images/arrow_right_yellowgreen.png";
			ImgFavToDispo.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}


		private void ImgDispoToFav_MouseEnter(object sender, MouseEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/arrow_left_green.png";
			ImgDispoToFav.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}

		private void ImgDispoToFav_MouseLeave(object sender, MouseEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/arrow_left_yellowgreen.png";
			ImgDispoToFav.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}

		private void ImgDispoToFav_Click(object sender, RoutedEventArgs e)
		{
			string packUri = "pack://application:,,,/TradeManager;component/Images/arrow_left_blue.png";
			ImgDispoToFav.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
			for (int i = 0; i < gridDisponibles.SelectedItems.Count; i++)
			{
				Actif item = gridDisponibles.SelectedItems[i] as Actif;
				actifsDispo.Remove(item);
				configuration.Actifs.Add(item);
				i--;
			}
			actifsDispo = new ObservableCollection<Actif>(actifsDispo.OrderBy(i => i.Ticker));
			configuration.Actifs = new ObservableCollection<Actif>(configuration.Actifs.OrderBy(i => i.Ticker));
			gridFavoris.ItemsSource = configuration.Actifs;
			gridDisponibles.ItemsSource = actifsDispo;
			packUri = "pack://application:,,,/TradeManager;component/Images/arrow_left_yellowgreen.png";
			ImgDispoToFav.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
		}

		private void TextAll_MouseUp(object sender, MouseButtonEventArgs e)
		{
			for (int i = 0; i < gridDisponibles.Items.Count; i++)
			{
				Actif item = gridDisponibles.Items[i] as Actif;
				actifsDispo.Remove(item);
				configuration.Actifs.Add(item);
				i--;
			}
			actifsDispo = new ObservableCollection<Actif>(actifsDispo.OrderBy(i => i.Ticker));
			configuration.Actifs = new ObservableCollection<Actif>(configuration.Actifs.OrderBy(i => i.Ticker));
			gridFavoris.ItemsSource = configuration.Actifs;
			gridDisponibles.ItemsSource = actifsDispo;
		}

		private void ImgPoubelle_MouseUp(object sender, MouseButtonEventArgs e)
		{
			for (int i = 0; i < gridFavoris.Items.Count; i++)
			{
				Actif item = gridFavoris.Items[i] as Actif;
				actifsDispo.Add(item);
				configuration.Actifs.Remove(item);
				i--;
			}
			actifsDispo = new ObservableCollection<Actif>(actifsDispo.OrderBy(i => i.Ticker));
			configuration.Actifs = new ObservableCollection<Actif>(configuration.Actifs.OrderBy(i => i.Ticker));
			gridFavoris.ItemsSource = configuration.Actifs;
			gridDisponibles.ItemsSource = actifsDispo;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
