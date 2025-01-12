using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib;

public class OcrRegionMatcher : BaseRegionMatcher
{
    public OcrRegionMatcher(Image<Rgba32> regionImage) : base(regionImage)
    {
    }

    protected override List<string> ReconJob(Image<Rgba32> regionImage)
    {
        var recognizedText = CvUtilsOcr.GetTextFromRegion(regionImage);
        Log.Info(
            $"Recognized text: {recognizedText}");
        return !string.IsNullOrWhiteSpace(recognizedText)
            ? [recognizedText]
            : [];
    }
}