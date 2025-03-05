namespace Game.Common.Model;

public record EvResult(
    decimal Pot,
    decimal MyInvestment,
    double Equity,
    decimal ExpectedWinnings, 
    decimal NetEv
)
{
    public override string ToString()
    {
        return $"P: {Pot}, MI: {MyInvestment}, " +
               $"EQ: {Equity:P2}, EXP: {ExpectedWinnings}, EV: {NetEv}";
    }
}