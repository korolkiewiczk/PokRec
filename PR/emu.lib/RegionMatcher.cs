using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib;

public class RegionMatcher : BaseRegionMatcher
{
    private static readonly log4net.ILog Log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private readonly List<WeightedImages> _imgList = new();

    private readonly string _classPath;
    private readonly int _take;
    private readonly int _threshold;

    public RegionMatcher(Image<Rgba32> regionImage, string classPath, int take,
        int threshold) : base(regionImage)
    {
        _classPath = classPath;
        _take = take;
        _threshold = threshold;
    }

    protected override List<string> ReconJob(Image<Rgba32> regionImage)
    {
        ProcessFolder(regionImage);

        var result = _imgList.OrderByDescending(x => x.Score)
            .Where(x => x.Score > _threshold)
            .Take(_take)
            .Select(x => Path.GetFileNameWithoutExtension(x.ImagePath))
            .ToList();

        Log.Info(
            $"{string.Join(";", _imgList.Select(x => $"{Path.GetFileNameWithoutExtension(x.ImagePath)} {x.Score}"))}");

        
        return result.Count != 0 ? result : [];
    }

    private void ProcessFolder(Image<Rgba32> regionImage)
    {
        _imgList.Clear();
        using Image<Gray, byte> regionImageCv = CvUtils.ConvertToEmguCvGray(regionImage);
        foreach (var classImage in Directory.GetFiles(_classPath))
        {
            if (!ProcessImage(regionImageCv, classImage)) break;
        }
    }

    private bool ProcessImage(Image<Gray, byte> regionImageCv, string classImagePath)
    {
        // Load class image using Emgu CV
        using var classImageFull = new Image<Gray, byte>(classImagePath);

        var numExtraColumns = (int) (classImageFull.Width * 0.1);

        if (numExtraColumns >= classImageFull.Width)
        {
            return true;
        }

        var cropRect = new Rectangle(numExtraColumns, 0, classImageFull.Width - numExtraColumns,
            classImageFull.Height);

        using var classImage = classImageFull.Copy(cropRect);

        if (classImage.Width > regionImageCv.Width || classImage.Height > regionImageCv.Height)
            return true;

        var result = regionImageCv.MatchTemplate(classImage, TemplateMatchingType.CcoeffNormed);

        result.MinMax(out _, out var maxValues, out _, out _);

        var similarity = maxValues[0];

        if (similarity * 100 >= _threshold)
        {
            _imgList.Add(new WeightedImages(ImagePath: classImagePath, Score: similarity * 100));
        }

        return true;
    }
}