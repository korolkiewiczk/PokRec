namespace Common.RegionMatchers
{
    public class Flop : CardsMatcher
    {
        public Flop(Board board) : base(board)
        {
        }

        public override RegionSpec GetRegionSpec()
        {
            var spec = base.GetRegionSpec();
            spec.Num = 3;
            return spec;
        }
    }
}