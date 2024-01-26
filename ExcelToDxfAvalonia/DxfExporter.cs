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
            const int Distance = 100;
            DxfDocument doc = new DxfDocument();

            int width = int.Parse(product.ExternalWidth);
            int length = int.Parse(product.Length);

            AddRectangle(doc.Entities, new Vector2(0, 0), new Vector2(width, length));
            AddRectangle(doc.Entities, new Vector2(width + Distance, 0), new Vector2((width * 2) + Distance, length));

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
