using System;

namespace GmWeb.Logic.Interfaces
{
    public interface IConditionallyDisposable : IDisposable
    {
        bool EnableDispose { get; }
    }
}
