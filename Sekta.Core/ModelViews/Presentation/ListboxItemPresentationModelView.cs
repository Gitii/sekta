using ReactiveUI;

namespace Sekta.Core.ModelView.Presentation;

public class ListboxItemPresentationModelView : ReactiveObject
{
    private string _keyName;

    public string KeyName
    {
        get { return _keyName; }
        set { this.RaiseAndSetIfChanged(ref _keyName, value); }
    }

    private string _value;

    public string Value
    {
        get { return _value; }
        set { this.RaiseAndSetIfChanged(ref _value, value); }
    }
}