using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;

namespace ExcelToDxfAvalonia;

public class ExcelParser
{
    public IEnumerable<ProductInformation> ReadExcelFile(string filePath)
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

        return products;
    }
}
