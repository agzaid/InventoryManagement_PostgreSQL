using Application.Interface.Contract.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IInvTransRepository : IGenericRepository<InvTrans>
    {
        Task DeleteRangeAsync(Expression<Func<InvTrans, bool>> predicate);
        Task<InvTrans?> GetByTrNumAsync(int code);

    }
}
