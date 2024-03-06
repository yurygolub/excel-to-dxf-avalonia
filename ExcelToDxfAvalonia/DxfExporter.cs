using System;
using System.Collections.Generic;
using System.IO;
using netDxf;
using netDxf.Collections;
using netDxf.Entities;

namespace ExcelToDxfAvalonia;

public class DxfExporter
{
    public void ExportToDxf(string directoryPath, IEnumerable<ProductInformation> products)
    {
        Directory.CreateDirectory(directoryPath);
        int counter = 0;
        foreach (ProductInformation product in products)
        {
            string filePath = Path.Combine(directoryPath, $"{counter} - {product.ProductType}.dxf");
            ExportToDxfFile(filePath, product);
            counter++;
        }

        static void ExportToDxfFile(string filePath, ProductInformation product)
        {
            const int LongDistance = 200;
            const int ShortDistance = 100;

            DxfDocument doc = new DxfDocument();

            Vector2 leftBottom = Vector2.Zero;

            // left jamb
            AddRectangle(doc.Entities, leftBottom, new Vector2(product.JambWidth, product.JambLength));

            if (product.LeafAmount == 1)
            {
                AddLocks(doc.Entities, leftBottom, product);
            }
            else if (product.LeafAmount == 2)
            {
                AddHinges(doc.Entities, leftBottom, product);
            }

            leftBottom = new Vector2(product.JambWidth + ShortDistance, leftBottom.Y);

            // right jamb
            AddRectangle(
                doc.Entities,
                leftBottom,
                new Vector2(leftBottom.X + product.JambWidth, product.JambLength));

            AddHinges(doc.Entities, leftBottom, product);

            leftBottom = new Vector2(leftBottom.X + product.JambWidth + LongDistance, leftBottom.Y);

            Vector2 leftUp = new Vector2(0, product.JambLength + LongDistance);

            // lintel
            AddRectangle(
                doc.Entities,
                leftUp,
                new Vector2(leftUp.X + product.LintelLength, leftUp.Y + product.LintelWidth));

            leftUp = new Vector2(leftUp.X, leftUp.Y + product.LintelWidth + ShortDistance);

            if (product.InnerJambLength.HasValue
                && product.InnerJambWidth.HasValue
                && product.InnerLintelLength.HasValue
                && product.InnerLintelWidth.HasValue)
            {
                // left inner jamb
                AddRectangle(
                    doc.Entities,
                    leftBottom,
                    new Vector2(leftBottom.X + product.InnerJambWidth.Value, product.InnerJambLength.Value));

                leftBottom = new Vector2(leftBottom.X + product.InnerJambWidth.Value + ShortDistance, leftBottom.Y);

                // right inner jamb
                AddRectangle(
                    doc.Entities,
                    leftBottom,
                    new Vector2(leftBottom.X + product.InnerJambWidth.Value, product.InnerJambLength.Value));

                // inner lintel
                AddRectangle(
                    doc.Entities,
                    leftUp,
                    new Vector2(leftUp.X + product.InnerLintelLength.Value, leftUp.Y + product.InnerLintelWidth.Value));
            }

            doc.Save(filePath);
        }
    }

    private static void AddHinges(DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
    {
        switch (product.HingeType)
        {
            case HingeType.HingeCEMOM_EB_755:
                AddHingeAmount(AddHingeCEMOM_EB_755, entities, leftBottom, product);
                break;

            case HingeType.HingeR_10_102x76:
                AddHingeAmount(AddHingeR_10_102x76, entities, leftBottom, product);
                break;

            case HingeType.Hinge4BB_R14:
                AddHingeAmount(AddHinge4BB_R14, entities, leftBottom, product);
                break;

            case HingeType.HingeOTLAV_30x120:
                AddHingeAmount(AddHingeOTLAV_30x120, entities, leftBottom, product);
                break;

            default:
                break;
        }

        static void AddHingeAmount(
            Action<DrawingEntities, Vector2, ProductInformation> addHinge,
            DrawingEntities entities,
            Vector2 leftBottom,
            ProductInformation product)
        {
            const int BottomOffset = 256;
            const int UpOffset = 313;
            const int DistanceBetweenHinges = 500;

            if (product.HingeAmount == 4)
            {
                addHinge(
                    entities,
                    new Vector2(leftBottom.X, leftBottom.Y + product.JambLength - (UpOffset + DistanceBetweenHinges + DistanceBetweenHinges)),
                    product);
            }

            if (product.HingeAmount >= 3)
            {
                addHinge(
                    entities,
                    new Vector2(leftBottom.X, leftBottom.Y + product.JambLength - (UpOffset + DistanceBetweenHinges)),
                    product);
            }

            if (product.HingeAmount >= 2)
            {
                addHinge(entities, new Vector2(leftBottom.X, leftBottom.Y + product.JambLength - UpOffset), product);
                addHinge(entities, new Vector2(leftBottom.X, leftBottom.Y + BottomOffset), product);
            }
        }

        static void AddHingeCEMOM_EB_755(DrawingEntities entities, Vector2 leftCenter, ProductInformation product)
        {
            const double Width = 4.2;
            const double Length = 35;
            const double LeftHingeOffset = 33.7;
            const double LeftCircleOffset = LeftHingeOffset + Width + 27.3;
            const double Radius = 8.5 / 2;

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + LeftHingeOffset, leftCenter.Y - (Length / 2));

            AddRectangle(entities, hingeLeftBottom, new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Length));

            Vector2 center = new Vector2(leftCenter.X + LeftCircleOffset, leftCenter.Y);
            entities.Add(new Circle(center, Radius));
        }

        static void AddHingeR_10_102x76(DrawingEntities entities, Vector2 leftCenter, ProductInformation product)
        {
            const double Width = 29.6;
            const double Length = 102.5;
            const double LeftOffset = 50;
            const double Radius = 10;

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + LeftOffset, leftCenter.Y - (Length / 2));
            Vector2 hingeLeftUp = new Vector2(hingeLeftBottom.X, hingeLeftBottom.Y + Length);

            entities.Add(new Line(hingeLeftBottom, hingeLeftUp)); // left

            entities.Add(new Line(hingeLeftUp, new Vector2(hingeLeftUp.X + (Width - Radius), hingeLeftUp.Y))); // up

            entities.Add(new Line(
                new Vector2(hingeLeftUp.X + Width, hingeLeftUp.Y - Radius),
                new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Radius))); // right

            entities.Add(new Line(hingeLeftBottom, new Vector2(hingeLeftBottom.X + (Width - Radius), hingeLeftBottom.Y))); // down

            entities.Add(new Arc(new Vector2(hingeLeftUp.X + (Width - Radius), hingeLeftUp.Y - Radius), Radius, 0, 90));
            entities.Add(new Arc(new Vector2(hingeLeftBottom.X + (Width - Radius), hingeLeftBottom.Y + Radius), Radius, 270, 360));
        }

        static void AddHinge4BB_R14(DrawingEntities entities, Vector2 leftCenter, ProductInformation product)
        {
            const double Width = 28.6;
            const double Length = 100.5;
            const double LeftOffset = 50;
            const double Radius = 14;

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + LeftOffset, leftCenter.Y - (Length / 2));
            Vector2 hingeLeftUp = new Vector2(hingeLeftBottom.X, hingeLeftBottom.Y + Length);

            entities.Add(new Line(hingeLeftBottom, hingeLeftUp)); // left

            entities.Add(new Line(hingeLeftUp, new Vector2(hingeLeftUp.X + (Width - Radius), hingeLeftUp.Y))); // up

            entities.Add(new Line(
                new Vector2(hingeLeftUp.X + Width, hingeLeftUp.Y - Radius),
                new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Radius))); // right

            entities.Add(new Line(hingeLeftBottom, new Vector2(hingeLeftBottom.X + (Width - Radius), hingeLeftBottom.Y))); // down

            entities.Add(new Arc(new Vector2(hingeLeftUp.X + (Width - Radius), hingeLeftUp.Y - Radius), Radius, 0, 90));
            entities.Add(new Arc(new Vector2(hingeLeftBottom.X + (Width - Radius), hingeLeftBottom.Y + Radius), Radius, 270, 360));
        }

        static void AddHingeOTLAV_30x120(DrawingEntities entities, Vector2 leftCenter, ProductInformation product)
        {
            const double Width = 30.4;
            const double Length = 102.4;
            const double Radius = Width / 2;

            double leftOffset;

            switch (product.QuarterType)
            {
                case QuarterType.Quarter51:
                case QuarterType.Quarter53:
                    leftOffset = 58.5;
                    break;

                case QuarterType.Quarter55:
                case QuarterType.Quarter61:
                case QuarterType.Quarter63:
                    leftOffset = 57.5;
                    break;

                default:
                    return;
            }

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + leftOffset, leftCenter.Y - (Length / 2));
            Vector2 hingeLeftUp = new Vector2(hingeLeftBottom.X, hingeLeftBottom.Y + Length);

            entities.Add(new Line(
                new Vector2(hingeLeftBottom.X, hingeLeftBottom.Y + Radius),
                new Vector2(hingeLeftUp.X, hingeLeftUp.Y - Radius))); // left

            entities.Add(new Line(
                new Vector2(hingeLeftUp.X + Width, hingeLeftUp.Y - Radius),
                new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Radius))); // right

            entities.Add(new Arc(new Vector2(hingeLeftUp.X + Radius, hingeLeftUp.Y - Radius), Radius, 0, 180));
            entities.Add(new Arc(new Vector2(hingeLeftBottom.X + Radius, hingeLeftBottom.Y + Radius), Radius, 180, 360));
        }
    }

    private static void AddLocks(DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
    {
        switch (product.LockType)
        {
            case LockType.BorderRoom:
                AddBorderRoomLock(entities, leftBottom, product);
                break;

            case LockType.LobZ7504:
                AddLobZ7504Lock(entities, leftBottom, product);
                break;

            case LockType.LobZ755:
                AddLobZ755Lock(entities, leftBottom, product);
                break;

            case LockType.LH25_50SN:
                AddLH25_50SNLock(entities, leftBottom, product);
                break;

            default:
                break;
        }

        static void AddBorderRoomLock(DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
        {
            const int BottomOffset = 981;

            double? leftOffset = product.QuarterType switch
            {
                QuarterType.Quarter32H => 52,
                QuarterType.Quarter47 => 63,
                QuarterType.Quarter51 => 64.5,
                QuarterType.Quarter53 => 65.5,
                QuarterType.Quarter55 => 62.3,
                _ => null,
            };

            if (!leftOffset.HasValue)
            {
                return;
            }

            Vector2 lockLeftBottom = new Vector2(leftBottom.X + leftOffset.Value, leftBottom.Y + BottomOffset);
            AddUpperLockPart(entities, lockLeftBottom);
        }

        static void AddLobZ7504Lock(DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
        {
            const int Offset = 30;
            const int Height = 50;
            const int BottomOffset = 973;

            double leftUpperPartOffset;
            double leftLowerPartOffset;

            switch (product.QuarterType)
            {
                case QuarterType.Quarter32H:
                    leftUpperPartOffset = 52;
                    leftLowerPartOffset = 56;
                    break;

                case QuarterType.Quarter47:
                    leftUpperPartOffset = 61.3;
                    leftLowerPartOffset = 63;
                    break;

                case QuarterType.Quarter51:
                    leftUpperPartOffset = 63.5;
                    leftLowerPartOffset = 64;
                    break;

                case QuarterType.Quarter53:
                    leftUpperPartOffset = 64.3;
                    leftLowerPartOffset = 65;
                    break;

                case QuarterType.Quarter55:
                    leftUpperPartOffset = 65.5;
                    leftLowerPartOffset = 67;
                    break;

                default:
                    return;
            }

            Vector2 lockLeftBottom = new Vector2(leftBottom.X + leftLowerPartOffset, leftBottom.Y + BottomOffset);
            AddLowerLockPart(entities, lockLeftBottom);

            lockLeftBottom = new Vector2(leftBottom.X + leftUpperPartOffset, leftBottom.Y + BottomOffset + Offset + Height);
            AddUpperLockPart(entities, lockLeftBottom);
        }

        static void AddLobZ755Lock(DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
        {
            const int Offset = 30;
            const int Height = 50;
            const int BottomOffset = 973;

            double leftUpperPartOffset;
            double leftLowerPartOffset;

            switch (product.QuarterType)
            {
                case QuarterType.Quarter32H:
                case QuarterType.Quarter42H:
                case QuarterType.Quarter44H:
                case QuarterType.Quarter51H:
                case QuarterType.Quarter53H:
                    leftUpperPartOffset = 52;
                    leftLowerPartOffset = 56;
                    break;

                case QuarterType.Quarter34H:
                case QuarterType.Quarter51:
                case QuarterType.Quarter53:
                case QuarterType.Quarter61:
                case QuarterType.Quarter63:
                case QuarterType.Quarter73:
                case QuarterType.Quarter75:
                    leftUpperPartOffset = 62;
                    leftLowerPartOffset = 66;
                    break;

                default:
                    return;
            }

            Vector2 lockLeftBottom = new Vector2(leftBottom.X + leftLowerPartOffset, leftBottom.Y + BottomOffset);
            AddLowerLockPart(entities, lockLeftBottom);

            lockLeftBottom = new Vector2(leftBottom.X + leftUpperPartOffset, leftBottom.Y + BottomOffset + Offset + Height);
            AddUpperLockPart(entities, lockLeftBottom);
        }

        static void AddLH25_50SNLock(DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
        {
            const int Offset = 11;
            const int Height = 50;
            const int BottomOffset = 953;

            double leftUpperPartOffset;
            double leftLowerPartOffset;

            switch (product.QuarterType)
            {
                case QuarterType.Quarter32H:
                case QuarterType.Quarter34H:
                    leftUpperPartOffset = 52;
                    leftLowerPartOffset = 54;
                    break;

                case QuarterType.Quarter47:
                    leftUpperPartOffset = 61.3;
                    leftLowerPartOffset = 63;
                    break;

                case QuarterType.Quarter51:
                    leftUpperPartOffset = 64;
                    leftLowerPartOffset = 66;
                    break;

                case QuarterType.Quarter53:
                    leftUpperPartOffset = 65;
                    leftLowerPartOffset = 67;
                    break;

                case QuarterType.Quarter55:
                    leftUpperPartOffset = 65.5;
                    leftLowerPartOffset = 67;
                    break;

                default:
                    return;
            }

            Vector2 lockLeftBottom = new Vector2(leftBottom.X + leftLowerPartOffset, leftBottom.Y + BottomOffset);
            AddLowerLockPart(entities, lockLeftBottom);

            lockLeftBottom = new Vector2(leftBottom.X + leftUpperPartOffset, leftBottom.Y + BottomOffset + Offset + Height);
            AddUpperLockPart(entities, lockLeftBottom);
        }

        static void AddUpperLockPart(DrawingEntities entities, Vector2 leftBottom)
        {
            const int Height = 50;
            const int Width = 18;

            AddRectangle(entities, leftBottom, new Vector2(leftBottom.X + Width, leftBottom.Y + Height));
        }

        static void AddLowerLockPart(DrawingEntities entities, Vector2 leftBottom)
        {
            const double Height = 50;
            const int Width = 14;
            const int HeightOffset = 15;
            const int Radius = 20;
            const double CircleOffset = 2.68;

            // left down
            entities.Add(new Line(leftBottom, new Vector2(leftBottom.X, leftBottom.Y + HeightOffset)));

            // left up
            entities.Add(new Line(
                new Vector2(leftBottom.X, leftBottom.Y + Height - HeightOffset),
                new Vector2(leftBottom.X, leftBottom.Y + Height)));

            // up
            entities.Add(new Line(
                new Vector2(leftBottom.X, leftBottom.Y + Height),
                new Vector2(leftBottom.X + Width, leftBottom.Y + Height)));

            // right
            entities.Add(new Line(
                new Vector2(leftBottom.X + Width, leftBottom.Y + Height),
                new Vector2(leftBottom.X + Width, leftBottom.Y)));

            entities.Add(new Line(new Vector2(leftBottom.X + Width, leftBottom.Y), leftBottom)); // bottom

            entities.Add(new Arc(new Vector2(leftBottom.X - Radius + CircleOffset, leftBottom.Y + (Height / 2)), Radius, 330, 30));
        }
    }

    private static void AddRectangle(DrawingEntities entities, Vector2 leftBottom, Vector2 rightUp)
    {
        entities.Add(new Line(leftBottom, new Vector2(leftBottom.X, rightUp.Y))); // left
        entities.Add(new Line(new Vector2(leftBottom.X, rightUp.Y), rightUp)); // up
        entities.Add(new Line(rightUp, new Vector2(rightUp.X, leftBottom.Y))); // right
        entities.Add(new Line(new Vector2(rightUp.X, leftBottom.Y), leftBottom)); // bottom
    }
}
