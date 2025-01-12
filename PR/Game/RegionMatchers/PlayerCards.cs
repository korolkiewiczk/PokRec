using Common;

namespace Game.RegionMatchers
{
    public class PlayerCards : CardsMatcher
    {
        public PlayerCards(Board board) : base(board)
        {
        }
        
        public override RegionSpec GetRegionSpec()
        {
            var spec = base.GetRegionSpec();
            spec.Num = 2;
            return spec;
        }
    }
}