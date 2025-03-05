using Common;

namespace Game.Common.MultiRegionMatchers
{
    public class Opponent : MultiPosMatcher
    {
        public Opponent(Board board, int seats) : base(board, seats)
        {
        }

        protected override string GetClassPathName() => "opponent";
        protected override string GetMatcherName() => nameof(Opponent);
        protected override int GetThreshold() => 70;
    }
}