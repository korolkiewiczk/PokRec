using Common;

namespace Game.Common.Model;

public record ReconResults(
    ReconResult PlayerResult,
    ReconResult FlopResult,
    ReconResult TurnResult,
    ReconResult RiverResult,
    List<ReconResult> PositionResults,
    List<ReconResult> OpponentResults,
    List<ReconResult> StackResults,
    List<ReconResult> NicknameResults,
    ReconResult DecisionResult,
    ReconResult PotResult);