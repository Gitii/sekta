using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;
using Sekta.Core.ModelView.Intune;
using Sekta.Core.ModelView.Presentation;
using Sekta.Core.Schema;

namespace Sekta.Frontend.Wpf.Windows
{
    public partial class OMAViewer : ReactiveWindow<OMAModelView>, System.Windows.Markup.IComponentConnector
    {
        public OMAViewer()
        {
            InitializeComponent();

            ViewModel = null;
        }

        public OMAViewer(List<ConfiguredPolicy> configuredPolicies, string admxFileContent, AdmxPolicyDefinitions definitions)
        {
            InitializeComponent();

            ViewModel = new OMAModelView(configuredPolicies, admxFileContent, definitions);

            this.WhenActivated((disposeable) =>
            {
                this.OneWayBind(ViewModel,
                        (vm) => vm.Entries,
                        (v) => v.Items.ItemsSource)
                    .DisposeWith(disposeable);

                this.Bind(ViewModel,
                        (vm) => vm.ApplicationName,
                        (v) => v.ApplicationName.Text)
                    .DisposeWith(disposeable);

                this.Bind(ViewModel,
                        (vm) => vm.AdmxFileName,
                        (v) => v.FileName.Text)
                    .DisposeWith(disposeable);
            });
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
