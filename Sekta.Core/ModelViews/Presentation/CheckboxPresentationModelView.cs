using System;
using System.Diagnostics;
using System.Linq;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.Schema;

namespace Sekta.Core.ModelView.Presentation;

public class CheckboxPresentationModelView : BasePresentationModeView
{
    private CheckBox _presentationElement;
    private readonly AdmxPolicy _admxPolicy;

    public CheckboxPresentationModelView(CheckBox presentationElement, AdmxPolicy admxPolicy)
    {
        _presentationElement = presentationElement;
        this._admxPolicy = admxPolicy;

        _isChecked = DefaultValue;
    }

    bool _isChecked;
    public bool IsChecked
    {
        get { return _isChecked; }
        set
        {
            this.RaiseAndSetIfChanged(ref _isChecked, value);
            OnDataChanged();
        }
    }

    public bool DefaultValue
    {
        get { return _presentationElement.DefaultValue || _presentationElement.DefaultChecked; }
    }

    public CheckBox PresentationElement
    {
        get => _presentationElement;
        set => _presentationElement = value;
    }

    public override PolicyOptionValue Serialize(BaseElement[] elements)
    {
        var id = PresentationElement.RefId;
        var element = elements.FirstOrDefault((otherElement) => otherElement.id == id);
        if (element is BooleanElement booleanElement)
        {
            if (IsChecked)
            {
                return booleanElement.trueValue.AsPolicyOption(
                    booleanElement.key ?? _admxPolicy.Key,
                    booleanElement.valueName,
                    booleanElement.id
                );
            }
            else
            {
                return booleanElement.falseValue.AsPolicyOption(
                    booleanElement.key ?? _admxPolicy.Key,
                    booleanElement.valueName,
                    booleanElement.id
                );
            }
        }
        else
        {
            throw new Exception($"Failed to find element for {id}.");
        }
    }

    public override void Deserialize(BaseElement[] elements, PolicyOptionValue? serializedValue)
    {
        if (serializedValue == null)
        {
            RevertToDefaultValues();
        }
        else
        {
            var id = PresentationElement.RefId;
            var element = elements.FirstOrDefault((otherElement) => otherElement.id == id);
            if (element is BooleanElement booleanElement)
            {
                if (serializedValue.Value.HasSameValue(booleanElement.trueValue))
                {
                    IsChecked = true;
                }
                else
                {
                    Debug.Assert(serializedValue.Value.HasSameValue(booleanElement.falseValue));
                    IsChecked = false;
                }
            }
            else
            {
                throw new Exception($"Failed to find element for {id}.");
            }
        }
    }

    public override void RevertToDefaultValues()
    {
        IsChecked = DefaultValue;
    }

    public override string Id => _presentationElement.RefId;

    public override bool HasRelevantDataChanged(string propertyName)
    {
        return propertyName == nameof(IsChecked);
    }
}
