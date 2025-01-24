using System.Drawing;
using Common;
using Game.Tests.Helpers;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Xunit.Abstractions;

namespace Game.Tests;

public class PokerTestsBase
{
    protected ITestOutputHelper _testOutputHelper;

    public PokerTestsBase(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        // Configure log4net to use our custom appender
        var hierarchy = (Hierarchy) LogManager.GetRepository();
        hierarchy.Root.RemoveAllAppenders(); // Clear existing appenders

        var appender = new TestOutputHelperAppender(testOutputHelper);
        appender.ActivateOptions();

        hierarchy.Root.AddAppender(appender);
        hierarchy.Root.Level = Level.All;
        hierarchy.Configured = true;
    }

    protected ReconResult I(params string[] values)
    {
        return new ReconResult(
            new Rectangle(0, 0, 0, 0),
            values.ToList()
        );
    }
}