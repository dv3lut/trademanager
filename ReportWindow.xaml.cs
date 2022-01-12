using System;
using System.Collections.Generic;
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
	/// Logique d'interaction pour ReportWindow.xaml
	/// </summary>
	public partial class ReportWindow : Window
	{
		public ReportWindow(Configuration config)
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// Send mail
			this.Close();
		}
	}
}
