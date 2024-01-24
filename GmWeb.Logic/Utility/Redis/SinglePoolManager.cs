using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Models;
using System;

namespace GmWeb.Logic.Utility.Redis
{
    internal class SinglePool : IRedisCacheConnectionPoolManager
    {
        protected IConnectionMultiplexer Connection { get; private set; }

        public SinglePool(string connectionString)
        {
            this.Connection = ConnectionMultiplexer.Connect(connectionString);
        }

        public IConnectionMultiplexer GetConnection() => this.Connection;

        public void Dispose()
        {
            if (this.Connection == null)
                return;
            this.Connection.Dispose();
        }

        public ConnectionPoolInformation GetConnectionInformations() => throw new NotImplementedException();
    }
}
