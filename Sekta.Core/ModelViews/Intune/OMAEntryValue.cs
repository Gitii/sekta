using System;

namespace Sekta.Core.ModelView.Intune;

public readonly struct OMAEntryValue
{
    public string Name { get; }
    public string Description { get; }
    public string Uri { get; }
    public string Value { get; }

    public OMAEntryValue(string name, string description, string uri, string value)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}