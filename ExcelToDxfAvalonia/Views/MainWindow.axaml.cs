using System;
using Avalonia.Controls;
using ExcelToDxfAvalonia.ViewModels;

namespace ExcelToDxfAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel)
        : this()
    {
        this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
