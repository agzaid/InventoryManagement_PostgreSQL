using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface ITransactionWrapper : IAsyncDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }
}
