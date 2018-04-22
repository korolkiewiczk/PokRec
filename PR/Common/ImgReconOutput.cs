using System.Collections.Generic;

namespace Common
{
    public class ImgReconOutput : JsonObject<ImgReconOutput>
    {
        public class Recon
        {
            public List<string> Values { get; set; }
        }
        
        public List<Recon> SpecResults { get; set; }
    }
}