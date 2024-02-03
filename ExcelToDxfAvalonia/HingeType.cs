using System.ComponentModel;

namespace ExcelToDxfAvalonia;

public enum HingeType
{
    Undefined,

    [Description("EB 755")]
    HingeEB_755,

    [Description("R-10 102x76")]
    HingeR_10_102x76,

    [Description("4BB-R14")]
    Hinge4BB_R14,

    [Description("ОТLAV 30 х 120")]
    HingeOTLAV_30x120,
}
