using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Common
{
    public class ImgReconSpec : JsonObject<ImgReconSpec>
    {
        public class RegionSpec
        {
            public string Name { get; set; }

            public string ClassesPath { get; set; }

            public int Num { get; set; }

            public int Threshold { get; set; }
        }

        public string ImgPath { get; set; }
        public string RegionPath { get; set; }
        public List<RegionSpec> RegionSpecs { get; set; }
        public string OutFilePath { get; set; }
    }
}