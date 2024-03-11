using System.ComponentModel;

namespace ExcelToDxfAvalonia;

#pragma warning disable CS0067

public class ProductInformation : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public int Number { get; set; }

    public string ProductType { get; set; }

    public QuarterType QuarterType { get; set; }

    public string QuarterTypeRaw { get; set; }

    public HingeType HingeType { get; set; }

    public string HingeTypeRaw { get; set; }

    public int? HingeAmount { get; set; }

    public int? LeafAmount { get; set; }

    public LockType LockType { get; set; }

    public string LockTypeRaw { get; set; }

    public int JambLength { get; set; }

    public int JambWidth { get; set; }

    public int? InnerJambLength { get; set; }

    public int? InnerJambWidth { get; set; }

    public int LintelLength { get; set; }

    public int LintelWidth { get; set; }

    public int? InnerLintelLength { get; set; }

    public int? InnerLintelWidth { get; set; }

    public string Notes { get; set; }

    public override string ToString()
    {
        return $"{this.ProductType} {this.QuarterType} {this.HingeType} {this.LockType}";
    }
}
