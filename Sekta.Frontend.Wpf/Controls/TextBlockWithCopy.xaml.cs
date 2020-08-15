using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sekta.Frontend.Wpf.Controls
{
    public partial class TextBlockWithCopy : UserControl
    {
        public TextBlockWithCopy()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(TextBlockWithCopy), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty CopyiedProperty = DependencyProperty.Register(
            "Copyied", typeof(bool), typeof(TextBlockWithCopy), new PropertyMetadata(default(bool)));

        public bool Copyied
        {
            get { return (bool) GetValue(CopyiedProperty); }
            set { SetValue(CopyiedProperty, value); }
        }

        private async void CopyButton_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Text ?? "");
            Copyied = true;

            CopyBtn.Content = "Copied!";

            await Task.Delay(2000).ConfigureAwait(true);

            CopyBtn.Content = "Copy";
        }
    }
}
