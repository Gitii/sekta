using System;
using ReactiveUI;
using Sekta.Admx.Schema;
using Sekta.Core.Schema;

namespace Sekta.Core.ModelView.Presentation;

public class TextPresentationModelView : BasePresentationModeView
{
    private readonly AdmxPolicy _admxPolicy;
    private StringElement _presentationElement;

    readonly ObservableAsPropertyHelper<string> _label;

    public TextPresentationModelView(StringElement presentationElement, AdmxPolicy admxPolicy)
    {
        _admxPolicy = admxPolicy;
        _presentationElement = presentationElement;

        this.WhenAny((vm) => vm.PresentationElement, (x) => x.Value?.Value)
            .ToProperty(this, (vm) => vm.Label, out _label);
    }

    public string Label => _label.Value;

    public StringElement PresentationElement
    {
        get => _presentationElement;
        set => this.RaiseAndSetIfChanged(ref _presentationElement, value);
    }

    public override PolicyOptionValue Serialize(BaseElement[] elements)
    {
        throw new NotSupportedException();
    }

    public override void Deserialize(BaseElement[] elements, PolicyOptionValue? serializedValue)
    {
        throw new NotSupportedException();
    }

    public override void RevertToDefaultValues()
    {
        throw new NotSupportedException();
    }

    public override bool IsStatic => true;

    public override string Id => _presentationElement.RefId;

    public override bool HasRelevantDataChanged(string propertyName)
    {
        return false;
    }
}
