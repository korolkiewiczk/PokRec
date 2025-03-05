using Common;

namespace Game.Common.MultiRegionMatchers;

public class Nickname : MultiTextMatcher<string>
{
    public Nickname(int seats) : base(seats)
    {
    }

    public override List<string> Match(IEnumerable<ReconResult> results)
    {
        return results.Select(result => result.Results.FirstOrDefault() ?? string.Empty).ToList();
    }
} 