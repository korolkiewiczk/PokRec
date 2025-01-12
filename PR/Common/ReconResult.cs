using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Common;

public record ReconResult(Rectangle ItemRectangle, List<string> Results)
{
    public string Result => Results?.FirstOrDefault();
}