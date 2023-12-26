using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using ExcelToDxfAvalonia.Models;

namespace ExcelToDxfAvalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainModel model;

    private string inFilePath;

    public MainViewModel(MainModel model)
    {
        this.model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public string FileName { get; set; }

    public void SwitchConsoleVisibility() => MainModel.SwitchConsoleVisibility();

    public async Task OpenFileAsync()
    {
        var filters = new List<FilePickerFileType>
        {
            new FilePickerFileType("Excel file (*.xlsx)")
            {
                Patterns = new List<string> { "*.xlsx" },
            },
        };

        if (!await this.OpenFileAsync("Open CanHelper log file", filters))
        {
            return;
        }

        this.model.ReadExcelFile(this.inFilePath);
    }

    private async Task<bool> OpenFileAsync(string title, List<FilePickerFileType> filters)
    {
        filters.Add(new FilePickerFileType("All files (*.*)")
        {
            Patterns = new List<string> { "*.*" },
        });

        IReadOnlyList<IStorageFile> files = null;
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            TopLevel topLevel = TopLevel.GetTopLevel(desktop.MainWindow);

            files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = filters,
            });
        }

        if (files?.Count > 0)
        {
            string file = files[0].Path.LocalPath;

            this.inFilePath = file;
            this.FileName = Path.GetFileName(file);
            return true;
        }

        return false;
    }
}
