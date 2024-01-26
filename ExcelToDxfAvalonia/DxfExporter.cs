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

            AddRectangle(doc.Entities, new Vector2(0, 0), new Vector2(product.JambWidth, product.JambLength));
            AddRectangle(
                doc.Entities,
                new Vector2(product.JambWidth + JambDistance, 0),
                new Vector2((product.JambWidth * 2) + JambDistance, product.JambLength));

            AddRectangle(
                doc.Entities,
                new Vector2(LintelDistance, LintelDistance + product.JambLength),
                new Vector2(LintelDistance + product.LintelLength, LintelDistance + product.JambLength + product.LintelWidth));

            doc.Save(filePath);
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
