using Application.Interface.Contract.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        Task<Supplier?> GetByNameAsync(string name);
        Task<Supplier?> GetByIdStringAsync(string code);
        //Task<IReadOnlyList<Item>> GetAllItemsWithCategory();
    }
}
