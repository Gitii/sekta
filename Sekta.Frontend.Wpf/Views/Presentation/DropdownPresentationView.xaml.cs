using System.Reactive.Disposables;
using ReactiveUI;
using Sekta.Core.ModelView.Presentation;

namespace Sekta.Frontend.Wpf.Views.Presentation;

public partial class DropdownPresentationView
    : ReactiveUserControl<DropdownPresentationModelView>,
        System.Windows.Markup.IComponentConnector
{
    public DropdownPresentationView()
    {
        InitializeComponent();

        this.WhenActivated(
            (disposable) =>
            {
                this.OneWayBind(ViewModel, (vm) => vm.Items, (v) => v.ComboBox.ItemsSource)
                    .DisposeWith(disposable);

                this.Bind(
                        ViewModel,
                        (vm) => vm.SelectedItemIndex,
                        (v) => v.ComboBox.SelectedIndex,
                        (i) => (int)i,
                        (index) => index < 0 ? 0u : (uint)index
                    )
                    .DisposeWith(disposable);
            }
        );
    }
}
