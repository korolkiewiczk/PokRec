using System.IO;

namespace Common
{
    public static class Classes
    {
        public static string ClassPath(Board board, string className)
        {
            return Path.Combine($"{board.Rect.Width}X{board.Rect.Height}", "classes", className);
        }
    }
}