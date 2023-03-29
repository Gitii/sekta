using System.Collections.Generic;
using System.Linq;

namespace Sekta.Core.ModelView;

public readonly struct AdmxAndAdmlFiles
{
    public readonly string AdmxFilePath;
    public readonly string[] AdmlFilePaths;

    public AdmxAndAdmlFiles(string admxFilePath, IEnumerable<string> admlFilePaths)
    {
        AdmxFilePath = admxFilePath;
        AdmlFilePaths = admlFilePaths.ToArray();
    }
}
