using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.Schema;

namespace Sekta.Core.ModelView.Presentation;

public class DecimalTextboxPresentationModelView : BasePresentationModeView
{
    private readonly AdmxPolicy _admxPolicy;
    private DecimalTextBox _presentationElement;
    private DecimalElement _enumerationElement;
    private uint _value;

    readonly ObservableAsPropertyHelper<uint> _minValue;
    readonly ObservableAsPropertyHelper<uint> _maxValue;
    readonly ObservableAsPropertyHelper<uint> _spinStep;
    readonly ObservableAsPropertyHelper<uint> _defaultValue;
    readonly ObservableAsPropertyHelper<bool> _spin;
    readonly ObservableAsPropertyHelper<string> _label;

    public DecimalTextboxPresentationModelView(
        DecimalTextBox presentationElement,
        AdmxPolicy admxPolicy
    )
    {
        _admxPolicy = admxPolicy;
        PresentationElement = presentationElement;

        this.WhenAny((vm) => vm.EnumerationElement, (x) => x.Value?.minValue ?? 0)
            .ToProperty(this, (vm) => vm.MinValue, out _minValue);

        this.WhenAny((vm) => vm.EnumerationElement, (x) => x.Value?.maxValue ?? 0)
            .ToProperty(this, (vm) => vm.MaxValue, out _maxValue);

        this.WhenAny((vm) => vm.PresentationElement, (x) => x.Value.DefaultValue)
            .ToProperty(this, (vm) => vm.DefaultValue, out _defaultValue);

        this.WhenAny((vm) => vm.PresentationElement, (x) => x.Value.SpinStep)
            .ToProperty(this, (vm) => vm.SpinStep, out _spinStep);

        this.WhenAny((vm) => vm.PresentationElement, (x) => x.Value.Spin)
            .ToProperty(this, (vm) => vm.Spin, out _spin);

        this.WhenAnyValue((vm) => vm.PresentationElement, (vm) => vm.CurrentResources)
            .Where((x) => x.Item2 != null)
            .Select((tuple) => tuple.Item1.Value.LocalizeWith(tuple.Item2))
            .ToProperty(this, (vm) => vm.Label, out _label);

        RevertToDefaultValues();
    }

    public string Label => _label.Value;

    public uint MinValue => _minValue.Value;

    public uint MaxValue => _maxValue.Value;

    public uint SpinStep => _spinStep.Value;

    public uint DefaultValue => _defaultValue.Value;

    public bool Spin => _spin.Value;

    public DecimalTextBox PresentationElement
    {
        get => _presentationElement;
        set => this.RaiseAndSetIfChanged(ref _presentationElement, value);
    }

    public DecimalElement EnumerationElement
    {
        get { return _enumerationElement; }
        set { this.RaiseAndSetIfChanged(ref _enumerationElement, value); }
    }

    public uint Value
    {
        get { return _value; }
        set { this.RaiseAndSetIfChanged(ref _value, value); }
    }

    public override PolicyOptionValue Serialize(BaseElement[] elements)
    {
        return new PolicyOptionValue(
            EnumerationElement.key ?? _admxPolicy.Key,
            EnumerationElement.valueName,
            Value,
            EnumerationElement.id
        );
    }

    public override void Deserialize(BaseElement[] elements, PolicyOptionValue? serializedValue)
    {
        EnumerationElement = (DecimalElement)
            elements.First((e) => e.id == _presentationElement.RefId);

        if (serializedValue == null)
        {
            RevertToDefaultValues();
        }
        else
        {
            Value = serializedValue.Value.ToUIntValue();
        }
    }

    public override void RevertToDefaultValues()
    {
        Value = DefaultValue;
    }

    public override string Id => _presentationElement.RefId;

    public override bool HasRelevantDataChanged(string propertyName)
    {
        return propertyName == nameof(Value);
    }
}
