using System.Reactive.Disposables;
using ReactiveUI;
using Sekta.Core.ModelView.Presentation;

namespace Sekta.Frontend.Wpf.Views.Presentation
{
    public partial class CheckboxPresentationView : ReactiveUserControl<CheckboxPresentationModelView>
    {
        public CheckboxPresentationView()
        {
            InitializeComponent();

            this.WhenActivated((disposable) =>
            {
                this.Bind(ViewModel,
                    (vm) => vm.IsChecked,
                    (v) => v.CheckBox.IsChecked
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.PresentationElement.Value,
                    (v) => v.CheckBox.Content
                ).DisposeWith(disposable);
            });
        }
    }
}
