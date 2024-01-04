﻿using System;
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

namespace ExcelToDxfAvalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainModel model;
    private readonly IServiceProvider serviceProvider;

    private AboutView aboutView;

    private string inFilePath;

    public MainViewModel(MainModel model, IServiceProvider serviceProvider)
    {
        this.model = model ?? throw new ArgumentNullException(nameof(model));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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

        if (!await this.OpenFileAsync("Open CanHelper log file", filters))
        {
            return;
        }

        this.model.ReadExcelFile(this.inFilePath);
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
}
