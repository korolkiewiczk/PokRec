namespace Game.MultiRegionMatchers;

public abstract class MultiTextNumericMatcher : MultiTextMatcher<decimal?>
{
    protected MultiTextNumericMatcher(int seats) : base(seats)
    {
    }
}