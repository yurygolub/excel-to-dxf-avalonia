namespace ExcelToDxfAvalonia;

public class ProductInformation
{
    public string ProductType { get; set; }

    public string Quarter { get; set; }

    public HingeType HingeType { get; set; }

    public string HingeTypeRaw { get; set; }

    public int? HingeAmount { get; set; }

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
        return $"{this.ProductType} {this.Quarter} {this.HingeType} {this.LockType}";
    }
}
