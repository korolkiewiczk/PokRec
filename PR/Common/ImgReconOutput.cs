using System.Collections.Generic;

namespace Common
{
    public class ImgReconOutput : JsonObject<ImgReconOutput>
    {
        public class Recon
        {
            public string Name { get; set; }
            public List<string> Values { get; set; }
        }

        public List<Recon> SpecResults { get; set; }

        public static string OutFilePath(Project prj, Board board)
        {
            return $"{ImgReconSpec.SpecDirectory(prj, board)}\\out{board.Generated}.txt";
        }
    }
}