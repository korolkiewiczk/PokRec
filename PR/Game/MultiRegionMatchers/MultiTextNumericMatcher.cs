namespace Game.MultiRegionMatchers;

public abstract class MultiTextNumericMatcher : MultiTextMatcher<decimal?>
{
    public MultiTextNumericMatcher(int seats) : base(seats)
    {
        IsNumericOnly = true;
    }
}