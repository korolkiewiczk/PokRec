using System.Collections.Generic;
using Common;

namespace Game.Games.TexasHoldem.Model;

public record ReconResults(
    ReconResult PlayerResult,
    ReconResult FlopResult,
    ReconResult TurnResult,
    ReconResult RiverResult,
    List<ReconResult> PositionResults,
    List<ReconResult> OpponentResults,
    List<ReconResult> StackResults);