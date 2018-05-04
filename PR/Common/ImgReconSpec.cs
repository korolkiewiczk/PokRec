using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Common
{
    public class ImgReconSpec : JsonObject<ImgReconSpec>
    {
        private const string SpecDir = "specs";

        public string ImgPath { get; set; }
        public string RegionPath { get; set; }
        public List<RegionSpec> RegionSpecs { get; set; }
        public string OutFilePath { get; set; }

        public static ImgReconSpec CreateImgReconSpec(Project project, Board board, string outFilePath)
        {
            return new ImgReconSpec
            {
                ImgPath = SaveLoad.GetBoardPath(project, board), //todo iter
                //ImgPath = SaveLoad.GetBoardPathIter(project, board),
                RegionPath = RegionLoader.GetRegionPath(board),
                OutFilePath = outFilePath,
                RegionSpecs = new List<RegionSpec>()
            };
        }

        public static string SpecDirectory(Project prj, Board board)
        {
            return $"{prj.Path}\\{prj.Name}\\{SpecDir}\\{board.Id}";
        }

        public static string SpecFileName(Project prj, Board board)
        {
            return $"{SpecDirectory(prj, board)}\\spec{board.Generated}.txt";
        }
    }
}