namespace Common.RegionMatchers
{
    public class River : CardsMatcher
    {
        public River(Board board) : base(board)
        {
        }
        
        public override RegionSpec GetRegionSpec()
        {
            var spec = base.GetRegionSpec();
            spec.Num = 1;
            return spec;
        }
    }
}