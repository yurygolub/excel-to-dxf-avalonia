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
    private static readonly (QuarterType type, string raw)[] QuarterTypes = new (QuarterType type, string raw)[]
    {
        (QuarterType.Quarter32H, "32 H"),
        (QuarterType.Quarter34H, "34 H"),
        (QuarterType.Quarter42H, "42 H"),
        (QuarterType.Quarter44H, "44 H"),
        (QuarterType.Quarter47, "47"),
        (QuarterType.Quarter51, "51"),
        (QuarterType.Quarter51H, "51 H"),
        (QuarterType.Quarter53, "53"),
        (QuarterType.Quarter53H, "53 H"),
        (QuarterType.Quarter55, "55"),
        (QuarterType.Quarter61, "61"),
        (QuarterType.Quarter63, "63"),
        (QuarterType.Quarter73, "73"),
        (QuarterType.Quarter75, "75"),
    };

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
        (LockType.LobZ755, "Z755"),
        (LockType.LH25_50SN, "LH 25-50 SN"),
    };

    public IEnumerable<ProductInformation> ReadExcelFile(string filePath)
    {
        const int HeaderRowsCount = 4;

        using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Auto-detect format, supports:
        //  - Binary Excel files (2.0-2003 format; *.xls)
        //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
        using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration
        {
            FallbackEncoding = Encoding.GetEncoding(1251),
        });

        DataSet result = reader.AsDataSet();

        DataTable table = result.Tables.Cast<DataTable>().First();
        DataRow[] rows = table.Rows.Cast<DataRow>().Skip(HeaderRowsCount).ToArray();
        var products = new List<ProductInformation>();

        const int RowStep = 5;
        int i = 0;
        while (i < rows.Length - 3)
        {
            object[] row1 = rows[i]?.ItemArray;
            object[] row2 = rows[i + 1]?.ItemArray;
            object[] row3 = rows[i + 2]?.ItemArray;
            object[] row4 = rows[i + 3]?.ItemArray;

            ProductInformation product = ParseProduct(products.Count + 1, row1, row2, row3, row4);
            if (product is null)
            {
                i++;
                continue;
            }

            products.Add(product);
            i += RowStep;
        }

        return products;
    }

    private static ProductInformation ParseProduct(int number, object[] row1, object[] row2, object[] row3, object[] row4)
    {
        const int ProductTypeIndex = 2;
        const int QuarterIndex = 2;
        const int LeftHingeAmountIndex = 10;
        const int RightHingeAmountIndex = 11;
        const int LeafAmountIndex = 12;
        const int NotesIndex = 14;
        const int WidthIndex = 15;
        const int LengthIndex = 16;

        if (row1 is null ||
            row2 is null ||
            row3 is null ||
            row4 is null)
        {
            return null;
        }

        if (row1.Length <= LengthIndex ||
            row2.Length <= LengthIndex ||
            row3.Length <= LengthIndex ||
            row4.Length <= LengthIndex)
        {
            return null;
        }

        string[] notes = row1[NotesIndex].ToString().Split('*');

        if (notes.Length <= QuarterIndex)
        {
            return null;
        }

        string quarterTypeRaw = notes[QuarterIndex];

        int? jambWidth = Parse(row1[WidthIndex]);
        int? jambLength = Parse(row1[LengthIndex]);
        int? lintelWidth = Parse(row3[WidthIndex]);
        int? lintelLength = Parse(row3[LengthIndex]);

        if (!jambWidth.HasValue ||
            !jambLength.HasValue ||
            !lintelWidth.HasValue ||
            !lintelLength.HasValue)
        {
            return null;
        }

        return new ProductInformation
        {
            Number = number,
            ProductType = row1[ProductTypeIndex].ToString(),
            QuarterType = ParseQuarterType(quarterTypeRaw),
            QuarterTypeRaw = quarterTypeRaw,
            HingeType = ParseHingeType(notes, out string hingeTypeRaw),
            HingeTypeRaw = hingeTypeRaw,
            HingeAmount = ParseHingeAmount(row1[LeftHingeAmountIndex], row1[RightHingeAmountIndex]),
            LeafAmount = ParseDoorLeafAmount(row1[LeafAmountIndex]),
            LockType = ParseLockType(notes, out string lockTypeRaw),
            LockTypeRaw = lockTypeRaw,
            JambWidth = jambWidth.Value,
            JambLength = jambLength.Value,
            InnerJambWidth = Parse(row2[WidthIndex]),
            InnerJambLength = Parse(row2[LengthIndex]),
            LintelWidth = lintelWidth.Value,
            LintelLength = lintelLength.Value,
            InnerLintelWidth = Parse(row4[WidthIndex]),
            InnerLintelLength = Parse(row4[LengthIndex]),
        };

        static int? Parse(object value)
        {
            if (value is double casted)
            {
                return (int)casted;
            }

            if (int.TryParse(value?.ToString(), out int result))
            {
                return result;
            }

            return null;
        }
    }

    private static QuarterType ParseQuarterType(string quarterTypeRaw)
    {
        return Array.Find(QuarterTypes, x => quarterTypeRaw.Contains(x.raw, StringComparison.OrdinalIgnoreCase)).type;
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
