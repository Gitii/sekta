using System.Reactive.Disposables;
using ReactiveUI;

namespace Sekta.Frontend.Wpf.Views.Presentation
{
    public partial class TextboxPresentationView
    {
        public TextboxPresentationView()
        {
            InitializeComponent();

            this.WhenActivated((disposable) =>
            {
                this.Bind(ViewModel,
                    (vm) => vm.Value,
                    (v) => v.TextBox.Text
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.Label,
                    (v) => v.LabelTextBlock.Text
                ).DisposeWith(disposable);
            });
        }
    }
}
