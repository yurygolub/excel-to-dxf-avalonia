using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

    public async Task ReadExcelFile(string filePath)
    {
        IEnumerable<ProductInformation> products = this.excelParser.ReadExcelFile(filePath);
        if (!products.Any())
        {
            await Helper.OpenDialog("Сообщение", "Не удалось прочитать ни одной записи");
            return;
        }

        this.productInfoCollection.Clear();
        this.productInfoCollection.AddRange(products);
    }

    public void ExportToDxf(string directoryPath, IEnumerable<ProductInformation> selected)
    {
        List<ProductInformation> selectedList = selected.ToList();
        this.dxfExporter.ExportToDxf(directoryPath, selectedList.Count != 0 ? selectedList : this.productInfoCollection);
    }

    public void AddProduct(ProductInformation product)
    {
        _ = product ?? throw new ArgumentNullException(nameof(product));

        this.productInfoCollection.Add(product);
    }

    public void RemoveProduct(ProductInformation product)
    {
        _ = product ?? throw new ArgumentNullException(nameof(product));

        this.productInfoCollection.Remove(product);
    }
}
