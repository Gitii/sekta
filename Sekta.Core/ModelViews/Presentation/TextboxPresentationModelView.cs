using ReactiveUI;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Sekta.Admx.Schema;
using Sekta.Core.Schema;

namespace Sekta.Core.ModelView.Presentation;

public class TextboxPresentationModelView : BasePresentationModeView
{
    private readonly AdmxPolicy _admxPolicy;
    private TextBox _presentationElement;
    private TextElement _enumerationElement;
    private string _value;

    readonly ObservableAsPropertyHelper<string> _defaultValue;
    readonly ObservableAsPropertyHelper<string> _label;

    public TextboxPresentationModelView(
        TextBox presentationElement,
        TextElement enumerationElement,
        AdmxPolicy admxPolicy
    )
    {
        _admxPolicy = admxPolicy;
        PresentationElement = presentationElement;
        EnumerationElement = enumerationElement;

        ValidationContext = new ValidationContext();

        this.WhenAny((vm) => vm.PresentationElement, (x) => x.Value?.Label)
            .ToProperty(this, (vm) => vm.Label, out _label);

        this.WhenAny((vm) => vm.PresentationElement, (x) => x.Value?.DefaultValue)
            .ToProperty(this, (vm) => vm.DefaultValue, out _defaultValue);

        RevertToDefaultValues();

        if (enumerationElement.required)
        {
            this.ValidationRule(
                (vm) => vm.Value,
                (value) => !string.IsNullOrWhiteSpace(value),
                "Must not be empty or whitespace!"
            );
        }
    }

    public string Label => _label.Value;

    public string DefaultValue => _defaultValue.Value;

    public TextBox PresentationElement
    {
        get => _presentationElement;
        set => this.RaiseAndSetIfChanged(ref _presentationElement, value);
    }

    public TextElement EnumerationElement
    {
        get { return _enumerationElement; }
        set { this.RaiseAndSetIfChanged(ref _enumerationElement, value); }
    }

    public string Value
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
        if (serializedValue == null)
        {
            RevertToDefaultValues();
        }
        else
        {
            Value = serializedValue.Value.KeyValueString;
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
