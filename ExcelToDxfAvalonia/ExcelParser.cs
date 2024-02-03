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
        const int HeaderRowsCount = 4;
        const int ProductTypeIndex = 2;
        const int QuarterIndex = 2;
        const int DoorLockTypeIndex = 8;
        const int NotesIndex = 14;
        const int WidthIndex = 15;
        const int LengthIndex = 16;

        using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Auto-detect format, supports:
        //  - Binary Excel files (2.0-2003 format; *.xls)
        //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
        using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

        DataSet result = reader.AsDataSet();

        Console.OutputEncoding = Encoding.UTF8;

        DataTable table = result.Tables.Cast<DataTable>().First();
        DataRow[] rows = table.Rows.Cast<DataRow>().Skip(HeaderRowsCount).ToArray();
        var products = new List<ProductInformation>();

        const int RowStep = 5;
        for (int i = 0; i < rows.Length - 3; i += RowStep)
        {
            DataRow row1 = rows[i];
            DataRow row2 = rows[i + 1];
            DataRow row3 = rows[i + 2];
            DataRow row4 = rows[i + 3];

            string productType = row1[ProductTypeIndex].ToString();
            if (string.IsNullOrWhiteSpace(productType))
            {
                continue;
            }

            string[] notes = row1[NotesIndex].ToString().Split('*');

            products.Add(new ProductInformation
            {
                ProductType = productType,
                Quarter = notes[QuarterIndex],
                HingeType = ParseHingeType(notes, out string hingeTypeRaw),
                HingeTypeRaw = hingeTypeRaw,
                DoorLockType = notes[DoorLockTypeIndex],
                Notes = notes.Aggregate(new StringBuilder(), (acc, i) => acc.AppendLine(i)).ToString(),
                JambWidth = (int)(double)row1[WidthIndex],
                JambLength = (int)(double)row1[LengthIndex],
                InnerJambWidth = (int?)(row2[WidthIndex] is DBNull ? null : (double?)row2[WidthIndex]),
                InnerJambLength = (int?)(row2[LengthIndex] is DBNull ? null : (double?)row2[LengthIndex]),
                LintelWidth = (int)(double)row3[WidthIndex],
                LintelLength = (int)(double)row3[LengthIndex],
                InnerLintelWidth = (int?)(row4[WidthIndex] is DBNull ? null : (double?)row4[WidthIndex]),
                InnerLintelLength = (int?)(row4[LengthIndex] is DBNull ? null : (double?)row4[LengthIndex]),
            });
        }

        return products;
    }

    private static HingeType ParseHingeType(string[] notes, out string hingeTypeRaw)
    {
        const string HingeMark = "Петля";

        hingeTypeRaw = Array.Find(notes, x => x.Contains(HingeMark, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Could not parse hinge type.");

        const string HingeEB_755 = "EB 755";
        const string HingeR_10_102x76 = "R-10 102x76";
        const string Hinge4BB_R14 = "4BB-R14";
        const string HingeOTLAV_30x120 = "ОТLAV 30 х 120";

        return hingeTypeRaw.Contains(HingeEB_755) ? HingeType.HingeEB_755
            : hingeTypeRaw.Contains(HingeR_10_102x76) ? HingeType.HingeR_10_102x76
            : hingeTypeRaw.Contains(Hinge4BB_R14) ? HingeType.Hinge4BB_R14
            : hingeTypeRaw.Contains(HingeOTLAV_30x120) ? HingeType.HingeOTLAV_30x120
            : HingeType.Undefined;
    }
}
