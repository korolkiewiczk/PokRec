using Common;
using Game.Interfaces;
using Game.Presenters;

namespace Game.MultiRegionMatchers
{
    public class Opponent : MultiPosMatcher
    {
        public Opponent(Board board, int seats) : base(board, seats)
        {
        }

        public override IResultPresenter GetPresenter()
        {
            return new OpponentPresenter();
        }

        protected override string GetClassPathName() => "opponent";
        protected override string GetMatcherName() => nameof(Opponent);
        protected override int GetThreshold() => 70;
    }
}