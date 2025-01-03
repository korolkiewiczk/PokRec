using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib;

public class OcrRegionMatcher : BaseRegionMatcher
{
    private readonly bool _isNumericOnly;

    public OcrRegionMatcher(Image<Rgba32> regionImage, bool isNumericOnly) : base(regionImage)
    {
        _isNumericOnly = isNumericOnly;
    }

    protected override List<string> ReconJob(Image<Rgba32> regionImage)
    {
        var recognizedText = CvUtilsOcr.GetTextFromRegion(regionImage, _isNumericOnly);
        Log.Info(
            $"Recognized text: {recognizedText}");
        return !string.IsNullOrWhiteSpace(recognizedText)
            ? [recognizedText]
            : [];
    }
}