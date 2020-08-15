using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Sekta.Frontend.Wpf.ModelViews
{
    public class AboutModelView : ReactiveObject
	{
		private ImageSource _applicationLogo;
		private string _title;
		private string _description;
		private string _version;
		private ImageSource _publisherLogo;
		private string _copyright;
		private string _additionalNotes;
		private string _hyperlinkText;
		private Uri _hyperlink;
		private string _publisher;

        public AboutModelView()
		{
            Assembly assembly = Assembly.GetEntryAssembly() ?? throw new ArgumentNullException();
            AssemblyName assemblyName = assembly.GetName();

            Version = assemblyName.Version?.ToString() ?? "Unknown";
			Title = assemblyName.Name;

            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
			Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
			Publisher = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

			ApplicationLogo = new BitmapImage(new Uri(@"pack://application:,,,/policy.png", UriKind.Absolute));
		}

        public void UpdateWindow(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowStyle = WindowStyle.ToolWindow;

            window.ShowInTaskbar = false;
            window.Title = "About";
        }

        public ImageSource ApplicationLogo
        {
            get { return _applicationLogo; }
            set { this.RaiseAndSetIfChanged(ref _applicationLogo, value); }
        }

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }

        public string Version
        {
            get { return _version; }
            set { this.RaiseAndSetIfChanged(ref _version, value); }
        }

        public ImageSource PublisherLogo
        {
            get { return _publisherLogo; }
            set { this.RaiseAndSetIfChanged(ref _publisherLogo, value); }
        }

        public string Publisher
        {
            get { return _publisher; }
            set { this.RaiseAndSetIfChanged(ref _publisher, value); }
        }

        public string Copyright
        {
            get { return _copyright; }
            set { this.RaiseAndSetIfChanged(ref _copyright, value); }
        }

        public string HyperlinkText
        {
            get { return _hyperlinkText; }
            set { this.RaiseAndSetIfChanged(ref _hyperlinkText, value); }
        }

        public Uri Hyperlink
        {
            get { return _hyperlink; }
            set { this.RaiseAndSetIfChanged(ref _hyperlink, value); }
        }

        public string AdditionalNotes
        {
            get { return _additionalNotes; }
            set { this.RaiseAndSetIfChanged(ref _additionalNotes, value); }
        }
	}
}
