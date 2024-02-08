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
    private static readonly (HingeType type, string raw)[] HingeTypes = new (HingeType type, string raw)[]
    {
        (HingeType.Hinge4BB_R14, "4BB-R14"),
        (HingeType.HingeCEMOM_EB_755, "EB 755"),
        (HingeType.HingeOTLAV_30x120, "ОТLAV 30 х 120"),
        (HingeType.HingeR_10_102x76, "R-10 102x76"),
    };

    private static readonly (LockType type, string raw)[] LockTypes = new (LockType type, string raw)[]
    {
        (LockType.BorderRoom, "Border Room"),
        (LockType.LobZ7504, "Z7504"),
        (LockType.LH25_50SN, "LH 25-50 SN"),
    };

    public IEnumerable<ProductInformation> ReadExcelFile(string filePath)
    {
        const int HeaderRowsCount = 4;
        const int ProductTypeIndex = 2;
        const int QuarterIndex = 2;
        const int LeftHingeAmountIndex = 10;
        const int RightHingeAmountIndex = 11;
        const int LeafAmountIndex = 12;
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
                HingeAmount = ParseHingeAmount(row1[LeftHingeAmountIndex], row1[RightHingeAmountIndex]),
                LeafAmount = ParseDoorLeafAmount(row1[LeafAmountIndex]),
                LockType = ParseLockType(notes, out string lockTypeRaw),
                LockTypeRaw = lockTypeRaw,
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

        string hingeTypeTemp = Array.Find(notes, x => x.Contains(HingeMark, StringComparison.OrdinalIgnoreCase))
            ?? string.Empty;

        hingeTypeRaw = hingeTypeTemp.Trim();

        return Array.Find(HingeTypes, x => hingeTypeTemp.Contains(x.raw, StringComparison.OrdinalIgnoreCase)).type;
    }

    private static LockType ParseLockType(string[] notes, out string lockTypeRaw)
    {
        const string LockMark1 = "Замок";
        const string LockMark2 = "Защелка";

        string lockTypeTemp = Array.Find(notes, x => x.Contains(LockMark1, StringComparison.OrdinalIgnoreCase)
            || x.Contains(LockMark2, StringComparison.OrdinalIgnoreCase))
            ?? string.Empty;

        lockTypeRaw = lockTypeTemp.Trim();

        return Array.Find(LockTypes, x => lockTypeTemp.Contains(x.raw, StringComparison.OrdinalIgnoreCase)).type;
    }

    private static int? ParseHingeAmount(object left, object right)
    {
        int? leftAmount = (int?)(left is DBNull ? null : (double?)left);
        int? rightAmount = (int?)(right is DBNull ? null : (double?)right);

        return leftAmount ?? rightAmount;
    }

    private static int? ParseDoorLeafAmount(object cell)
    {
        if (cell is DBNull)
        {
            return null;
        }

        string[] parts = cell.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 0)
        {
            if (int.TryParse(parts[0], out int res))
            {
                return res;
            }

            return 1;
        }

        return null;
    }
}
