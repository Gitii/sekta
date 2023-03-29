using System;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using Sekta.Admx.Schema;

namespace Sekta.Core.ModelView.Presentation;

public abstract class BasePresentationModeView : ReactiveObject, IValidatableViewModel
{
    public abstract PolicyOptionValue Serialize(BaseElement[] elements);
    public abstract void Deserialize(BaseElement[] elements, PolicyOptionValue? serializedValue);

    public event Action<BasePresentationModeView> DataChanged;

    public abstract string Id { get; }

    private PolicyDefinitionResources _currentResources;
    public PolicyDefinitionResources CurrentResources
    {
        get { return _currentResources; }
        set { this.RaiseAndSetIfChanged(ref _currentResources, value); }
    }

    protected virtual void OnDataChanged()
    {
        DataChanged?.Invoke(this);
    }

    public abstract void RevertToDefaultValues();

    public virtual bool IsStatic { get; } = false;

    public ValidationContext ValidationContext { get; protected set; }

    public virtual bool HasRelevantDataChanged(string propertyName)
    {
        return true;
    }
}
