using Game.Interfaces;
using Game.Presenters;

namespace Game.MultiRegionMatchers;

public class PlayerBet : Money
{
    public PlayerBet(int seats) : base(seats)
    {
    }

    public override IResultPresenter GetPresenter()
    {
        return new PlayerBetPresenter();
    }
} 