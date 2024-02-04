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
            const int JambDistance = 200;
            const int LintelDistance = 100;
            DxfDocument doc = new DxfDocument();

            // left jamb
            AddRectangle(doc.Entities, new Vector2(0, 0), new Vector2(product.JambWidth, product.JambLength));

            Vector2 leftBottom = new Vector2(product.JambWidth + JambDistance, 0);

            // right jamb
            AddRectangle(
                doc.Entities,
                new Vector2(product.JambWidth + JambDistance, 0),
                new Vector2((product.JambWidth * 2) + JambDistance, product.JambLength));

            // lintel
            AddRectangle(
                doc.Entities,
                new Vector2(LintelDistance, LintelDistance + product.JambLength),
                new Vector2(LintelDistance + product.LintelLength, LintelDistance + product.JambLength + product.LintelWidth));

            AddHinges(doc.Entities, leftBottom, product);

            doc.Save(filePath);
        }
    }

    private static void AddHinges(DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
    {
        switch (product.HingeType)
        {
            case HingeType.HingeEB_755:
                AddHingeAmount(AddHingeEB_755, entities, leftBottom, product);
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

        static void AddHingeAmount(Action<DrawingEntities, Vector2> addHinge, DrawingEntities entities, Vector2 leftBottom, ProductInformation product)
        {
            const int BottomOffset = 256;
            const int UpOffset = 313;
            const int DistanceBetweenHinges = 500;

            if (product.HingeAmount == 4)
            {
                addHinge(entities, new Vector2(leftBottom.X, leftBottom.Y + product.JambLength - (UpOffset + DistanceBetweenHinges + DistanceBetweenHinges)));
            }

            if (product.HingeAmount >= 3)
            {
                addHinge(entities, new Vector2(leftBottom.X, leftBottom.Y + product.JambLength - (UpOffset + DistanceBetweenHinges)));
            }

            if (product.HingeAmount >= 2)
            {
                addHinge(entities, new Vector2(leftBottom.X, leftBottom.Y + product.JambLength - UpOffset));
                addHinge(entities, new Vector2(leftBottom.X, leftBottom.Y + BottomOffset));
            }
        }

        static void AddHingeEB_755(DrawingEntities entities, Vector2 leftCenter)
        {
            const double Width = 29.6;
            const double Length = 102.5;
            const double LeftOffset = 33.7;

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + LeftOffset, leftCenter.Y - (Length / 2));

            AddRectangle(entities, hingeLeftBottom, new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Length));
        }

        static void AddHingeR_10_102x76(DrawingEntities entities, Vector2 leftCenter)
        {
            const double Width = 29.6;
            const double Length = 102.5;
            const double LeftOffset = 50;

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + LeftOffset, leftCenter.Y - (Length / 2));

            AddRectangle(entities, hingeLeftBottom, new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Length));
        }

        static void AddHinge4BB_R14(DrawingEntities entities, Vector2 leftCenter)
        {
            const double Width = 28.6;
            const double Length = 100.5;
            const double LeftOffset = 50;

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + LeftOffset, leftCenter.Y - (Length / 2));

            AddRectangle(entities, hingeLeftBottom, new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Length));
        }

        static void AddHingeOTLAV_30x120(DrawingEntities entities, Vector2 leftCenter)
        {
            const double Width = 30.4;
            const double Length = 102.4;
            const double LeftOffset = 57.5;

            Vector2 hingeLeftBottom = new Vector2(leftCenter.X + LeftOffset, leftCenter.Y - (Length / 2));

            AddRectangle(entities, hingeLeftBottom, new Vector2(hingeLeftBottom.X + Width, hingeLeftBottom.Y + Length));
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
