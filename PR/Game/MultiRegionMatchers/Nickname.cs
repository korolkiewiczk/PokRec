using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Interfaces;
using Game.Presenters;

namespace Game.MultiRegionMatchers;

public class Nickname : MultiTextMatcher<string>
{
    public Nickname(int seats) : base(seats)
    {
    }

    public override IResultPresenter GetPresenter()
    {
        return new NicknamePresenter();
    }

    public override List<string> Match(IEnumerable<ReconResult> results)
    {
        return results.Select(result => result.Results.FirstOrDefault() ?? string.Empty).ToList();
    }
} 