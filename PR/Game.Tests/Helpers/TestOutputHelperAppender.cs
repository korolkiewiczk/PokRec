using log4net.Appender;
using log4net.Core;
using Xunit.Abstractions;

namespace Game.Tests.Helpers;

public class TestOutputHelperAppender : AppenderSkeleton
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TestOutputHelperAppender(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    protected override void Append(LoggingEvent loggingEvent)
    {
        _testOutputHelper.WriteLine($"[{loggingEvent.Level}] {loggingEvent.RenderedMessage}");
    }
} 