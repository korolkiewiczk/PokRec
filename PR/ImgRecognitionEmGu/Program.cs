using System;
using System.Collections.Generic;
using System.Drawing;
using Common;

namespace ImgRecognitionEmGu
{
    class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(params string[] args)
        {
            try
            {
                var specFilePath = args[0];

                var spec = ImgReconSpec.Load(specFilePath);

                var mainImg = Bitmap.FromFile(spec.ImgPath);

                ImgReconOutput output = new ImgReconOutput();

                output.SpecResults = new List<ImgReconOutput.Recon>();

                foreach (var regionSpec in spec.RegionSpecs)
                {
                    log.Info(regionSpec.Name);
                    var regionMatcher = new RegionMatcher(spec.RegionPath, regionSpec.Name,
                        mainImg,
                        regionSpec.ClassesPath, regionSpec.Num, regionSpec.Threshold);

                    output.SpecResults.Add(new ImgReconOutput.Recon
                    {
                        Name = regionSpec.Name,
                        Values = regionMatcher.Process()
                    });
                }

                output.Save(spec.OutFilePath);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}