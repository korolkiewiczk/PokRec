using System;
using Newtonsoft.Json;

namespace Common
{
    public class RegionSpec
    {
        public string Name { get; set; }

        public string ClassesPath { get; set; }

        public int Num { get; set; }

        public int Threshold { get; set; }
        
        [JsonIgnore]
        public Func<IResultPresenter> GetPresenter { get; set; } 
    }
}