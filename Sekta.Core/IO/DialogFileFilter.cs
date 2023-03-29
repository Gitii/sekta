namespace Sekta.Core.IO;

public readonly struct DialogFileFilter
{
    public readonly string[] Extensions;
    public readonly string FileDescription;

    public DialogFileFilter(string fileDescription, params string[] extensions)
    {
        Extensions = extensions;
        FileDescription = fileDescription ?? string.Empty;
    }
}