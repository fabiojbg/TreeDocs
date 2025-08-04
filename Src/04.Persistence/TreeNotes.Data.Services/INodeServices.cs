using TreeNotes.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TreeNotes.Data.Services
{
    public interface INodeServices : IBasicService<UserNode>
    {
        Task<IEnumerable<UserNode>> GetChildNodes(string parentId);
    }
}