using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;
using Sekta.Core.ModelView;
#pragma warning disable MA0051

namespace Sekta.Frontend.Wpf.Views;

public partial class PolicyConfigurationView
{
    public PolicyConfigurationView()
    {
        InitializeComponent();

        this.WhenActivated(
            (disposable) =>
            {
                ViewModel = (ViewModel ?? DataContext) as PolicyConfigurationModelView;

                if (ViewModel == null)
                {
                    return;
                }

                this.WhenAnyValue(x => x.ViewModel.IsEnabled)
                    .Subscribe(someBoolean =>
                    {
                        if (someBoolean == true)
                        {
                            EnabledRadioButton.IsChecked = true;
                        }
                        else if (someBoolean == false)
                        {
                            DisabledRadioButton.IsChecked = true;
                        }
                        else
                        {
                            NotConfiguredRadioButton.IsChecked = true;
                        }
                    });

                this.WhenAnyValue(
                        x => x.EnabledRadioButton.IsChecked,
                        x => x.DisabledRadioButton.IsChecked,
                        x => x.NotConfiguredRadioButton.IsChecked
                    )
                    .Subscribe(values =>
                    {
                        var isEnabled = values.Item1;
                        var isDisabled = values.Item2;
                        var notConfigured = values.Item3;

                        if (
                            isEnabled.GetValueOrDefault()
                            && ViewModel.IsEnabled.GetValueOrDefault() != true
                        )
                        {
                            ViewModel.IsEnabled = true;
                            ViewModel.IsDataChanged = true;
                        }

                        if (
                            isDisabled.GetValueOrDefault()
                            && ViewModel.IsEnabled.GetValueOrDefault(true) != false
                        )
                        {
                            ViewModel.IsEnabled = false;
                            ViewModel.IsDataChanged = true;
                        }

                        if (notConfigured.GetValueOrDefault() && ViewModel.IsEnabled.HasValue)
                        {
                            ViewModel.IsEnabled = null;
                            ViewModel.IsDataChanged = true;
                        }
                    });

                this.OneWayBind(ViewModel, (vm) => vm.ValidationText, (v) => v.ValidationText.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(
                        ViewModel,
                        (vm) => vm.IsEnabled,
                        (v) => v.PresentationElements.Visibility,
                        (x) => x.GetValueOrDefault() ? Visibility.Visible : Visibility.Collapsed
                    )
                    .DisposeWith(disposable);

                this.OneWayBind(
                        ViewModel,
                        (vm) => vm.IsDataChanged,
                        (v) => v.ButtonPanel.Visibility,
                        (x) => x ? Visibility.Visible : Visibility.Collapsed
                    )
                    .DisposeWith(disposable);

                PresentationElements.ItemsSource = ViewModel.PresentationElements;

                this.BindCommand(ViewModel, (vm) => vm.ApplyChangesCommand, (v) => v.ApplyButton)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, (vm) => vm.RevertChangesCommand, (v) => v.RevertButton)
                    .DisposeWith(disposable);

                ViewModel.RevertChangesCommand.Execute(Unit.Default).Subscribe();
            }
        );
    }
}
