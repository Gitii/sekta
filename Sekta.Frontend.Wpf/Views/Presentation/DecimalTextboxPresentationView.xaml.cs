using System.Reactive.Disposables;
using ReactiveUI;

namespace Sekta.Frontend.Wpf.Views.Presentation;

public partial class DecimalTextboxPresentationView : System.Windows.Markup.IComponentConnector
{
    public DecimalTextboxPresentationView()
    {
        InitializeComponent();

        this.WhenActivated(
            (disposable) =>
            {
                this.Bind(ViewModel, (vm) => vm.Value, (v) => v.NumericUpDown.Value)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, (vm) => vm.MinValue, (v) => v.NumericUpDown.Minimum)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, (vm) => vm.MaxValue, (v) => v.NumericUpDown.Maximum)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, (vm) => vm.SpinStep, (v) => v.NumericUpDown.Increment)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, (vm) => vm.Label, (v) => v.LabelTextBlock.Text)
                    .DisposeWith(disposable);
            }
        );
    }
}
