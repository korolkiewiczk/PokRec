using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MarkItDown;

public class RegionsMarker
{
    private readonly List<string> _regions;
    private bool _showRegions;
    private Dictionary<string, Rectangle> _parsedRegions;

    public RegionsMarker()
    {
        _regions = LoadRegionsDescriptions();
    }

    public List<string> Regions => _regions;

    public void MarkRegionsOnScreen(string regionsDir)
    {
        _showRegions = !_showRegions;

        if (_showRegions && _parsedRegions == null)
        {
            _parsedRegions = new Dictionary<string, Rectangle>();
            foreach (var region in _regions)
            {
                var rect = LoadRegion(region, regionsDir);
                if (rect != Rectangle.Empty)
                {
                    _parsedRegions.Add(region, rect);
                }
            }
        }
    }

    private static List<string> LoadRegionsDescriptions()
    {
        return File.ReadAllLines(Common.Paths.Regions).ToList();
    }

    private Rectangle LoadRegion(string tag, string regionsDir)
    {
        try
        {
            var rectStr = File.ReadAllText(regionsDir + tag + ".txt");
            var rect = (Rectangle)new RectangleConverter().ConvertFromString(rectStr)!;
            return rect;
        }
        catch
        {
            return Rectangle.Empty;
        }
    }

    public void PaintRegions(PaintEventArgs e)
    {
        if (!_showRegions || _parsedRegions == null) return;
            
        var color = Color.Magenta;
        using var brush = new SolidBrush(color);
        using var pen = new Pen(brush, 3f);
        foreach (var rect in _parsedRegions)
        {
            e.Graphics.DrawRectangle(pen, rect.Value);
            using var font = new Font("Arial", 6, FontStyle.Regular, GraphicsUnit.Point);
            e.Graphics.DrawString(rect.Key, font, brush, rect.Value.Location);
        }
    }
}