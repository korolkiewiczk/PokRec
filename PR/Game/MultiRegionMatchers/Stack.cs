using Game.Interfaces;
using Game.Presenters;

namespace Game.MultiRegionMatchers;

public class Stack : Money
{
    public Stack(int seats) : base(seats)
    {
    }

    public override IResultPresenter GetPresenter()
    {
        return new StackPresenter();
    }
}