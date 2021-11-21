using System.Reactive.Disposables;
using ReactiveUI;

namespace Sekta.Frontend.Wpf.Views.Presentation
{
    public partial class TextPresentationView: System.Windows.Markup.IComponentConnector
    {
        public TextPresentationView()
        {
            InitializeComponent();

            this.WhenActivated((disposable) =>
            {
                this.OneWayBind(ViewModel,
                    (vm) => vm.Label,
                    (v) => v.LabelTextBlock.Text
                ).DisposeWith(disposable);
            });
        }
    }
}
