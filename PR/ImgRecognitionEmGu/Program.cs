using System.Collections.Generic;
using System.Drawing;
using Common;

namespace ImgRecognitionEmGu
{
    class Program
    {
        static void Main(params string[] args)
        {
            var specFilePath = args[0];

            var spec = ImgReconSpec.Load(specFilePath);

            var mainImg = Bitmap.FromFile(spec.ImgPath);

            ImgReconOutput output = new ImgReconOutput();

            output.SpecResults = new List<ImgReconOutput.Recon>();

            foreach (var regionSpec in spec.RegionSpecs)
            {
                var regionMatcher = new RegionMatcher(spec.RegionPath, regionSpec.Name,
                    mainImg,
                    regionSpec.ClassesPath, regionSpec.Num, regionSpec.Threshold);

                output.SpecResults.Add(new ImgReconOutput.Recon() {Values = regionMatcher.Process()});
            }

            output.Save(spec.OutFilePath);
        }
    }
}