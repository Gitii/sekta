using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using Sekta.Core.IO;
using Splat;

namespace Sekta.Core.ModelView;

public class AdmxAdmlContextViewModel : ReactiveObject
{
    string _admxFilePath;

    public string AdmxFilePath
    {
        get { return _admxFilePath; }
        set { this.RaiseAndSetIfChanged(ref _admxFilePath, value); }
    }

    private SourceList<string> _admlFilePathList;
    private readonly ReadOnlyObservableCollection<string> _admlFilePaths;

    public ReadOnlyObservableCollection<string> AdmlFilePaths => _admlFilePaths;

    string _selectedAdmlFilePath;

    public string SelectedAdmlFilePath
    {
        get { return _selectedAdmlFilePath; }
        set { this.RaiseAndSetIfChanged(ref _selectedAdmlFilePath, value); }
    }

    bool _autoAddAdmlFiles;

    public bool AutoAddAdmlFiles
    {
        get { return _autoAddAdmlFiles; }
        set { this.RaiseAndSetIfChanged(ref _autoAddAdmlFiles, value); }
    }

    private ObservableAsPropertyHelper<bool> _isAllValid;
    public bool IsAllValid => _isAllValid.Value;

    public ReactiveCommand<Unit, Unit> SelectAdmxFileCommand;
    public ReactiveCommand<Unit, Unit> SelectMoreAdmlFilesCommand;
    public ReactiveCommand<Unit, Unit> RemoveSelectedAdmlFileCommand;

    public AdmxAdmlContextViewModel()
    {
        _admlFilePathList = new SourceList<string>();

        _admlFilePathList
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _admlFilePaths)
            .Subscribe();

        SelectAdmxFileCommand = ReactiveCommand.CreateFromTask(SelectAdmxFile);

        SelectMoreAdmlFilesCommand = ReactiveCommand.CreateFromTask(AddAdmlFile);

        RemoveSelectedAdmlFileCommand = ReactiveCommand.CreateFromTask(
            RemoveSelectedAdmlFile,
            this.WhenAny((vm) => vm.AdmxFilePath, (path) => !string.IsNullOrEmpty(path.Value))
        );

        this.WhenAny((vm) => vm.AdmxFilePath, (path) => !string.IsNullOrEmpty(path.Value))
            .ToProperty(this, (x) => x.IsAllValid, out _isAllValid);

        AdmxFilePath = string.Empty;
        AutoAddAdmlFiles = true;
    }

    private async Task SelectAdmxFile()
    {
        IOService service = Locator.Current.GetService<IOService>();

        var filePath = await service.SelectSingleInputFile(
            new DialogFileFilter("Schema Definition File", "*.admx")
        );
        if (filePath != null)
        {
            AdmxFilePath = filePath;
            if (AutoAddAdmlFiles)
            {
                _admlFilePathList.Clear();

                var expectedFileName = Path.GetFileNameWithoutExtension(filePath) + ".adml";

                foreach (
                    string file in await service.FindFiles(
                        Path.GetDirectoryName(AdmxFilePath),
                        expectedFileName
                    )
                )
                {
                    _admlFilePathList.Add(file);
                }
            }
        }
    }

    private async Task AddAdmlFile()
    {
        IOService service = Locator.Current.GetService<IOService>();

        var files = await service.SelectMultipleInputFiles(
            new DialogFileFilter("Schema Resource File", "*.adml")
        );
        _admlFilePathList.AddRange(files.Except(_admlFilePathList.Items).Distinct());
    }

    private async Task RemoveSelectedAdmlFile()
    {
        if (SelectedAdmlFilePath != null)
        {
            _admlFilePathList.Remove(SelectedAdmlFilePath);
            SelectedAdmlFilePath = null;
        }
    }
}
