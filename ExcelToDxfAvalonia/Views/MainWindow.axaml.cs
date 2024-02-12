using System;
using Avalonia.Controls;
using Avalonia.Input;
using ExcelToDxfAvalonia.ViewModels;
using PropertyChanged;

namespace ExcelToDxfAvalonia.Views;

[DoNotNotify]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel)
        : this()
    {
        this.ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        this.DataContext = viewModel;
        viewModel.SelectedProducts = this.myDataGrid.SelectedItems;
    }

    public MainViewModel ViewModel { get; }

    private void DataGrid_DoubleTapped(object sender, TappedEventArgs e)
    {
        this.ViewModel.EditSelectedProductAsync();
    }
}
