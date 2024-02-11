using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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

    private void DataGrid_DoubleTapped(object sender, TappedEventArgs e)
    {
        StyledElement el = e.Source as StyledElement;
        var viewModel = new EditViewModel { Product = el.DataContext as ProductInformation };
        var editView = new EditView(viewModel);
        editView.ShowDialog(this);
    }
}
