using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

public interface IOService
{
    Task<string> SelectSingleOutputFile(params DialogFileFilter[] filters);
    Task<string> SelectSingleInputFile(params DialogFileFilter[] filters);
    Task<string[]> SelectMultipleInputFiles(params DialogFileFilter[] filters);
    Task<bool> FileExists(params string[] files);
    Task<bool> FileExists(IEnumerable<string> files);
    Task<Stream> OpenFileRead(string filePath);
    Task<Stream> CreateOrOpenFileWrite(string filePath);
    Task<string[]> FindFiles(string directoryPath, string filter = null);
    Task<Stream> CreateOrOverwriteFileWrite(string filePath);
}
