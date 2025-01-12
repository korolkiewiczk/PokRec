using Common;
using Game.Interfaces;
using Game.Presenters;

namespace Game.MultiRegionMatchers
{
    public class Position : MultiPosMatcher
    {
        public Position(Board board, int seats) : base(board, seats)
        {
        }

        public override IResultPresenter GetPresenter()
        {
            return new PositionPresenter();
        }

        protected override string GetClassPathName() => "position";
        protected override string GetMatcherName() => nameof(Position);
        protected override int GetThreshold() => 80;
    }
}