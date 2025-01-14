﻿using System.Collections.Concurrent;
using Common;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Formatting = System.Xml.Formatting;

namespace emu.lib;

public static class RegionProcessor
{
    private static readonly log4net.ILog Log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private static readonly RegionImageCache ImageCache = new();

    public static async Task ProcessRegions(string id, IEnumerable<RegionSpec> regionSpecs,
        ConcurrentDictionary<string, ReconResult> state, Image<Rgba32> mainImg, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(regionSpecs, cancellationToken, (regionSpec, _) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return default;
            }

            if (regionSpec.Rectangle is null) return default;

            using var regionImage = CvUtils.GenerateRegionImage(regionSpec.Rectangle.Value, mainImg);

            // Skip processing if image hasn't changed and we're not forcing an update
            if (!ImageCache.ShouldProcess(id + regionSpec.Name, regionImage))
            {
                return default;
            }

            BaseRegionMatcher regionMatcher = regionSpec.IsOcr switch
            {
                true => new OcrRegionMatcher(regionImage),
                _ => new RegionMatcher(regionImage, regionSpec.ClassesPath, regionSpec.Num,
                    regionSpec.Threshold)
            };

            var reconResult = new ReconResult(
                regionSpec.Rectangle.Value,
                regionMatcher.Process()
            );

            Log.Info($"Processed region {regionSpec.Name} with values [{string.Join(',',reconResult.Results)}]");
            state[regionSpec.Name] = reconResult;
            return default;
        });
    }
}