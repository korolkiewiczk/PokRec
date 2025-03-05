using Common;

namespace Game.Common.MultiRegionMatchers
{
    public class Position : MultiPosMatcher
    {
        public Position(Board board, int seats) : base(board, seats)
        {
        }

        protected override string GetClassPathName() => "position";
        protected override string GetMatcherName() => nameof(Position);
        protected override int GetThreshold() => 80;
    }
}