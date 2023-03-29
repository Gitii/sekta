using System.Reactive.Disposables;
using ReactiveUI;
using Sekta.Core.ModelView.Presentation;

namespace Sekta.Frontend.Wpf.Views.Presentation;

public partial class ListboxPresentationView
    : ReactiveUserControl<ListboxPresentationModelView>,
        System.Windows.Markup.IComponentConnector
{
    public ListboxPresentationView()
    {
        InitializeComponent();

        this.WhenActivated(
            (disposable) =>
            {
                this.OneWayBind(ViewModel, (vm) => vm.Items, (v) => v.Items.ItemsSource)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, (vm) => vm.AddItemCommand, (v) => v.AddItemButton)
                    .DisposeWith(disposable);
            }
        );
    }
}
