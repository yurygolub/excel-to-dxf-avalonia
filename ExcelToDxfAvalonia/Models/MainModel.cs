using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using ExcelToDxfAvalonia.Extensions;

namespace ExcelToDxfAvalonia.Models;

public class MainModel
{
    private readonly ExcelParser excelParser;
    private readonly DxfExporter dxfExporter;

    private readonly ObservableRangeCollection<ProductInformation> productInfoCollection = new ();

    public MainModel(ExcelParser excelParser, DxfExporter dxfExporter)
    {
        this.excelParser = excelParser ?? throw new ArgumentNullException(nameof(excelParser));
        this.dxfExporter = dxfExporter ?? throw new ArgumentNullException(nameof(dxfExporter));
        this.ProductInfoPublicCollection = new ReadOnlyObservableCollection<ProductInformation>(this.productInfoCollection);
    }

    public ReadOnlyObservableCollection<ProductInformation> ProductInfoPublicCollection { get; }

    public static void SwitchConsoleVisibility()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (ConsoleManager.IsConsoleVisible())
            {
                ConsoleManager.HideConsole();
            }
            else
            {
                ConsoleManager.ShowConsole();
            }
        }
    }

    public void ReadExcelFile(string filePath)
    {
        this.productInfoCollection.Clear();
        this.productInfoCollection.AddRange(this.excelParser.ReadExcelFile(filePath));
    }

    public void ExportToDxf(string directoryPath)
    {
        this.dxfExporter.ExportToDxf(directoryPath, this.productInfoCollection);
    }
}
