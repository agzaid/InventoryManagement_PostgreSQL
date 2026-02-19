using Application.Interface.Contract.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IItemCategoryRepository : IGenericRepository<ItemCategory>
    {
        Task<ItemCategory?> GetByNameAsync(string name);
        Task<ItemCategory?> GetByIdStringAsync(string code);
    }
}
