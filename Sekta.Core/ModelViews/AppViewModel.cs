using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DynamicData;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.IO;
using Sekta.Core.ModelView.Presentation;
using Sekta.Core.Schema;
using Splat;

namespace Sekta.Core.ModelView
{
    public readonly struct AdmxAndAdmlFiles
    {
        public readonly string AdmxFilePath;
        public readonly string[] AdmlFilePaths;

        public AdmxAndAdmlFiles(string admxFilePath, IEnumerable<string> admlFilePaths)
        {
            AdmxFilePath = admxFilePath;
            AdmlFilePaths = admlFilePaths.ToArray();
        }
    }

    public class AppViewModel: ReactiveObject
    {
        private AdmxPolicyDefinitions _policyDefinitions;
        public AdmxPolicyDefinitions PolicyDefinitions
        {
            get => _policyDefinitions;
            private set => this.RaiseAndSetIfChanged(ref _policyDefinitions, value);
        }

        string _policyDefinitionsContent;
        public string PolicyDefinitionsContent
        {
            get { return _policyDefinitionsContent; }
            set { this.RaiseAndSetIfChanged(ref _policyDefinitionsContent, value); }
        }

        private AdmxCategory _currentCategory;
        public AdmxCategory CurrentCategory
        {
            get => _currentCategory;
            private set => this.RaiseAndSetIfChanged(ref _currentCategory, value);
        }

        private AdmxPolicy _currentPolicy;
        public AdmxPolicy CurrentPolicy
        {
            get => _currentPolicy;
            private set => this.RaiseAndSetIfChanged(ref _currentPolicy, value);
        }

        PolicyDefinitionResources _currentResources;
        public PolicyDefinitionResources CurrentResources
        {
            get { return _currentResources; }
            set { this.RaiseAndSetIfChanged(ref _currentResources, value); }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
        }

        private readonly ObservableAsPropertyHelper<PolicyConfigurationModelView> _currentPolicyConfiguration;
        public PolicyConfigurationModelView CurrentPolicyConfiguration => _currentPolicyConfiguration.Value;

        private ObservableAsPropertyHelper<List<AdmxPolicy>> _currentPolicies;
        public List<AdmxPolicy> CurrentPolicies => _currentPolicies.Value;

        private SourceList<PolicyDefinitionResources> _resourceList;
        private readonly ReadOnlyObservableCollection<PolicyDefinitionResources> _resources;
        public ReadOnlyObservableCollection<PolicyDefinitionResources> Resources => _resources;

        public ReactiveCommand<AdmxAndAdmlFiles, Unit> OpenFileCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetSearchTermCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadConfigurationCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveConfigurationCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportConfigurationCommand { get; }

        private SourceList<ConfiguredPolicy> _configuredPolicyList;

        private ReadOnlyObservableCollection<ConfiguredPolicy> _configuredPolicies;
        public ReadOnlyObservableCollection<ConfiguredPolicy> ConfiguredPolicies => _configuredPolicies;

        private readonly ObservableAsPropertyHelper<IEnumerable<AdmxPolicy>> _searchResults;
        public IEnumerable<AdmxPolicy> SearchResults => _searchResults.Value;

        private readonly ObservableAsPropertyHelper<bool> _isSearchActive;
        public bool IsSearchActive => _isSearchActive.Value;

        readonly ObservableAsPropertyHelper<bool> _areConfiguredPoliciesChanged;
        public bool AreConfiguredPoliciesChanged => _areConfiguredPoliciesChanged.Value;

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
                            var group = _configuredPolicyList.Items.FirstOrDefault((cp) => cp.PolicyDefinitionName == policy.Name);
                            if (group == null)
                            {
                                _configuredPolicyList.Add(group = new ConfiguredPolicy(policy.Name, policy.Class));
                            }

                            var res = new PolicyConfigurationModelView(policy,
                                policy.Presentation.GetLocalizedPresentation(CurrentResources), group);

                            res.CurrentResources = CurrentResources;

                            return res;
                        }
                    })
                .ToProperty(this, (vm) => vm.CurrentPolicyConfiguration, out _currentPolicyConfiguration);

            this.WhenAny((vm) => vm.CurrentResources, (x) => x.Value)
                .Subscribe((res) =>
                {
                    if (CurrentPolicyConfiguration != null)
                    {
                        CurrentPolicyConfiguration.CurrentResources = res;
                    }  
                });

            _configuredPolicyList = new SourceList<ConfiguredPolicy>();
            _configuredPolicyList
                .Connect()
                .AutoRefresh()
                .Bind(out _configuredPolicies)
                .Subscribe();

            _searchResults = this
                .WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Select(term => term?.Trim())
                .DistinctUntilChanged()
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .Select(SearchPolicies)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, (x) => x.SearchResults, out _searchResults);

            _isSearchActive = this
                .WhenAnyValue(x => x.SearchTerm)
                .Select((term) => !string.IsNullOrWhiteSpace(term))
                .ToProperty(this, x => x.IsSearchActive);

            _configuredPolicyList
                .Connect()
                .Select((x) => true)
                .ToProperty(this, (vm) => vm.AreConfiguredPoliciesChanged, out _areConfiguredPoliciesChanged);

            var isConfigurationReady = this
                .WhenAnyValue((vm) => vm.IsSearchActive, (vm) => vm.PolicyDefinitions)
                .Select((x) => !x.Item1 && x.Item2 != null);

            LoadConfigurationCommand = ReactiveCommand.CreateFromTask(LoadConfiguration, isConfigurationReady);
            SaveConfigurationCommand = ReactiveCommand.CreateFromTask(SaveConfiguration, isConfigurationReady);
            ExportConfigurationCommand = ReactiveCommand.CreateFromTask(ExportConfiguration, isConfigurationReady);
        }

        private async Task OpenAsync(AdmxAndAdmlFiles files)
        {
            IOService service = Locator.Current.GetService<IOService>();

            if (await service.FileExists(files.AdmlFilePaths.Concat(new string[] { files.AdmxFilePath })))
            {
                List<PolicyDefinitionResources> resources = new List<PolicyDefinitionResources>();

                List<(string, DeserializationLog)> logs = new List<(string, DeserializationLog)>();

                foreach (string admlFilePath in files.AdmlFilePaths)
                {
                    (PolicyDefinitionResources admlRes, DeserializationLog admlLog) = await PolicyDefinitionResources.DeserializeAsync(await service.OpenFileRead(admlFilePath));

                    string dirName = Path.GetFileName(Path.GetDirectoryName(admlFilePath));
                    var culture = dirName != null ? CultureInfo.GetCultureInfo(dirName) : null;
                    admlRes.Culture = culture;
                    admlRes.CultureEnglishName = culture?.EnglishName ?? $"Unknown {(resources.Count + 1)}";

                    resources.Add(admlRes);
                    logs.Add((admlFilePath, admlLog));
                }

                (PolicyDefinitions admxDef, DeserializationLog admxLog) = await Admx.Schema.PolicyDefinitions.DeserializeAsync(await service.OpenFileRead(files.AdmxFilePath));

                ValidateAdmx(admxDef, admxLog);
                
                logs.Add((files.AdmxFilePath, admxLog));

                _resourceList.Clear();
                _resourceList.AddRange(resources);
                CurrentResources = resources.FirstOrDefault();
                PolicyDefinitions = AdmxPolicyDefinitions.From(admxDef);

                _configuredPolicyList.Clear();

                using (var admxFileStream = await service.OpenFileRead(files.AdmxFilePath))
                using (var reader = new StreamReader(admxFileStream))
                {
                    PolicyDefinitionsContent = reader.ReadToEnd();
                }
            }
        }

        private void ValidateAdmx(PolicyDefinitions policyDefinitions, DeserializationLog log)
        {
            foreach (PolicyDefinition policy in policyDefinitions.Policies.Where((p) => p.Elements != null))
            {
                foreach (BaseElement element in policy.Elements)
                {
                    if (element is BooleanElement booleanElement)
                    {
                        if (booleanElement.trueValue == null && booleanElement.falseValue == null)
                        {
                            log.LogWarning(0, 0,$"Found boolean '{element.id}' which neither has a trueValue nor a trueList defined. Using fallback values.");
                            booleanElement.trueValue = new ValueContainer() { Item = new ValueDecimal() { RawValueInAttribute = "1" }};
                        }

                        if (booleanElement.falseValue == null && booleanElement.falseList == null)
                        {
                            log.LogWarning(0, 0,$"Found boolean '{element.id}' which neither has a falseValue nor a falseList defined. Using fallback values.");
                            booleanElement.falseValue = new ValueContainer() { Item = new ValueDecimal() { RawValueInAttribute = "0" }};
                        }
                    }
                }
            }
        }

        private void ResetSearchTerm()
        {
            SearchTerm = null;
        }

        private IEnumerable<AdmxPolicy> SearchPolicies(
            string term)
        {
            var culture = CurrentResources.Culture ?? CultureInfo.InvariantCulture;

            foreach (AdmxCategory category in PolicyDefinitions.Categories.FlattenCategories())
            {
                foreach (AdmxPolicy admxPolicy in category.Policies)
                {
                    if (culture.CompareInfo.IndexOf(admxPolicy.DisplayName?.LocalizeWith(CurrentResources) ?? "", SearchTerm, CompareOptions.IgnoreCase) >= 0
                        || culture.CompareInfo.IndexOf(admxPolicy.Key, SearchTerm, CompareOptions.IgnoreCase) >= 0
                        || culture.CompareInfo.IndexOf(admxPolicy.ExplainText?.LocalizeWith(CurrentResources) ?? "", SearchTerm, CompareOptions.IgnoreCase) >= 0
                        || culture.CompareInfo.IndexOf(admxPolicy.Name, SearchTerm, CompareOptions.IgnoreCase) >= 0)
                    {
                        yield return admxPolicy;
                    }
                }
            }
        }

        private async Task LoadConfiguration()
        {
            IOService service = Locator.Current.GetService<IOService>();

            var selectedFilePath = await service.SelectSingleInputFile(new DialogFileFilter("Policy configuration (*.policy.json)",
                "*.policy.json"));

            if (selectedFilePath != null)
            {
                using (Stream file = await service.OpenFileRead(selectedFilePath))
                using (StreamReader reader = new StreamReader(file))
                {
                    var policies = ConfiguredPolicy.Deserialize(reader.ReadToEnd());

                    _configuredPolicyList.Edit((innerList) =>
                    {
                        innerList.Clear();
                        innerList.AddRange(policies);
                    });
                }
            }
        }

        private async Task SaveConfiguration()
        {
            IOService service = Locator.Current.GetService<IOService>();

            var selectedFilePath = await service.SelectSingleOutputFile(new DialogFileFilter("Policy configuration (*.policy.json)",
                "*.policy.json"));

            if (selectedFilePath != null)
            {
                using (Stream file = await service.CreateOrOverwriteFileWrite(selectedFilePath))
                using (StreamWriter writer = new StreamWriter(file))
                {
                    var strPolicyList = ConfiguredPolicy.Serialize(ConfiguredPolicies);

                    writer.Write(strPolicyList);

                    writer.Close();
                }
            }
        }

        private async Task ExportConfiguration()
        {
            IOService service = Locator.Current.GetService<IOService>();

            var selectedFilePath = await service.SelectSingleOutputFile(new DialogFileFilter("Powershell script (*.ps1)",
                "*.ps1"));

            if (selectedFilePath != null)
            {
                using (Stream file = await service.CreateOrOverwriteFileWrite(selectedFilePath))
                using (StreamWriter writer = new StreamWriter(file))
                {
                    foreach (ConfiguredPolicy policy in ConfiguredPolicies.Where((cp) => cp.IsEnabled.HasValue).OrderBy((cp) => cp.PolicyDefinitionName))
                    {
                        await writer.WriteLineAsync("# Policy : " + policy.PolicyDefinitionName);
                        await writer.WriteLineAsync("# Enabled: " + policy.IsEnabled);
                        await writer.WriteLineAsync("# =========================================");

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
                            await writer.WriteLineAsync("# Class: " + policy.PolicyClass);
                            await writer.WriteLineAsync("# -----------------------------------------");

                            foreach (ConfiguredPolicyOption option in policy.Values.OrderBy((v) => v.ElementId))
                            {
                                await writer.WriteLineAsync("# Option: " + option.ElementId);
                                await writer.WriteLineAsync($"New-Item -Path '{className}:\\{option.ElementValue.Path}' -Force | Out-Null");
                                if (option.ElementValue.KeyValueList != null)
                                {
                                    foreach (KeyValuePair<string, string> pair in option.ElementValue.KeyValueList)
                                    {
                                        await writer.WriteLineAsync($"Set-ItemProperty -Path '{className}:\\{option.ElementValue.Path}' -Name '{pair.Key}' -Value '{pair.Value}' -PropertyType String -Force | Out-Null");
                                    }
                                } 
                                else if (option.ElementValue.KeyValueString != null)
                                {
                                    await writer.WriteLineAsync($"Set-ItemProperty -Path '{className}:\\{option.ElementValue.Path}' -Name '{option.ElementValue.KeyName}' -Value '{option.ElementValue.KeyValueString}' -PropertyType String -Force | Out-Null");
                                }
                                else if (option.ElementValue.KeyValueUSignedInteger.HasValue)
                                {
                                    await writer.WriteLineAsync($"Set-ItemProperty -Path '{className}:\\{option.ElementValue.Path}' -Name '{option.ElementValue.KeyName}' -Value {option.ElementValue.KeyValueUSignedInteger} -PropertyType DWord -Force | Out-Null");
                                }

                                await writer.WriteLineAsync();
                            }
                        }

                        await writer.WriteLineAsync();
                        await writer.WriteLineAsync();
                    }

                    writer.Close();
                }
            }
        }
    }
}
