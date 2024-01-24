using System;

namespace GmWeb.Logic.Interfaces
{
    public interface IDisposableMapper : AutoMapper.IMapper, IDisposable
    {
    }
}