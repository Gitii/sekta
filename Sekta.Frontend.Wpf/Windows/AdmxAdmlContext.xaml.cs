using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;
using Sekta.Core.ModelView;

namespace Sekta.Frontend.Wpf.Windows;

/// <summary>
/// Interaktionslogik für AdmxAdmlContext.xaml
/// </summary>
public partial class AdmxAdmlContext
    : ReactiveWindow<AdmxAdmlContextViewModel>,
        System.Windows.Markup.IComponentConnector
{
    public AdmxAdmlContext()
    {
        InitializeComponent();

        ViewModel = new AdmxAdmlContextViewModel();

        this.WhenActivated(
            (disposeable) =>
            {
                this.BindCommand(
                        ViewModel,
                        (vm) => vm.SelectAdmxFileCommand,
                        (v) => v.SelectAdmxFileButton
                    )
                    .DisposeWith(disposeable);

                this.BindCommand(
                        ViewModel,
                        (vm) => vm.SelectMoreAdmlFilesCommand,
                        (v) => v.SelectAdmlFilesButton
                    )
                    .DisposeWith(disposeable);

                this.BindCommand(
                        ViewModel,
                        (vm) => vm.RemoveSelectedAdmlFileCommand,
                        (v) => v.RemoveSelectedAdmlFile
                    )
                    .DisposeWith(disposeable);

                this.OneWayBind(
                        ViewModel,
                        (vm) => vm.SelectedAdmlFilePath,
                        (v) => v.RemoveSelectedAdmlFile.IsEnabled,
                        (x) => x != null
                    )
                    .DisposeWith(disposeable);

                this.OneWayBind(
                        ViewModel,
                        (vm) => vm.AdmxFilePath,
                        (v) => v.AdmxFilePathTextBox.Text
                    )
                    .DisposeWith(disposeable);

                this.OneWayBind(
                        ViewModel,
                        (vm) => vm.AdmlFilePaths,
                        (v) => v.AdmlFilePaths.ItemsSource
                    )
                    .DisposeWith(disposeable);

                this.Bind(
                        ViewModel,
                        (vm) => vm.SelectedAdmlFilePath,
                        (v) => v.AdmlFilePaths.SelectedItem
                    )
                    .DisposeWith(disposeable);

                this.OneWayBind(ViewModel, (vm) => vm.IsAllValid, (v) => v.OkButton.IsEnabled)
                    .DisposeWith(disposeable);

                this.Bind(
                        ViewModel,
                        (vm) => vm.AutoAddAdmlFiles,
                        (v) => v.AutoAddAdmlFilesCheckBox.IsChecked
                    )
                    .DisposeWith(disposeable);
            }
        );
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
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
