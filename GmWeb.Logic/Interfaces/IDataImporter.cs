using System;
using System.Threading.Tasks;

namespace GmWeb.Logic.Interfaces
{
    public interface IDataImporter : IDisposable
    {
        Task ImportAsync();
    }
}
