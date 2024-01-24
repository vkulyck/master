namespace GmWeb.Tests.Logic.Tests;

[Collection("Sequential")]
public abstract class LogicTestBase<TLogicTests> : IDisposable
    where TLogicTests : LogicTestBase<TLogicTests>
{
    public LogicTestBase()
    {
    }

    public void Dispose()
    {
    }
}