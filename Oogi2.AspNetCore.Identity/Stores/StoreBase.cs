using Oogi2;
using System;

namespace Oogi2.AspNetCore.Identity.Stores
{
    public abstract class StoreBase
    {
        protected bool disposed;

        protected IConnection _connection;

        protected StoreBase(IConnection connection)
        {
            _connection = connection;
        }

        protected virtual void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}