using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.IO;
using Sekta.Core.ModelView.Presentation;
using Sekta.Core.Schema;
using Splat;
#pragma warning disable MA0051

namespace Sekta.Core.ModelView;

public class AppViewModel : ReactiveObject
{
    readonly ObservableAsPropertyHelper<bool> _areConfiguredPoliciesChanged;

    private readonly ObservableAsPropertyHelper<PolicyConfigurationModelView> _currentPolicyConfiguration;

    readonly ObservableAsPropertyHelper<bool> _isConfigurationReady;

    private readonly ObservableAsPropertyHelper<bool> _isSearchActive;
    private readonly ReadOnlyObservableCollection<PolicyDefinitionResources> _resources;

    private readonly ObservableAsPropertyHelper<IEnumerable<AdmxPolicy>> _searchResults;

    private ReadOnlyObservableCollection<ConfiguredPolicy> _configuredPolicies;

    private SourceList<ConfiguredPolicy> _configuredPolicyList;

    private AdmxCategory _currentCategory;

    private ObservableAsPropertyHelper<List<AdmxPolicy>> _currentPolicies;

    private AdmxPolicy _currentPolicy;

    PolicyDefinitionResources _currentResources;
    private AdmxPolicyDefinitions _policyDefinitions;

    string _policyDefinitionsContent;

    private SourceList<PolicyDefinitionResources> _resourceList;

    private string _searchTerm;

    public AppViewModel()
    {
        OpenFileCommand = ReactiveCommand.CreateFromTask<AdmxAndAdmlFiles>(OpenAsync);
        ResetSearchTermCommand = ReactiveCommand.Create(ResetSearchTerm);

        this.WhenAny(x => x.CurrentCategory.Policies, x => x.Value)
            .ToProperty(this, x => x.CurrentPolicies, out _currentPolicies);

        _resourceList = new SourceList<PolicyDefinitionResources>();
        _resourceList
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _resources)
            .Subscribe();

        this.WhenAny((vm) => vm.CurrentCategory, (x) => x)
            .Subscribe((isNull) => CurrentPolicy = null);

        this.WhenAny(
                (vm) => vm.CurrentPolicy,
                (p) =>
                {
                    AdmxPolicy policy = p.Value;

                    if (policy == null)
                    {
                        return null;
                    }
                    else
                    {
                        var group = _configuredPolicyList.Items.FirstOrDefault(
                            (cp) => cp.PolicyDefinitionName == policy.Name
                        );
                        if (group == null)
                        {
                            _configuredPolicyList.Add(
                                group = new ConfiguredPolicy(policy.Name, policy.Class)
                            );
                        }

                        var res = new PolicyConfigurationModelView(
                            policy,
                            policy.Presentation.GetLocalizedPresentation(CurrentResources),
                            group
                        );

                        res.CurrentResources = CurrentResources;

                        return res;
                    }
                }
            )
            .ToProperty(
                this,
                (vm) => vm.CurrentPolicyConfiguration,
                out _currentPolicyConfiguration
            );

        this.WhenAny((vm) => vm.CurrentResources, (x) => x.Value)
            .Subscribe(
                (res) =>
                {
                    if (CurrentPolicyConfiguration != null)
                    {
                        CurrentPolicyConfiguration.CurrentResources = res;
                    }
                }
            );

        _configuredPolicyList = new SourceList<ConfiguredPolicy>();
        _configuredPolicyList.Connect().AutoRefresh().Bind(out _configuredPolicies).Subscribe();

        _searchResults = this.WhenAnyValue(x => x.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(800))
            .Select(term => term?.Trim())
            .DistinctUntilChanged()
            .Where(term => !string.IsNullOrWhiteSpace(term))
            .Select(SearchPolicies)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, (x) => x.SearchResults, out _searchResults);

        _isSearchActive = this.WhenAnyValue(x => x.SearchTerm)
            .Select((term) => !string.IsNullOrWhiteSpace(term))
            .ToProperty(this, x => x.IsSearchActive);

        _configuredPolicyList
            .Connect()
            .Select((x) => true)
            .ToProperty(
                this,
                (vm) => vm.AreConfiguredPoliciesChanged,
                out _areConfiguredPoliciesChanged
            );

        var isConfigurationReady = this.WhenAnyValue(
                (vm) => vm.IsSearchActive,
                (vm) => vm.PolicyDefinitions
            )
            .Select((x) => !x.Item1 && x.Item2 != null);

        isConfigurationReady.ToProperty(
            this,
            (vm) => vm.IsConfigurationReady,
            out _isConfigurationReady
        );

        LoadConfigurationCommand = ReactiveCommand.CreateFromTask(
            LoadConfigurationAsync,
            isConfigurationReady
        );
        SaveConfigurationCommand = ReactiveCommand.CreateFromTask(
            SaveConfigurationAsync,
            isConfigurationReady
        );
        ExportConfigurationCommand = ReactiveCommand.CreateFromTask(
            ExportConfigurationAsync,
            isConfigurationReady
        );
    }

    public AdmxPolicyDefinitions PolicyDefinitions
    {
        get => _policyDefinitions;
        private set => this.RaiseAndSetIfChanged(ref _policyDefinitions, value);
    }

    public string PolicyDefinitionsContent
    {
        get { return _policyDefinitionsContent; }
        set { this.RaiseAndSetIfChanged(ref _policyDefinitionsContent, value); }
    }

    public AdmxCategory CurrentCategory
    {
        get => _currentCategory;
        private set => this.RaiseAndSetIfChanged(ref _currentCategory, value);
    }

    public AdmxPolicy CurrentPolicy
    {
        get => _currentPolicy;
        private set => this.RaiseAndSetIfChanged(ref _currentPolicy, value);
    }

    public PolicyDefinitionResources CurrentResources
    {
        get { return _currentResources; }
        set { this.RaiseAndSetIfChanged(ref _currentResources, value); }
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    public PolicyConfigurationModelView CurrentPolicyConfiguration =>
        _currentPolicyConfiguration.Value;
    public List<AdmxPolicy> CurrentPolicies => _currentPolicies.Value;
    public ReadOnlyObservableCollection<PolicyDefinitionResources> Resources => _resources;

    public ReactiveCommand<AdmxAndAdmlFiles, Unit> OpenFileCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetSearchTermCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadConfigurationCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveConfigurationCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportConfigurationCommand { get; }
    public ReadOnlyObservableCollection<ConfiguredPolicy> ConfiguredPolicies => _configuredPolicies;
    public IEnumerable<AdmxPolicy> SearchResults => _searchResults.Value;
    public bool IsSearchActive => _isSearchActive.Value;
    public bool AreConfiguredPoliciesChanged => _areConfiguredPoliciesChanged.Value;
    public bool IsConfigurationReady => _isConfigurationReady.Value;

    private async Task OpenAsync(AdmxAndAdmlFiles files)
    {
        IOService service = Locator.Current.GetService<IOService>();

        if (
            await service
                .FileExistsAsync(files.AdmlFilePaths.Concat(new string[] { files.AdmxFilePath }))
                .ConfigureAwait(false)
        )
        {
            List<PolicyDefinitionResources> resources = new List<PolicyDefinitionResources>();

            List<(string, DeserializationLog)> logs = new List<(string, DeserializationLog)>();

            foreach (string admlFilePath in files.AdmlFilePaths)
            {
                (PolicyDefinitionResources admlRes, DeserializationLog admlLog) =
                    await PolicyDefinitionResources
                        .DeserializeAsync(
                            await service.OpenFileReadAsync(admlFilePath).ConfigureAwait(false)
                        )
                        .ConfigureAwait(false);

                string dirName = Path.GetFileName(Path.GetDirectoryName(admlFilePath));
                var culture = dirName != null ? CultureInfo.GetCultureInfo(dirName) : null;
                admlRes.Culture = culture;
                admlRes.CultureEnglishName =
                    culture?.EnglishName ?? $"Unknown {(resources.Count + 1)}";

                resources.Add(admlRes);
                logs.Add((admlFilePath, admlLog));
            }

            (PolicyDefinitions admxDef, DeserializationLog admxLog) =
                await Admx.Schema.PolicyDefinitions
                    .DeserializeAsync(
                        await service.OpenFileReadAsync(files.AdmxFilePath).ConfigureAwait(false)
                    )
                    .ConfigureAwait(false);

            ValidateAdmx(admxDef, admxLog);

            logs.Add((files.AdmxFilePath, admxLog));

            _resourceList.Clear();
            _resourceList.AddRange(resources);
            CurrentResources = resources.FirstOrDefault();
            PolicyDefinitions = AdmxPolicyDefinitions.From(admxDef);

            _configuredPolicyList.Clear();

            using (
                var admxFileStream = await service
                    .OpenFileReadAsync(files.AdmxFilePath)
                    .ConfigureAwait(false)
            )
            using (var reader = new StreamReader(admxFileStream))
            {
                PolicyDefinitionsContent = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }

    private void ValidateAdmx(PolicyDefinitions policyDefinitions, DeserializationLog log)
    {
        foreach (
            PolicyDefinition policy in policyDefinitions.Policies.Where((p) => p.Elements != null)
        )
        {
            foreach (BaseElement element in policy.Elements)
            {
                if (element is BooleanElement booleanElement)
                {
                    if (booleanElement.trueValue == null && booleanElement.falseValue == null)
                    {
                        log.LogWarning(
                            0,
                            0,
                            $"Found boolean '{element.id}' which neither has a trueValue nor a trueList defined. Using fallback values."
                        );
                        booleanElement.trueValue = new ValueContainer()
                        {
                            Item = new ValueDecimal() { RawValueInAttribute = "1" }
                        };
                    }

                    if (booleanElement.falseValue == null && booleanElement.falseList == null)
                    {
                        log.LogWarning(
                            0,
                            0,
                            $"Found boolean '{element.id}' which neither has a falseValue nor a falseList defined. Using fallback values."
                        );
                        booleanElement.falseValue = new ValueContainer()
                        {
                            Item = new ValueDecimal() { RawValueInAttribute = "0" }
                        };
                    }
                }
            }
        }
    }

    private void ResetSearchTerm()
    {
        SearchTerm = null;
    }

    private IEnumerable<AdmxPolicy> SearchPolicies(string term)
    {
        var culture = CurrentResources.Culture ?? CultureInfo.InvariantCulture;

        foreach (AdmxCategory category in PolicyDefinitions.Categories.FlattenCategories())
        {
            foreach (AdmxPolicy admxPolicy in category.Policies)
            {
                if (
                    culture.CompareInfo.IndexOf(
                        admxPolicy.DisplayName?.LocalizeWith(CurrentResources) ?? "",
                        SearchTerm,
                        CompareOptions.IgnoreCase
                    ) >= 0
                    || culture.CompareInfo.IndexOf(
                        admxPolicy.Key,
                        SearchTerm,
                        CompareOptions.IgnoreCase
                    ) >= 0
                    || culture.CompareInfo.IndexOf(
                        admxPolicy.ExplainText?.LocalizeWith(CurrentResources) ?? "",
                        SearchTerm,
                        CompareOptions.IgnoreCase
                    ) >= 0
                    || culture.CompareInfo.IndexOf(
                        admxPolicy.Name,
                        SearchTerm,
                        CompareOptions.IgnoreCase
                    ) >= 0
                )
                {
                    yield return admxPolicy;
                }
            }
        }
    }

    private async Task LoadConfigurationAsync()
    {
        IOService service = Locator.Current.GetService<IOService>();

        var selectedFilePath = await service
            .SelectSingleInputFileAsync(
                new DialogFileFilter("Policy configuration (*.policy.json)", "*.policy.json")
            )
            .ConfigureAwait(false);

        if (selectedFilePath != null)
        {
            using (
                Stream file = await service
                    .OpenFileReadAsync(selectedFilePath)
                    .ConfigureAwait(false)
            )
            using (StreamReader reader = new StreamReader(file))
            {
                var policies = ConfiguredPolicy.Deserialize(
                    await reader.ReadToEndAsync().ConfigureAwait(false)
                );

                _configuredPolicyList.Edit(
                    (innerList) =>
                    {
                        innerList.Clear();
                        innerList.AddRange(policies);
                    }
                );
            }
        }
    }

    private async Task SaveConfigurationAsync()
    {
        IOService service = Locator.Current.GetService<IOService>();

        var selectedFilePath = await service
            .SelectSingleOutputFileAsync(
                new DialogFileFilter("Policy configuration (*.policy.json)", "*.policy.json")
            )
            .ConfigureAwait(false);

        if (selectedFilePath != null)
        {
            using (
                Stream file = await service
                    .CreateOrOverwriteFileWriteAsync(selectedFilePath)
                    .ConfigureAwait(false)
            )
            using (StreamWriter writer = new StreamWriter(file))
            {
                var strPolicyList = ConfiguredPolicy.Serialize(ConfiguredPolicies);

                await writer.WriteAsync(strPolicyList).ConfigureAwait(false);

                writer.Close();
            }
        }
    }

    private async Task ExportConfigurationAsync()
    {
        IOService service = Locator.Current.GetService<IOService>();

        var selectedFilePath = await service
            .SelectSingleOutputFileAsync(new DialogFileFilter("Powershell script (*.ps1)", "*.ps1"))
            .ConfigureAwait(false);

        if (selectedFilePath != null)
        {
            using (
                Stream file = await service
                    .CreateOrOverwriteFileWriteAsync(selectedFilePath)
                    .ConfigureAwait(false)
            )
            using (StreamWriter writer = new StreamWriter(file))
            {
                foreach (
                    ConfiguredPolicy policy in ConfiguredPolicies
                        .Where((cp) => cp.IsEnabled.HasValue)
                        .OrderBy((cp) => cp.PolicyDefinitionName)
                )
                {
                    await writer
                        .WriteLineAsync("# Policy : " + policy.PolicyDefinitionName)
                        .ConfigureAwait(false);
                    await writer
                        .WriteLineAsync("# Enabled: " + policy.IsEnabled)
                        .ConfigureAwait(false);
                    await writer
                        .WriteLineAsync("# =========================================")
                        .ConfigureAwait(false);

                    string[] classNames;
                    switch (policy.PolicyClass)
                    {
                        case PolicyClass.User:
                            classNames = new string[] { "HKCU" };
                            break;
                        case PolicyClass.Machine:
                            classNames = new string[] { "HKLM" };
                            break;
                        case PolicyClass.Both:
                            classNames = new string[] { "HKCU", "HKLM" };
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (string className in classNames)
                    {
                        await writer
                            .WriteLineAsync("# Class: " + policy.PolicyClass)
                            .ConfigureAwait(false);
                        await writer
                            .WriteLineAsync("# -----------------------------------------")
                            .ConfigureAwait(false);

                        foreach (
                            ConfiguredPolicyOption option in policy.Values.OrderBy(
                                (v) => v.ElementId
                            )
                        )
                        {
                            await writer
                                .WriteLineAsync("# Option: " + option.ElementId)
                                .ConfigureAwait(false);
                            await writer
                                .WriteLineAsync(
                                    $"New-Item -Path '{className}:\\{option.ElementValue.Path}' -Force | Out-Null"
                                )
                                .ConfigureAwait(false);
                            if (option.ElementValue.KeyValueList != null)
                            {
                                foreach (
                                    KeyValuePair<string, string> pair in option
                                        .ElementValue
                                        .KeyValueList
                                )
                                {
                                    await writer
                                        .WriteLineAsync(
                                            $"Set-ItemProperty -Path '{className}:\\{option.ElementValue.Path}' -Name '{pair.Key}' -Value '{pair.Value}' -PropertyType String -Force | Out-Null"
                                        )
                                        .ConfigureAwait(false);
                                }
                            }
                            else if (option.ElementValue.KeyValueString != null)
                            {
                                await writer
                                    .WriteLineAsync(
                                        $"Set-ItemProperty -Path '{className}:\\{option.ElementValue.Path}' -Name '{option.ElementValue.KeyName}' -Value '{option.ElementValue.KeyValueString}' -PropertyType String -Force | Out-Null"
                                    )
                                    .ConfigureAwait(false);
                            }
                            else if (option.ElementValue.KeyValueUSignedInteger.HasValue)
                            {
                                await writer
                                    .WriteLineAsync(
                                        $"Set-ItemProperty -Path '{className}:\\{option.ElementValue.Path}' -Name '{option.ElementValue.KeyName}' -Value {option.ElementValue.KeyValueUSignedInteger} -PropertyType DWord -Force | Out-Null"
                                    )
                                    .ConfigureAwait(false);
                            }

                            await writer.WriteLineAsync().ConfigureAwait(false);
                        }
                    }

                    await writer.WriteLineAsync().ConfigureAwait(false);
                    await writer.WriteLineAsync().ConfigureAwait(false);
                }

                writer.Close();
            }
        }
    }
}
