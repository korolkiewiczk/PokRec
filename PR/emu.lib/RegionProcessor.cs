using System.Collections.Concurrent;
using Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace emu.lib;

public static class RegionProcessor
{
    public static async Task ProcessRegions(IEnumerable<RegionSpec> regionSpecs,
        ConcurrentDictionary<string, ReconResult> state, Image<Rgba32> mainImg, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(regionSpecs, cancellationToken, (regionSpec, _) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask(Task.CompletedTask);
            }

            using var regionImage = CvUtils.GenerateRegionImage(regionSpec.Rectangle, mainImg);
            BaseRegionMatcher regionMatcher = regionSpec.IsOcr switch
            {
                true => new OcrRegionMatcher(regionImage),
                _ => new RegionMatcher(regionImage, regionSpec.ClassesPath, regionSpec.Num,
                    regionSpec.Threshold)
            };

            var reconResult = new ReconResult(
                regionSpec.Rectangle,
                regionMatcher.Process()
            );

            state[regionSpec.Name] = reconResult;
            return default;
        });
    }
}