using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.MongoDb.Repositories;
using Repository.MongoDb.DbModels;
using Apps.Sdk.Helpers;
using MongoDB.Bson;
using TreeDocs.Domain.Entities;
using TreeDocs.Domain.Repositories;
using TreeDocs.Domain.Enums;
using System.IO;
using Apps.Sdk.Extensions;
using Repository.MongoDb.Extensions;

namespace Repository.MongoDb
{
    public enum Position
    {
        Start = 0,
        End = -1,
        NoChange = -2
    };

    public class UserNodeRepository : IUserNodeRepository
    {
        readonly int MAX_DOC_SIZE_FOR_ENTRY = 10000000;
        IRepository<DbUserNode> _nodeRepository;
        IFileRepository<DbNodeContents> _contentsRepository;

        public UserNodeRepository( IRepository<DbUserNode> nodeRepository,
                                   IFileRepository<DbNodeContents> contentsRepository)
        {
            _nodeRepository = nodeRepository;
            _contentsRepository = contentsRepository;
        }

        public Task SaveChanges()
        {
            return _nodeRepository.SaveChangesAsync();
        }

        public async Task<string> CreateAsync(UserNode userNode)
        {
            var (node, nodeContents) = await this.convertToDbModel(userNode);

            if (nodeContents != null)
            {
                node.Id = ObjectId.GenerateNewId();
                nodeContents.Name = node.Id.ToString();
                await _contentsRepository.CreateAsync(nodeContents);
                node.ContentId = nodeContents.Id;
            }

            await _nodeRepository.CreateAsync(node);

            userNode.Id = node.Id.ToString();

            return node.Id.ToString();
        }

        public async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId oid))
                return;

            var node = await _nodeRepository.GetByIdAsync(oid);
            if (node == null)
                return;

            if (node.ContentId != null)
            {
                await _contentsRepository.DeleteAsync(node.ContentId.Value);
            }

            await _nodeRepository.DeleteAsync(oid);
        }


        public async Task<UserNode> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId oid))
                return null;

            var dbNode = await _nodeRepository.GetByIdAsync(oid);
            if (dbNode == null)
                return null;

            var result = convertToModel(dbNode);

            if (result.Contents == null && dbNode.ContentId != null)
            {
                var nodeContents = await _contentsRepository.GetByIdAsync(dbNode.ContentId.Value);
                result.Contents = nodeContents?.Contents != null ? Encoding.UTF8.GetString(nodeContents.Contents) : "";
            }

            return result;
        }


        public Task<bool> FolderExists(string folderId)
        {
            if (String.IsNullOrEmpty(folderId))
                return Task.FromResult(true);

            if (!ObjectId.TryParse(folderId, out ObjectId folderOid))
                throw new InvalidDataException("Invalid folder Id");

            var node = _nodeRepository.Query.Where(n => n.Id == folderOid && n.NodeType == (byte)NodeType.Folder).FirstOrDefault();

            return Task.FromResult(node != null);

        }

        public Task<UserNode> GetNodeByName(string ownerId, string parentId, string nodeName)
        {
            ObjectId? folderOid = null;
            ObjectId? ownerOid = null;
            if (parentId != null && !parentId.TryObjectId(out folderOid))
                throw new InvalidDataException("Invalid folder Id");
            if (ownerId != null && !ownerId.TryObjectId(out ownerOid))
                throw new InvalidDataException("Invalid owner Id");

            var node = _nodeRepository.Query.Where(n => n.OwnerId==ownerId && 
                                                        n.ParentId==folderOid && 
                                                        n.Name.ToLowerInvariant() == nodeName.ToLowerInvariant())
                                            .FirstOrDefault();

            return Task.FromResult(convertToModel(node));
        }

        public async Task<IEnumerable<UserNode>> GetChildrenWithoutContentsAsync(string folderId)
        {
            if (!folderId.TryObjectId(out ObjectId? folderOid))
                throw new InvalidDataException("Invalid folder Id");

            var nodes = await _nodeRepository.GetListExcludingFieldsAsync(folderOid, nameof(DbUserNode.Contents));

            var result = nodes.Select(node => convertToModel(node));

            return result;
        }

        
        public async Task<IEnumerable<UserNode>> GetAllUserNodesWithoutContentsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidDataException("Invalid userId");

            var nodes = await _nodeRepository.GetListExcludingFieldsAsync(userId, nameof(DbUserNode.Contents));

            var result = nodes.Select(node => convertToModel(node));

            return result;
        }

        public async Task UpdateAsync(UserNode userNode)
        {
            if (userNode == null || !ObjectId.TryParse(userNode.Id, out ObjectId objOid))
                throw new InvalidOperationException($"Object and its Id cannot be null");

            var dbNode = await _nodeRepository.GetByIdAsync(objOid);
            if (dbNode == null)
                throw new InvalidOperationException($"Cannot find node with Id = {objOid}");

            ObjectId? folderOid = null;
            if (userNode.ParentId != null)
            {
                if (ObjectId.TryParse(userNode.ParentId, out ObjectId newFolderOid))
                    folderOid = newFolderOid;
                else
                    throw new InvalidOperationException($"Folder id is invalid");
            }

            if (folderOid != dbNode.ParentId)
            {
                if (folderOid == null)
                {
                    dbNode.InheritancePath = null;
                }
                else
                {
                    var parent = await _nodeRepository.GetByIdAsync(folderOid.Value);
                    if (parent == null)
                        throw new InvalidOperationException(@"Parent node does not exists");
                    dbNode.InheritancePath = parent.InheritancePath + "|" + parent.Id;
                    dbNode.ParentId = folderOid;
                }
            }
            dbNode.Name = userNode.Name;
            dbNode.UpdatedOn = DateTime.UtcNow;
            if (userNode.ChildrenOrder != null)
                dbNode.ChildrenOrder = userNode.ChildrenOrder.ToArray();

            if (userNode.Contents?.Length > MAX_DOC_SIZE_FOR_ENTRY)
            {
                DbNodeContents nodeContents = null;

                if (dbNode.ContentId != null)
                    nodeContents = await _contentsRepository.GetInfoByIdWithoutContentsAsync(dbNode.ContentId.Value);

                if (nodeContents == null)
                    nodeContents = new DbNodeContents();

                if (nodeContents.Id == ObjectId.Empty || nodeContents.ContentsHash != HashingHelper.ToSHA1Hash(userNode.Contents))
                {
                    if (nodeContents.Id != ObjectId.Empty)
                        await _contentsRepository.DeleteAsync(nodeContents.Id);

                    nodeContents.Contents = Encoding.UTF8.GetBytes(userNode.Contents);

                    await _contentsRepository.CreateAsync(nodeContents);
                    dbNode.ContentId = nodeContents.Id;
                }
            }
            else
            {
                dbNode.Contents = userNode.Contents;
            }

            await _nodeRepository.UpdateAsync(dbNode);
        }


        private UserNode convertToModel(DbUserNode node)
        {
            if (node == null)
                return null;

            var userNode = new UserNode(node.ParentId?.ToString(), node.Name, (NodeType)node.NodeType, node.OwnerId, node.Contents);

            userNode.Id = node.Id.ToString();
            userNode.ParentId = node.ParentId?.ToString();
            userNode.CreatedOn = node.CreatedOn;
            userNode.UpdatedOn = node.UpdatedOn;
            userNode.OwnerId = node.OwnerId;
            userNode.ChildrenOrder = node.ChildrenOrder?.ToList() ?? new List<string>();

            return userNode;
        }

        private async Task<(DbUserNode usernode, DbNodeContents nodeContents)> convertToDbModel(UserNode userNode)
        {
            DbUserNode parent = null;
            DbNodeContents nodeContents = null;

            if (!userNode.ParentId.TryObjectId(out ObjectId? folderOid))
                throw new InvalidOperationException($"Folder id is invalid");

            if( String.IsNullOrWhiteSpace(userNode.OwnerId))
                throw new InvalidOperationException($"OwnerId is invalid");

            if (folderOid != null)
            {
                parent = await _nodeRepository.GetByIdAsync(folderOid.Value);
                if (parent == null)
                    throw new InvalidOperationException(@"Parent node does not exists");
            }

            if (userNode?.Contents?.Length > MAX_DOC_SIZE_FOR_ENTRY)
            {
                nodeContents = new DbNodeContents
                {
                    Contents = Encoding.UTF8.GetBytes(userNode.Contents),
                };
            }
            var dbUserNode = new DbUserNode()
            {
                Contents = nodeContents == null ? userNode.Contents : null,
                ParentId = folderOid,
                InheritancePath = parent == null ? "" : parent.InheritancePath + "|" + parent.Id,
                Name = userNode.Name,
                NodeType = (byte)userNode.NodeType,
                OwnerId = userNode.OwnerId,
                ChildrenOrder = userNode.ChildrenOrder?.ToArray()??new string[0]
            };

            return (dbUserNode, nodeContents);
        }

    }
}
