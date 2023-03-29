using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.ModelView.Presentation;
using Sekta.Core.Schema;
#pragma warning disable MA0051

namespace Sekta.Core.ModelView.Intune;

public class OMAModelView : ReactiveObject
{
    private readonly List<ConfiguredPolicy> _configuredPolicies;
    private readonly AdmxPolicyDefinitions _definitions;

    public OMAModelView(
        List<ConfiguredPolicy> configuredPolicies,
        string admxFileContent,
        AdmxPolicyDefinitions definitions
    )
    {
        _configuredPolicies = configuredPolicies;
        _definitions = definitions;

        this.WhenAnyValue((vm) => vm.ApplicationName, (vm) => vm.AdmxFileName)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(
                (pair) =>
                {
                    var (applicationName, fileName) = pair;

                    var entries = new List<OMAEntryValue>()
                    {
                        new OMAEntryValue(
                            $"ADMX - {applicationName}",
                            "Contains the admx-file which will be ingested.",
                            $"./Vendor/MSFT/Policy/ConfigOperations/ADMXInstall/{applicationName}/Policy/{fileName}",
                            admxFileContent
                        )
                    };

                    foreach (ConfiguredPolicy configuredPolicy in _configuredPolicies)
                    {
                        var writer = new StringBuilder();

                        if (configuredPolicy.IsEnabled.HasValue)
                        {
                            if (configuredPolicy.IsEnabled.Value)
                            {
                                writer.AppendLine("<enabled/>");
                                var dataElements = configuredPolicy.Values
                                    .Where(
                                        (v) =>
                                            v.ElementValue.Id
                                            != AdmxPolicy.POLICY_EMBEDDED_OPTION_ID
                                    )
                                    .Select(
                                        (v) =>
                                        {
                                            if (v.ElementValue.KeyValueList != null)
                                            {
                                                return new XElement(
                                                    "data",
                                                    new XAttribute("id", v.ElementValue.Id),
                                                    new XAttribute(
                                                        "value",
                                                        string.Join(
                                                            char.ConvertFromUtf32(61440),
                                                            v.ElementValue.KeyValueList.SelectMany(
                                                                (kv) =>
                                                                    new string[]
                                                                    {
                                                                        kv.Key,
                                                                        kv.Value
                                                                    }
                                                            )
                                                        )
                                                    )
                                                );
                                            }
                                            else
                                            {
                                                var dataElement = new XElement(
                                                    "data",
                                                    new XAttribute("id", v.ElementValue.Id),
                                                    new XAttribute(
                                                        "value",
                                                        v.ElementValue.KeyValueString
                                                            ?? v.ElementValue.KeyValueUSignedInteger.Value.ToString()
                                                    )
                                                );

                                                return dataElement;
                                            }
                                        }
                                    );

                                foreach (XElement element in dataElements)
                                {
                                    writer.AppendLine(element.ToString(SaveOptions.None));
                                }
                            }
                            else
                            {
                                writer.AppendLine("<disabled/>");
                            }
                        }

                        string[] scopes;
                        switch (configuredPolicy.PolicyClass)
                        {
                            case PolicyClass.User:
                                scopes = new[] { "User" };
                                break;
                            case PolicyClass.Machine:
                                scopes = new[] { "Device" };
                                break;
                            case PolicyClass.Both:
                                scopes = new[] { "User", "Device" };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var originalPolicy = FindPolicy(
                            definitions,
                            configuredPolicy.PolicyDefinitionName
                        );
                        var path = GetCategoryPath(originalPolicy.Category).ToArray();

                        foreach (string scope in scopes)
                        {
                            entries.Add(
                                new OMAEntryValue(
                                    configuredPolicy.PolicyDefinitionName,
                                    "",
                                    $"./{scope}/Vendor/MSFT/Policy/Config/{applicationName}~Policy~{string.Join("~", path)}/{originalPolicy.Name}",
                                    writer.ToString()
                                )
                            );
                        }
                    }

                    return entries.ToArray();
                }
            )
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, (vm) => vm.Entries, out _entries);
    }

    private AdmxPolicy FindPolicy(AdmxPolicyDefinitions definitions, string policyName)
    {
        var parentCategory = definitions.Categories
            .FlattenCategories()
            .First((c) => c.Policies.Exists((p) => p.Name == policyName));

        return parentCategory.Policies.First((p) => p.Name == policyName);
    }

    private IEnumerable<string> GetCategoryPath(AdmxCategory category)
    {
        return category.GetCategoryPathElements().Select((c) => c.Name);
    }

    string _applicationName;

    public string ApplicationName
    {
        get { return _applicationName; }
        set { this.RaiseAndSetIfChanged(ref _applicationName, value); }
    }

    string _admxFileName;

    public string AdmxFileName
    {
        get { return _admxFileName; }
        set { this.RaiseAndSetIfChanged(ref _admxFileName, value); }
    }

    readonly ObservableAsPropertyHelper<OMAEntryValue[]> _entries;
    public OMAEntryValue[] Entries => _entries.Value;
}
