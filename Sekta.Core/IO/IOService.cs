using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sekta.Core.IO;

public interface IOService
{
    Task<string> SelectSingleOutputFileAsync(params DialogFileFilter[] filters);
    Task<string> SelectSingleInputFileAsync(params DialogFileFilter[] filters);
    Task<string[]> SelectMultipleInputFilesAsync(params DialogFileFilter[] filters);
    Task<bool> FileExistsAsync(params string[] files);
    Task<bool> FileExistsAsync(IEnumerable<string> files);
    Task<Stream> OpenFileReadAsync(string filePath);
    Task<Stream> CreateOrOpenFileWriteAsync(string filePath);
    Task<string[]> FindFilesAsync(string directoryPath, string filter = null);
    Task<Stream> CreateOrOverwriteFileWriteAsync(string filePath);
}
