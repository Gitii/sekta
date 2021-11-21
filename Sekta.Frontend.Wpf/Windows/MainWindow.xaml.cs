using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core;
using Sekta.Core.ModelView;
using Sekta.Frontend.Wpf.ModelViews;

namespace Sekta.Frontend.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<AppViewModel>, System.Windows.Markup.IComponentConnector
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new AppViewModel();

            LocalizedStringConverter localizedStringConverter = null;

            foreach (DictionaryEntry resource in Resources)
            {
                if (resource.Value is LocalizedStringConverter converter)
                {
                    localizedStringConverter = converter;
                }
            }

            this.WhenActivated((disposable) =>
            {
                this.BindCommand(ViewModel,
                    (vm) => vm.ResetSearchTermCommand,
                    (v) => v.ClearSearch
                ).DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    (vm) => vm.LoadConfigurationCommand,
                    (v) => v.LoadConfigButton
                ).DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    (vm) => vm.SaveConfigurationCommand,
                    (v) => v.SaveConfigButton
                ).DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    (vm) => vm.ExportConfigurationCommand,
                    (v) => v.ExportConfigButton
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.PolicyDefinitions.Categories,
                    (v) => v.Categories.ItemsSource
                ).DisposeWith(disposable);

                this.Bind(ViewModel,
                        (vm) => vm.CurrentCategory,
                        (v) => v.Categories.SelectedItem
                ).DisposeWith(disposable);

                ViewModel.WhenAnyValue((vm) => vm.CurrentCategory, (vm) => vm.SearchResults, (vm) => vm.IsSearchActive)
                    .Select((x =>
                    {
                        if (x.Item3)
                        {
                            if (x.Item2 == null)
                            {
                                return x.Item2;
                            }
                            ICollectionView view = CollectionViewSource.GetDefaultView(x.Item2);
                            view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                            return view as IEnumerable;
                        }
                        else
                        {
                            return x.Item1?.Policies as IEnumerable;
                        }
                    }))
                    .BindTo(this, (v) => v.Policies.ItemsSource)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.CurrentCategory.Policies,
                    (v) => v.Policies.ItemsSource
                ).DisposeWith(disposable);

                this.Bind(ViewModel,
                    (vm) => vm.CurrentPolicy,
                    (v) => v.Policies.SelectedItem
                ).DisposeWith(disposable);

                ViewModel.WhenAny((vm) => vm.CurrentResources, (x) => x).Subscribe((cr) =>
                {
                    if (cr.Value != null && localizedStringConverter != null)
                    {
                        localizedStringConverter.CurrentResources = cr.Value;
                    }
                });

                this.OneWayBind(ViewModel,
                    (vm) => vm.CurrentPolicy,
                    (v) => v.PolicyKeyTextBlock.Text,
                    (x) => x?.Key ?? string.Empty
                ).DisposeWith(disposable);

                this.WhenAnyValue((v) => v.ViewModel.CurrentPolicy, (v) => v.ViewModel.CurrentResources)
                    .Select((x) => x.Item1?.ExplainText.LocalizeWith(x.Item2) ?? string.Empty)
                    .BindTo(this, (v) => v.PolicyExplainTextBlock.Text);

                this.OneWayBind(ViewModel,
                    (vm) => vm.Resources,
                    (v) => v.LanguageMenuItem.ItemsSource
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.CurrentResources,
                    (v) => v.LanguageMenuItem.Header,
                    (x) => x == null ? "Language" : $"Language ({x.CultureEnglishName})"
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.Resources.Count,
                    (v) => v.LanguageMenuItem.IsEnabled,
                    (count) => count > 0
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.CurrentPolicyConfiguration,
                    (v) => v.PolicyConfigurationPresenter.ViewModel
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.CurrentPolicy,
                    (v) => v.PolicyConfigurationPresenter.Visibility,
                    (cp) => cp != null ? Visibility.Visible : Visibility.Hidden
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.SearchTerm,
                    (v) => v.ClearSearch.Visibility,
                    (x) => string.IsNullOrWhiteSpace(x) ? Visibility.Collapsed : Visibility.Visible
                ).DisposeWith(disposable);

                this.Bind(ViewModel,
                    (vm) => vm.SearchTerm,
                    (v) => v.SearchBox.Text
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.SearchTerm,
                    (v) => v.Categories.IsEnabled,
                    (x) => string.IsNullOrWhiteSpace(x)
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.PolicyDefinitions,
                    (v) => v.SearchBox.IsEnabled,
                    (x) => x != null
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    (vm) => vm.AreConfiguredPoliciesChanged,
                    (v) => v.Title,
                    (x) => $"Sekta - Admx/Adml Viewer {(x ? "*" : "")}"
                ).DisposeWith(disposable);

                //this.OneWayBind(ViewModel,
                //    (vm) => vm.ConfiguredPolicies,
                //    (v) => v.ExportToIntuneButton.IsEnabled,
                //    (x) => x != null && x.Count > 0
                //).DisposeWith(disposable);
            });

            ExitMenuItem.Click += (s, e) => Environment.Exit(0);
        }

        private void AdmxAdmlContextButton_OnClick(object sender, RoutedEventArgs e)
        {
            AdmxAdmlContext dlg = new AdmxAdmlContext();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                ViewModel.OpenFileCommand.Execute(new AdmxAndAdmlFiles(dlg.ViewModel.AdmxFilePath,
                    dlg.ViewModel.AdmlFilePaths));
            }
        }

        private void LanguageMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = ((MenuItem) e.OriginalSource);
            if (menuItem.Header is PolicyDefinitionResources res)
            {
                ViewModel.CurrentResources = res;
            }
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            AboutModelView modelView = new AboutModelView();
            var aboutWindow = new AboutWindow();
            modelView.UpdateWindow(aboutWindow);

            aboutWindow.ShowDialog();
        }

        private void ExportToIntune_OnClick(object sender, RoutedEventArgs e)
        {
            var w = new OMAViewer(ViewModel.ConfiguredPolicies.ToList(), ViewModel.PolicyDefinitionsContent, ViewModel.PolicyDefinitions);
            w.Owner = this;
            w.ShowDialog();
        }
    }
}
