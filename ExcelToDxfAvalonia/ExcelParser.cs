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

        for (int i = 0; i < rows.Length - 3; i += 5)
        {
            DataRow row1 = rows[i];
            DataRow row2 = rows[i + 1];
            DataRow row3 = rows[i + 2];
            DataRow row4 = rows[i + 3];

            string productType = row1[2].ToString();
            if (string.IsNullOrWhiteSpace(productType))
            {
                continue;
            }

            string[] notes = row1[14].ToString().Split('*');

            products.Add(new ProductInformation
            {
                ProductType = productType,
                Quarter = notes[2],
                DoorHingeType = notes[7],
                DoorLockType = notes[8],
                Notes = notes.Aggregate(new StringBuilder(), (acc, i) => acc.AppendLine(i)).ToString(),
                JambWidth = (int)(double)row1[15],
                JambLength = (int)(double)row1[16],
                InnerJambWidth = (int?)(row2[15] is DBNull ? null : (double?)row2[15]),
                InnerJambLength = (int?)(row2[16] is DBNull ? null : (double?)row2[16]),
                LintelWidth = (int)(double)row3[15],
                LintelLength = (int)(double)row3[16],
                InnerLintelWidth = (int?)(row4[15] is DBNull ? null : (double?)row4[15]),
                InnerLintelLength = (int?)(row4[15] is DBNull ? null : (double?)row4[16]),
            });
        }

        return products;
    }
}
