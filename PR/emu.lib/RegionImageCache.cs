using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace emu.lib;

public class RegionImageCache
{
    private readonly ConcurrentDictionary<string, byte[]> _previousHashes = new();
    private readonly Random _random = new();
    private const int ForceUpdateProbability = 100;

    public bool ShouldProcess(string regionName, Image<Rgba32> image)
    {
        if (_random.Next(ForceUpdateProbability) == 0)
        {
            _previousHashes[regionName] = CalculateImageHash(image);
            return true;
        }

        var currentHash = CalculateImageHash(image);

        if (!_previousHashes.TryGetValue(regionName, out var oldHash))
        {
            _previousHashes[regionName] = currentHash;
            return true;
        }

        bool changed = !oldHash.SequenceEqual(currentHash);
        if (changed)
        {
            _previousHashes[regionName] = currentHash;
        }

        return changed;
    }

    private static byte[] CalculateImageHash(Image<Rgba32> image)
    {
        using var md5 = MD5.Create();

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                
                var rowBytes = MemoryMarshal.AsBytes(row);

                md5.TransformBlock(rowBytes.ToArray(), 0, rowBytes.Length, null, 0);
            }
        });

        md5.TransformFinalBlock([], 0, 0);
        return md5.Hash ?? [];
    }
}