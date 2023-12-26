using System;
using ExcelToDxfAvalonia.Models;

namespace ExcelToDxfAvalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MainModel mainModel;

    public MainViewModel(MainModel mainModel)
    {
        this.mainModel = mainModel ?? throw new ArgumentNullException(nameof(mainModel));
    }

    public void TestButton()
    {
    }
}
