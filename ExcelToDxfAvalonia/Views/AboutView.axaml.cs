using System;
using Avalonia.Controls;
using ExcelToDxfAvalonia.ViewModels;

namespace ExcelToDxfAvalonia.Views;

public partial class AboutView : Window
{
    public AboutView()
    {
        this.InitializeComponent();
    }

    public AboutView(AboutViewModel viewModel)
        : this()
    {
        this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
