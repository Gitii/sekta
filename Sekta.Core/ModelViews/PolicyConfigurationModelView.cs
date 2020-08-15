using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.ModelView.Presentation;
using Sekta.Core.Schema;

namespace Sekta.Core.ModelView
{
    public class PolicyConfigurationModelView: ReactiveObject
    {
        private readonly AdmxPolicy _policy;
        private readonly PolicyPresentation _presentation;
        private readonly BasePresentationModeView[] _presentationElements;
        private ConfiguredPolicy _configuredPolicy;

        public ReactiveCommand<Unit, Unit> ApplyChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> RevertChangesCommand { get; }


        public PolicyConfigurationModelView(AdmxPolicy policy, PolicyPresentation presentation,
            ConfiguredPolicy configuredPolicy)
        {
            _policy = policy;
            _presentation = presentation;

            _presentationElements = (Presentation?.Items ?? Array.Empty<BaseDataElement>())
                .Select<BaseDataElement, BasePresentationModeView>((i) =>
                {
                    var enumElem = _policy.Elements.FirstOrDefault((e) => e.id == i.RefId);

                    if (i is CheckBox cb)
                    {
                        return new CheckboxPresentationModelView(cb, policy);
                    } 
                    else if (i is DropdownList dd)
                    {
                        return new DropdownPresentationModelView(dd, policy);
                    }
                    else if (i is DecimalTextBox dtb)
                    {
                        return new DecimalTextboxPresentationModelView(dtb, policy);
                    }
                    else if (i is TextBox tb)
                    {
                        return new TextboxPresentationModelView(tb, (TextElement) enumElem, policy);
                    }
                    else if (i is StringElement se)
                    {
                        return new TextPresentationModelView(se, policy);
                    }
                    else if (i is ListBox lb)
                    {
                        return new ListboxPresentationModelView(lb, policy);
                    }
                    else
                    {
                        return null;
                    }
                })
                .Where((i) => i != null)
                .ToArray();

            foreach (BasePresentationModeView element in _presentationElements)
            {
                element.Changed.Subscribe((e) =>
                {
                    if (e.PropertyName != nameof(BasePresentationModeView.CurrentResources) && element.HasRelevantDataChanged(e.PropertyName))
                    {
                        IsDataChanged = true;
                    }
                });
            }

            IObservable<bool> isValid = _presentationElements
                .Where((pe) => pe.ValidationContext != null)
                .Select((pe) => pe.ValidationContext.Valid)
                .Concat(new []{Observable.Return(true)})
                .CombineLatest()
                .CombineLatest(this.WhenAnyValue((vm) => vm.IsEnabled), (list, isEnabled) => (list, isEnabled))
                .Select(((tuple) =>
                {
                    var (validationToken, isEnabled) = tuple;
                    if (validationToken.Count == 0 || isEnabled.GetValueOrDefault(false) == false || validationToken.All((b) => b))
                    {
                        // assume everything is valid if
                        // 1. policy is disabled or not configured
                        // 2. policy does not contain any elements
                        return true;
                    }

                    return false;
                }));

            isValid.ToProperty(this, (vm) => vm.AreElementsAllValid, out _areElementsAllValid);

            isValid.Select(((isValid) =>
            {
                if (isValid)
                {
                    return null;
                }
                else
                {
                    return _presentationElements.FirstOrDefault(pe => pe.ValidationContext != null)?.ValidationContext
                        .Text.ToSingleLine();
                }
            })).ToProperty(this, (vm) => vm.ValidationText, out _validationText);

            ApplyChangesCommand = ReactiveCommand.CreateFromTask(ApplyChanges, this.WhenAnyValue((vm) => vm.AreElementsAllValid));
            RevertChangesCommand = ReactiveCommand.CreateFromTask(RevertChanges);
            _configuredPolicy = configuredPolicy;

            this.WhenAny((vm) => vm.CurrentResources, (x) => x.Value).Subscribe((cr) =>
            {
                foreach (BasePresentationModeView element in _presentationElements)
                {
                    element.CurrentResources = cr;
                }
            });
        }

        readonly ObservableAsPropertyHelper<string> _validationText;
        public string ValidationText => _validationText.Value;

        readonly ObservableAsPropertyHelper<bool> _areElementsAllValid;
        public bool AreElementsAllValid => _areElementsAllValid.Value;

        private bool? _isEnabled;
        public bool? IsEnabled
        {
            get { return _isEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        }

        private bool _isDataChanged;
        public bool IsDataChanged
        {
            get { return _isDataChanged; }
            set { this.RaiseAndSetIfChanged(ref _isDataChanged, value); }
        }

        private PolicyDefinitionResources _currentResources;
        public PolicyDefinitionResources CurrentResources
        {
            get { return _currentResources; }
            set { this.RaiseAndSetIfChanged(ref _currentResources, value); }
        }

        public PolicyPresentation Presentation => _presentation;
        public BasePresentationModeView[] PresentationElements => _presentationElements;

        private async Task ApplyChanges()
        {
            _configuredPolicy.Values.Clear();

            if (IsEnabled.HasValue)
            {
                foreach (BasePresentationModeView element in _presentationElements.Where((pe) => !pe.IsStatic))
                {
                    if (!IsEnabled.GetValueOrDefault())
                    {
                        // DISABLED
                        // apply but use default values
                        element.RevertToDefaultValues();
                    }

                    var value = element.Serialize(_policy.Elements);
                    
                    _configuredPolicy.Values.Add(new ConfiguredPolicyOption(element.Id, value));
                }

                if (IsEnabled.GetValueOrDefault())
                {
                    AddEnabledValues();
                }
                else
                {
                    AddDisabledValues();
                }
            }
            
            IsDataChanged = false;
            _configuredPolicy.IsEnabled = IsEnabled;
        }

        private void AddDisabledValues()
        {
            if (_policy.DisabledValue != null)
            {
                _configuredPolicy.Values.Add(new ConfiguredPolicyOption(_policy.Key + "_disabledValue", _policy.DisabledValue.AsPolicyOption(_policy.Key, _policy.ValueName, AdmxPolicy.POLICY_EMBEDDED_OPTION_ID)));
            }
            
            if (_policy.DisabledList != null)
            {
                foreach (ValueItem item in _policy.DisabledList.Items)
                {
                    _configuredPolicy.Values.Add(new ConfiguredPolicyOption(_policy.Key + "_disabledList_" + item.Key, item.Value.AsPolicyOption(item.Key, item.ValueName, AdmxPolicy.POLICY_EMBEDDED_OPTION_ID)));
                }
            }
        }

        private void AddEnabledValues()
        {
            if (_policy.EnabledValue != null)
            {
                _configuredPolicy.Values.Add(new ConfiguredPolicyOption(_policy.Key + "_enabledValue", _policy.EnabledValue.AsPolicyOption(_policy.Key, _policy.ValueName, AdmxPolicy.POLICY_EMBEDDED_OPTION_ID)));
            }
            
            if (_policy.EnabledList != null)
            {
                foreach (ValueItem item in _policy.EnabledList.Items)
                {
                    _configuredPolicy.Values.Add(new ConfiguredPolicyOption(_policy.Key + "_enabledList_" + item.Key, item.Value.AsPolicyOption(item.Key, item.ValueName, AdmxPolicy.POLICY_EMBEDDED_OPTION_ID)));
                }
            }
        }

        private Task RevertChanges()
        {
            IsDataChanged = false;
            IsEnabled = _configuredPolicy.IsEnabled;

            foreach (BasePresentationModeView element in _presentationElements.Where((pe) => !pe.IsStatic))
            {
                ConfiguredPolicyOption value = _configuredPolicy.Values.FirstOrDefault((sv) => sv.ElementId == element.Id);

                element.Deserialize(_policy.Elements, value?.ElementValue);
            }

            return Task.CompletedTask;
        }
    }
}
