using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ExcelDataReader;
using ExcelToDxfAvalonia.Extensions;

namespace ExcelToDxfAvalonia.Models;

public class MainModel
{
    private readonly ObservableRangeCollection<ProductInformation> productInfoCollection = new ();

    public MainModel()
    {
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
        using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Auto-detect format, supports:
        //  - Binary Excel files (2.0-2003 format; *.xls)
        //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
        using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

        DataSet result = reader.AsDataSet();

        Console.OutputEncoding = Encoding.UTF8;

        DataTable table = result.Tables.Cast<DataTable>().First();
        DataRow[] rows = table.Rows.Cast<DataRow>().Skip(4).ToArray();
        var products = new List<ProductInformation>();

        for (int i = 0; i < rows.Length; i += 5)
        {
            DataRow row = rows[i];
            products.Add(new ProductInformation
            {
                ProductType = row[2].ToString(),
                Notes = row[14].ToString(),
                ExternalWidth = row[15].ToString(),
                Length = row[16].ToString(),
            });
        }

        this.productInfoCollection.AddRange(products);
    }
}
