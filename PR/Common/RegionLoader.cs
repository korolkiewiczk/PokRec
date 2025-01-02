using System.Drawing;
using System.IO;

namespace Common
{
    public static class RegionLoader
    {
        public static string GetRegionPath(Project project, Board board)
        {
            return Path.Combine($"{project.Path}\\{project.Name}\\regions\\{board.Id}");
        }
        
        public static Rectangle LoadRegion(Project project, Board board, string name)
        {
            string path = Path.Combine(GetRegionPath(project, board), name + ".txt");
            var converted = new RectangleConverter().ConvertFromString(File.ReadAllText(path));
            return (Rectangle)converted!;
        }
    }
}
