using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using ExcelToDxfAvalonia.Models;
using ExcelToDxfAvalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;

namespace ExcelToDxfAvalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainModel model;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<MainViewModel> logger;

    private AboutView aboutView;

    private string inFilePath;
    private string exportFolderPath;

    public MainViewModel(MainModel model, IServiceProvider serviceProvider, ILogger<MainViewModel> logger)
    {
        this.model = model ?? throw new ArgumentNullException(nameof(model));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ReadOnlyObservableCollection<ProductInformation> ProductInfoCollection => this.model.ProductInfoPublicCollection;

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

        if (!await this.OpenFileAsync("Open excel file", filters))
        {
            return;
        }

        try
        {
            this.model.ReadExcelFile(this.inFilePath);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception:");
            await this.OpenDialog("Ошибка", $"Возникла ошибка при чтении excel файла{Environment.NewLine}{ex.Message}");
        }
    }

    public async Task ExportToDxfAsync()
    {
        if (!await this.OpenFolderAsync("Open directory"))
        {
            return;
        }

        try
        {
            this.model.ExportToDxf(this.exportFolderPath);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception:");
            await this.OpenDialog("Ошибка", $"Возникла ошибка при экспорте dxf файла{Environment.NewLine}{ex.Message}");
        }
    }

    public void OpenAboutWindow()
    {
        if (this.aboutView is null)
        {
            MainWindow ownerWindow = this.serviceProvider.GetRequiredService<MainWindow>();
            this.aboutView = this.serviceProvider.GetRequiredService<AboutView>();
            this.aboutView.Closed += (o, e) =>
            {
                this.aboutView = null;
            };

            this.aboutView.ShowDialog(ownerWindow);
        }
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

    private async Task<bool> OpenFolderAsync(string title)
    {
        IReadOnlyList<IStorageFolder> folders = null;
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            TopLevel topLevel = TopLevel.GetTopLevel(desktop.MainWindow);

            folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
            });
        }

        if (folders?.Count > 0)
        {
            string folder = folders[0].Path.LocalPath;

            this.exportFolderPath = folder;
            return true;
        }

        return false;
    }

    private async Task OpenDialog(string title, string message)
    {
        IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok);
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            await box.ShowWindowDialogAsync(desktop.MainWindow);
        }
    }
}
