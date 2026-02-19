using Application.Interfaces.Contracts.Persistance;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class EfTransactionWrapper : ITransactionWrapper
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync() => _transaction.CommitAsync();

        public Task RollbackAsync() => _transaction.RollbackAsync();

        public async ValueTask DisposeAsync() => await _transaction.DisposeAsync();
    }
}
