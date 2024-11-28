namespace emu.lib
{
    public class SpecReader
    {
        public class RegionSpec
        {
            public string Name { get; set; }

            public string ClassesPath { get; set; }

            public int Num { get; set; }

            public int Threshold { get; set; }

            public RegionSpec(string name, string classesPath, int num, int threshold)
            {
                Name = name;
                ClassesPath = classesPath;
                Num = num;
                Threshold = threshold;
            }
        }

        public string ImgPath { get; private set; }
        public string RegionPath { get; private set; }
        public List<RegionSpec> RegionSpecs { get; private set; }
        public string OutFilePath { get; private set; }

        public void Read(string path)
        {
            var lines = File.ReadAllLines(path);
            ImgPath = lines[0];
            RegionPath = lines[1];
            var regionCount = int.Parse(lines[2]);
            RegionSpecs = new List<RegionSpec>();
            for (int i = 0; i < regionCount; i++)
            {
                var regionSpec = lines[i + 3].Split(' ');
                RegionSpecs.Add(new RegionSpec(regionSpec[0],regionSpec[1],int.Parse(regionSpec[2]),int.Parse(regionSpec[3])));
            }

            OutFilePath = lines[3 + regionCount];
        }
    }
}