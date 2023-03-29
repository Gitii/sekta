using System;

namespace Sekta.Core.ModelView.Presentation;

public class ConfiguredPolicyOption
{
    public ConfiguredPolicyOption(string elementId, PolicyOptionValue elementValue)
    {
        ElementId = elementId ?? throw new ArgumentNullException(nameof(elementId));
        ElementValue = elementValue;
    }

    public string ElementId { get; }

    public PolicyOptionValue ElementValue { get; }
}
