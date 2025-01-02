using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common;
using emu.lib;
using scr;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace Agent;

public class GameProcessing : BackgroundProcessing
{
    private readonly Rectangle _rect;
    private readonly IList<RegionSpec> _regionSpecs;
    private readonly ConcurrentDictionary<string, ReconResult> _state = new();

    public GameProcessing(Rectangle rect, IList<RegionSpec> regionSpecs)
    {
        _regionSpecs = regionSpecs;
        _rect = rect;
    }

    public IDictionary<string, ReconResult> State => _state;

    protected override async Task Work(CancellationToken cancellationToken)
    {
        var mainImg = await GetMainImgFromScreen(cancellationToken);

        try
        {
            await RegionProcessor.ProcessRegions(_regionSpecs, _state, mainImg, cancellationToken);
        }
        finally
        {
            mainImg.Dispose();
        }
    }

    private async Task<Image<Rgba32>> GetMainImgFromScreen(CancellationToken cancellationToken)
    {
        using var bmp = ScreenShot.Capture(_rect);
        using var memoryStream = new MemoryStream();
        bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var mainImg = await Image.LoadAsync<Rgba32>(memoryStream, cancellationToken);
        return mainImg;
    }
}