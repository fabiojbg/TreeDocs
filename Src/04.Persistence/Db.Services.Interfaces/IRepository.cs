using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity: class
    {
        Task<TEntity> GetByIdAsync(string id);
        Task<IEnumerable<TEntity>> GetListAsync(string parentId);
        Task CreateAsync(TEntity model);
        Task UpdateAsync(TEntity model);
        Task DeleteAsync(string id);
    }
}
