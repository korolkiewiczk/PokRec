using System.Collections.Generic;

namespace Common
{
    public class ImgReconSpec : JsonObject<ImgReconSpec>
    {
        private const string SpecDir = "specs";

        public string ImgPath { get; set; }
        public string RegionPath { get; set; }
        public List<RegionSpec> RegionSpecs { get; set; }
        public string OutFilePath { get; set; }
        public string LastOutputPath { get; set; }

        public static ImgReconSpec CreateImgReconSpec(Project project, Board board, string outFilePath,
            string prevOutputFilePath)
        {
            return new ImgReconSpec
            {
                //ImgPath = SaveLoad.GetBoardPath(project, board), //todo iter
                ImgPath = SaveLoad.GetBoardPathIter(project, board),
                RegionPath = RegionLoader.GetRegionPath(project, board),
                OutFilePath = outFilePath,
                RegionSpecs = new List<RegionSpec>(),
                LastOutputPath = prevOutputFilePath
            };
        }

        public static string SpecDirectory(Project prj, Board board)
        {
            return $"{prj.Path}\\{prj.Name}\\{SpecDir}\\{board.Id}";
        }

        public static string SpecFileName(Project prj, Board board)
        {
            return $"{SpecDirectory(prj, board)}\\spec{board.Computed}.json";
        }
    }
}