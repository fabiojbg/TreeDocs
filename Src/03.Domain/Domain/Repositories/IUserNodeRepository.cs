﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TreeDocs.Domain.Entities;

namespace TreeDocs.Domain.Repositories
{
    public interface IUserNodeRepository
    {
        Task<string> CreateAsync(UserNode userNode);
        Task DeleteAsync(string id);
        Task<UserNode> GetByIdAsync(string id);
        Task<IEnumerable<UserNode>> GetChildrenWithoutContentsAsync(string folderId);
        Task<IEnumerable<UserNode>> GetAllUserNodesWithoutContentsAsync(string userId);
        Task UpdateAsync(UserNode userNode);
        Task<UserNode> GetNodeByName(string ownerId, string folderId, string nodeName);
        Task<bool> FolderExists(string folderId);
        Task SaveChanges();
    }
}