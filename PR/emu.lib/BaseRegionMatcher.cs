using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib;

public abstract class BaseRegionMatcher
{
    private static readonly log4net.ILog Log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private readonly Image<Rgba32> _regionImage;

    public BaseRegionMatcher(Image<Rgba32> regionImage)
    {
        _regionImage = regionImage;
    }

    public List<string> Process()
    {
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = ReconJob(_regionImage);
            sw.Stop();
            Log.Info($"{GetType().Name} in {sw.ElapsedMilliseconds} ms");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return [];
        }
    }

    protected abstract List<string> ReconJob(Image<Rgba32> regionImage);
}