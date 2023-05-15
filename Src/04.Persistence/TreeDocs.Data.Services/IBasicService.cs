using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TreeDocs.Data.Services
{
    public interface IBasicService<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAll();

        Task Create(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(string id);

    }
}
