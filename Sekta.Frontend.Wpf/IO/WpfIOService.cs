﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Sekta.Core.IO;

namespace Sekta.Frontend.Wpf.IO;

public class WpfIOService : IOService
{
    public async Task<string> SelectSingleOutputFileAsync(params DialogFileFilter[] filters)
    {
        var sfd = new SaveFileDialog()
        {
            Filter = string.Join(
                '|',
                filters.Select((f) => $"{f.FileDescription}|{string.Join(';', f.Extensions)}")
            ),
            CheckFileExists = false,
            OverwritePrompt = true
        };

        if (sfd.ShowDialog().GetValueOrDefault())
        {
            return sfd.FileName;
        }
        else
        {
            return null;
        }
    }

    public async Task<string> SelectSingleInputFileAsync(params DialogFileFilter[] filters)
    {
        OpenFileDialog ofd = new OpenFileDialog()
        {
            Filter = string.Join(
                '|',
                filters.Select((f) => $"{f.FileDescription}|{string.Join(';', f.Extensions)}")
            ),
            CheckFileExists = true,
            Multiselect = false,
        };

        if (ofd.ShowDialog().GetValueOrDefault())
        {
            return ofd.FileName;
        }
        else
        {
            return null;
        }
    }

    public async Task<string[]> SelectMultipleInputFilesAsync(params DialogFileFilter[] filters)
    {
        OpenFileDialog ofd = new OpenFileDialog()
        {
            Filter = string.Join(
                '|',
                filters.Select((f) => $"{f.FileDescription}|{string.Join(';', f.Extensions)}")
            ),
            CheckFileExists = true,
            Multiselect = true,
        };

        if (ofd.ShowDialog().GetValueOrDefault())
        {
            return ofd.FileNames;
        }
        else
        {
            return Array.Empty<string>();
        }
    }

    public Task<bool> FileExistsAsync(params string[] files)
    {
        return FileExistsAsync((IEnumerable<string>)files);
    }

    public async Task<bool> FileExistsAsync(IEnumerable<string> files)
    {
        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                return false;
            }
        }

        return true;
    }

    public async Task<Stream> OpenFileReadAsync(string filePath)
    {
        return File.OpenRead(filePath);
    }

    public async Task<Stream> CreateOrOpenFileWriteAsync(string filePath)
    {
        return File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    public async Task<Stream> CreateOrOverwriteFileWriteAsync(string filePath)
    {
        return File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
    }

    public async Task<string[]> FindFilesAsync(string directoryPath, string filter = null)
    {
        return Directory
            .EnumerateFiles(directoryPath, filter, SearchOption.AllDirectories)
            .ToArray();
    }
}
