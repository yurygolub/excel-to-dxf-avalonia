﻿using System;
using Avalonia.Controls;
using ExcelToDxfAvalonia.ViewModels;
using PropertyChanged;

namespace ExcelToDxfAvalonia.Views;

[DoNotNotify]
public partial class EditView : Window
{
    public EditView()
    {
        this.InitializeComponent();
    }

    public EditView(EditViewModel viewModel)
        : this()
    {
        this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
