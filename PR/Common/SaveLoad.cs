using System.IO;

namespace Common
{
    public static class SaveLoad
    {
        public const string ProjExtension = ".proj";
        private const string BoardsDir = "boards";

        public static void SaveProject(Project prj)
        {
            CreateDirIfNotExists(prj.Path);
            string path = Path.Combine(prj.Path, prj.Name + ProjExtension);
            prj.Save(path);
        }

        public static Project LoadProject(string filePath)
        {
            if (!Path.HasExtension(filePath)) filePath += ProjExtension;
            return Project.Load(filePath);
        }

        public static string GetBoardPath(Project prj, Board board)
        {
            string path = Path.Combine(prj.Path, prj.Name, BoardsDir, board.Id);
            CreateDirIfNotExists(path);
            return Path.Combine(path, "board.png");
        }

        private static void CreateDirIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
