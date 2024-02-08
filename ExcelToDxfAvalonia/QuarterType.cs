using System.ComponentModel;

namespace ExcelToDxfAvalonia;

public enum QuarterType
{
    Undefined,

    [Description("32H")]
    Quarter32H,

    [Description("34H")]
    Quarter34H,

    [Description("42H")]
    Quarter42H,

    [Description("44H")]
    Quarter44H,

    [Description("47")]
    Quarter47,

    [Description("51")]
    Quarter51,

    [Description("51H")]
    Quarter51H,

    [Description("53")]
    Quarter53,

    [Description("53H")]
    Quarter53H,

    [Description("55")]
    Quarter55,

    [Description("61")]
    Quarter61,

    [Description("63")]
    Quarter63,

    [Description("73")]
    Quarter73,

    [Description("75")]
    Quarter75,
}
