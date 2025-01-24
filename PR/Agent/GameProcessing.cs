using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    private readonly string _id;
    private readonly Rectangle _rect;
    private readonly IList<RegionSpec> _regionSpecs;
    private readonly ConcurrentDictionary<string, ReconResult> _state = new();

    // Add event for processing completion
    public event EventHandler ProcessingCompleted;

    public GameProcessing(string id, Rectangle rect, IList<RegionSpec> regionSpecs)
    {
        _id = id;
        _regionSpecs = regionSpecs;
        _rect = rect;
    }

    public IDictionary<string, ReconResult> State => _state;

    protected override async Task Work(CancellationToken cancellationToken)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var mainImg = await GetMainImgFromScreen(cancellationToken);

        try
        {
            await RegionProcessor.ProcessRegions(_id, _regionSpecs, _state, mainImg, cancellationToken);
            ProcessingCompleted?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            mainImg.Dispose();
        }
        sw.Stop();
        Log.Info($"Processing done in {sw.ElapsedMilliseconds} ms");
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