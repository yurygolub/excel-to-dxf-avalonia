using System.ComponentModel;

namespace ExcelToDxfAvalonia
{
    public enum LockType
    {
        Undefined,

        [Description("Border Room")]
        BorderRoom,

        [Description("LOB Z7504")]
        LobZ7504,

        [Description("LH 25-50 SN")]
        LH25_50SN,
    }
}
