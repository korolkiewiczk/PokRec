using System.Drawing;

namespace Common
{
    public class RegionSpec
    {
        public string Name { get; set; }

        public string ClassesPath { get; set; }

        public int Num { get; set; }

        public int Threshold { get; set; }

        public bool IsOcr { get; set; }
        
        public Rectangle? Rectangle { get; set; }
    }
}