using System;
using System.Drawing;
using System.IO;

namespace Common
{
    class RegionLoader
    {
        public static string GetRegionPath(string basePath, Board board)
        {
            return Path.Combine(basePath, $"{board.Rect.Width}X{board.Rect.Height}", "regions");
        }
        
        public static Rectangle LoadRegion(string basePath, Board board, string name)
        {
            string path = Path.Combine(basePath, $"{board.Rect.Width}X{board.Rect.Height}", "regions",
                name + ".txt");
            var converted = new RectangleConverter().ConvertFromString(File.ReadAllText(path));
            return (Rectangle)converted;
        }
    }
}
