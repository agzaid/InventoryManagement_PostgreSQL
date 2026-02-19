using Application.Interface.Contract.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IItemRepository : IGenericRepository<Item>
    {
        Task<Item?> GetByNameAsync(string name);
        Task<Item?> GetByIdStringAsync(string code);
        Task<IReadOnlyList<Item>> GetAllItemsWithCategory();
    }
}
