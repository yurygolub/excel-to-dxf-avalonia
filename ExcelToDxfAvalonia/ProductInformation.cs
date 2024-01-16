namespace ExcelToDxfAvalonia;

public class ProductInformation
{
    public string ProductType { get; set; }

    public string Quarter { get; set; }

    public string DoorHingeType { get; set; }

    public string DoorLockType { get; set; }

    public string Length { get; set; }

    public string ExternalWidth { get; set; }

    public string InternalWidth { get; set; }

    public string Notes { get; set; }

    public override string ToString()
    {
        return $"{this.ProductType} {this.Quarter} {this.DoorHingeType} {this.DoorLockType}";
    }
}
