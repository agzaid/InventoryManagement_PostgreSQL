using Application.Interface.Contract.Persistance;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
    }
}
