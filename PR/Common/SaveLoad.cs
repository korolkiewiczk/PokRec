using System.IO;
using Newtonsoft.Json;

namespace Common
{
    public class SaveLoad
    {
        public const string ProjExtension = ".proj";
        public const string BoardsDir = "boards";

        public static void SaveProject(Project prj)
        {
            CreateDirIfNotExists(prj.Path);
            string path = Path.Combine(prj.Path, prj.Name + ProjExtension);
            File.WriteAllText(path, JsonConvert.SerializeObject(prj));
        }

        public static Project LoadProject(string filePath)
        {
            if (!Path.HasExtension(filePath)) filePath += ProjExtension;
            return JsonConvert.DeserializeObject<Project>(File.ReadAllText(filePath));
        }

        public static string GetBoardPath(Project prj, Board board)
        {
            string path = Path.Combine(prj.Path, prj.Name, BoardsDir, board.Id);
            CreateDirIfNotExists(path);
            return Path.Combine(path, "board.png");
        }

        public static string GetBoardPathIter(Project prj, Board board)
        {
            string path = Path.Combine(prj.Path, prj.Name, BoardsDir, board.Id);
            CreateDirIfNotExists(path);
            return Path.Combine(path, $"board{board.Generated}.png");
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
