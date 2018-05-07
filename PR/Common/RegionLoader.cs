using System.Drawing;
using System.IO;

namespace Common
{
    public class RegionLoader
    {
        public static string GetRegionPath(Board board)
        {
            return Path.Combine($"{board.Rect.Width}X{board.Rect.Height}", "regions");
        }
        
        public static Rectangle LoadRegion(Board board, string name)
        {
            string path = Path.Combine($"{board.Rect.Width}X{board.Rect.Height}", "regions",
                name + ".txt");
            var converted = new RectangleConverter().ConvertFromString(File.ReadAllText(path));
            return (Rectangle)converted;
        }
    }
}
