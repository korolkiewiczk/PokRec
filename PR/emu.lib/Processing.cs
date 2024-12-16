using Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace emu.lib;

public class Processing
{
    private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
    
    static Processing()
    {
        var logRepository = log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
        log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo("emulog.config"));
    }

    public static void Process(string specFilePath)
    {
        try
        {
            var spec = ImgReconSpec.Load(specFilePath);

            if (spec == null) return;

            using var mainImg = Image.Load(spec.ImgPath);

            ImgReconOutput output = new ImgReconOutput();

            output.SpecResults = new List<ImgReconOutput.Recon>();

            ImgReconOutput prevOutput =
                spec.LastOutputPath != null ? ImgReconOutput.Load(spec.LastOutputPath) : null;

            foreach (var regionSpec in spec.RegionSpecs)
            {
                log.Info(regionSpec.Name);
                var regionMatcher = new RegionMatcher(spec.RegionPath, regionSpec.Name,
                    mainImg.CloneAs<Rgba32>(),
                    regionSpec.ClassesPath, regionSpec.Num, regionSpec.Threshold,
                    prevOutput?.SpecResults.FirstOrDefault(x => x.Name == regionSpec.Name)?.Values,
                    regionSpec.IsOcr);

                output.SpecResults.Add(new ImgReconOutput.Recon
                {
                    Name = regionSpec.Name,
                    Values = regionMatcher.Process()
                });
            }

            // var logicalCoreCount = Environment.ProcessorCount;
            //
            // Parallel.ForEach(spec.RegionSpecs, new ParallelOptions { MaxDegreeOfParallelism = logicalCoreCount }, regionSpec =>
            // {
            //     log.Info(regionSpec.Name);
            //
            //     // Clone the image for thread safety
            //     var threadSafeMainImg = mainImg.CloneAs<Rgba32>();
            //
            //     var regionMatcher = new RegionMatcher(spec.RegionPath, regionSpec.Name,
            //         threadSafeMainImg,
            //         regionSpec.ClassesPath, regionSpec.Num, regionSpec.Threshold,
            //         prevOutput != null
            //             ? prevOutput.SpecResults.FirstOrDefault(x => x.Name == regionSpec.Name)?.Values
            //             : null,
            //         regionSpec.IsOcr);
            //
            //     var recon = new ImgReconOutput.Recon
            //     {
            //         Name = regionSpec.Name,
            //         Values = regionMatcher.Process()
            //     };
            //
            //     lock (output.SpecResults) // Ensure thread safety for shared collection
            //     {
            //         output.SpecResults.Add(recon);
            //     }
            // });


            output.Save(spec.OutFilePath);
        }
        catch (Exception ex)
        {
            log.Error(ex);
        }
    }
}