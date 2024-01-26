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
using netDxf;
using netDxf.Collections;
using netDxf.Entities;

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

        for (int i = 0; i < rows.Length - 1; i += 5)
        {
            DataRow row = rows[i];
            DataRow nextRow = rows[i + 1];

            string productType = row[2].ToString();
            if (string.IsNullOrWhiteSpace(productType))
            {
                continue;
            }

            string[] notes = row[14].ToString().Split('*');

            products.Add(new ProductInformation
            {
                ProductType = productType,
                Quarter = notes[2],
                DoorHingeType = notes[7],
                DoorLockType = notes[8],
                Notes = notes.Aggregate(new StringBuilder(), (acc, i) => acc.AppendLine(i)).ToString(),
                ExternalWidth = row[15].ToString(),
                InternalWidth = nextRow[15].ToString(),
                Length = row[16].ToString(),
            });
        }

        this.productInfoCollection.Clear();
        this.productInfoCollection.AddRange(products);
    }

    public void ExportToDxf(string directoryPath)
    {
        Directory.CreateDirectory(directoryPath);
        int counter = 0;
        foreach (ProductInformation product in this.productInfoCollection)
        {
            string filePath = Path.Combine(directoryPath, $"{counter} - {product.ProductType}.dxf");
            ExportToDxfFile(filePath, product);
            counter++;
        }

        static void ExportToDxfFile(string filePath, ProductInformation product)
        {
            const int Distance = 100;
            DxfDocument doc = new DxfDocument();

            int width = int.Parse(product.ExternalWidth);
            int length = int.Parse(product.Length);

            AddRectangle(doc.Entities, new Vector2(0, 0), new Vector2(width, length));
            AddRectangle(doc.Entities, new Vector2(width + Distance, 0), new Vector2((width * 2) + Distance, length));

            doc.Save(filePath);
        }
    }

    private static void AddRectangle(DrawingEntities entities, Vector2 leftBottom, Vector2 rightUp)
    {
        entities.Add(new Line(leftBottom, new Vector2(leftBottom.X, rightUp.Y))); // left
        entities.Add(new Line(new Vector2(leftBottom.X, rightUp.Y), rightUp)); // up
        entities.Add(new Line(rightUp, new Vector2(rightUp.X, leftBottom.Y))); // right
        entities.Add(new Line(new Vector2(rightUp.X, leftBottom.Y), leftBottom)); // bottom
    }
}
