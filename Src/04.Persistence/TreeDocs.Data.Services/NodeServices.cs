using TreeDocs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Treedocs.Data.Services;

namespace TreeDocs.Data.Services
{
    public class NodeServices : INodeServices
    {
        IRepository<UserNode> _userNodeServices;

        public NodeServices(IRepository<UserNode> userNodeServices)
        {
            _userNodeServices = userNodeServices;
        }

        public async Task<IEnumerable<UserNode>> GetChildNodes(string parentId)
        {
            var result = await _userNodeServices.GetListAsync(parentId);
            return result;
        }

        public Task<IEnumerable<UserNode>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task Create(UserNode userNode)
        {
            await _userNodeServices.CreateAsync(userNode);
        }

        public async Task Update(UserNode userNode)
        {
            await _userNodeServices.UpdateAsync(userNode);
        }

        public async Task Delete(string id)
        {
            await _userNodeServices.DeleteAsync(id);
        }
    }
}
