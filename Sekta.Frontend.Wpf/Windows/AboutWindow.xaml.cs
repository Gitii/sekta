using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Sekta.Frontend.Wpf.Windows
{
    /// <summary>
    /// Interaktionslogik für AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string navigateUri = e.Uri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}