import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { nodeService } from '../services/nodeService';
import Header from '../components/layout/Header';
import NodeTree from '../components/nodes/NodeTree';
import NodeEditor from '../components/nodes/NodeEditor';
import { useNavigate } from 'react-router-dom';

export default function DashboardPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [nodes, setNodes] = useState([]);
  const [selectedNode, setSelectedNode] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  useEffect(() => {
    if (user && user.token) { // Ensure user and token are available
      loadUserNodes()
    }
  }, [user])

  const loadUserNodes = async () => {
    try {
      setLoading(true)
      const data = await nodeService.getUserNodes()
      setNodes(data.nodes || [])
      
      // Select the first node by default if none selected
      if (!selectedNode && data.nodes && data.nodes.length > 0) {
        setSelectedNode(data.nodes[0])
      }
    } catch (err) {
      setError('Failed to load nodes')
      console.error('Error loading nodes:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleNodeSelect = async (node) => {
    if (!node) {
      setSelectedNode(null);
      return;
    }

    // Optimistically set the selected node to keep the UI responsive
    setSelectedNode(node);

    try {
      const detailedNode = await nodeService.getNodeById(node.id);
      // Merge the content into the existing node object to preserve children
      setSelectedNode(prevNode => ({
        ...prevNode,
        ...detailedNode.node,
      }));
    } catch (err) {
      setError(`Failed to load content for ${node.name}`);
      console.error('Error fetching node details:', err);
    }
  };

  const handleNodeCreate = async (parentId, name, nodeType, contents) => {
    try {
      const createResponse = await nodeService.createNode(parentId, name, nodeType, contents)
      const newNodeId = createResponse.id;
      const detailedNewNodeResponse = await nodeService.getNodeById(newNodeId);
      const detailedNewNode = detailedNewNodeResponse.node;

      setNodes(prevNodes => {
        const addNodeToTree = (nodesArray) => {
          if (!parentId) {
            return [...nodesArray, detailedNewNode]; // Add to root if no parent
          }
          return nodesArray.map(node => {
            if (node.id === parentId) {
              return {
                ...node,
                children: node.children ? [...node.children, detailedNewNode] : [detailedNewNode]
              }
            }
            if (node.children && node.children.length > 0) {
              return {
                ...node,
                children: addNodeToTree(node.children)
              }
            }
            return node
          })
        }
        return addNodeToTree(prevNodes)
      })
      
      // Select the newly created node
      setSelectedNode(detailedNewNode); 
      
      return detailedNewNode // Return the detailed node for NodeTree to use
    } catch (err) {
      console.error('Error creating node:', err)
      throw err
    }
  }

  const handleNodeUpdate = async (nodeId, updates) => {
    try {
      const updatedNodeResponse = await nodeService.updateNode(nodeId, updates.parentId, updates.name, updates.contents, updates.childrenOrder)
      const updatedNode = updatedNodeResponse.node;

      // Update the specific node in the existing tree structure
      setNodes(prevNodes => {
        const updateNodeInTree = (nodesArray) => {
          return nodesArray.map(node => {
            if (node.id === nodeId) {
              // Update only the properties that were changed, and ensure children are preserved
              return { ...node, ...updatedNode }
            }
            if (node.children && node.children.length > 0) {
              return {
                ...node,
                children: updateNodeInTree(node.children)
              }
            }
            return node
          })
        }
        return updateNodeInTree(prevNodes)
      })

      // Update selected node if it was the one updated
      if (selectedNode && selectedNode.id === nodeId) {
        setSelectedNode(prevSelectedNode => ({
          ...prevSelectedNode,
          ...updatedNode,
        }))
      }
      return updatedNodeResponse
    } catch (err) {
      console.error('Error updating node:', err)
      throw err
    }
  }

  const handleNodeDelete = async (nodeId) => {
    try {
      await nodeService.deleteNode(nodeId)
      
      setNodes(prevNodes => {
        const removeNodeFromTree = (nodesArray) => {
          return nodesArray.filter(node => node.id !== nodeId).map(node => {
            if (node.children && node.children.length > 0) {
              return {
                ...node,
                children: removeNodeFromTree(node.children)
              }
            }
            return node
          })
        }
        return removeNodeFromTree(prevNodes)
      })
      
      // Clear selection if deleted node was selected
      if (selectedNode && selectedNode.id === nodeId) {
        setSelectedNode(null)
      }
    } catch (err) {
      console.error('Error deleting node:', err)
      throw err
    }
  }

  if (!user) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <p className="text-gray-600">Please log in to access the dashboard.</p>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Header user={user} onLogout={handleLogout} />
      
      <div className="flex h-screen pt-16">
        {/* Sidebar - Node Tree */}
        <div className="w-1/4 bg-white border-r border-gray-200 overflow-y-auto">
          <div className="p-4">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold text-gray-900">Your Notes</h2>
              <button
                onClick={() => handleNodeCreate(null, 'New Node', 'Document', '')}
                className="inline-flex items-center px-3 py-1 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                <svg className="-ml-1 mr-1 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
                Add
              </button>
            </div>
            
            {loading ? (
              <div className="flex items-center justify-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
              </div>
            ) : error ? (
              <div className="bg-red-50 border border-red-200 rounded-md p-4">
                <p className="text-sm text-red-700">{error}</p>
                <button
                  onClick={loadUserNodes}
                  className="mt-2 text-sm text-indigo-600 hover:text-indigo-500"
                >
                  Retry
                </button>
              </div>
            ) : (
              <NodeTree
                nodes={nodes}
                selectedNode={selectedNode}
                onSelect={handleNodeSelect}
                onCreate={handleNodeCreate}
                onDelete={handleNodeDelete}
              />
            )}
          </div>
        </div>

        {/* Main Content - Node Editor */}
        <div className="flex-1 overflow-hidden">
          {selectedNode ? (
            <NodeEditor
              node={selectedNode}
              onUpdate={handleNodeUpdate}
            />
          ) : (
            <div className="flex items-center justify-center h-full">
              <div className="text-center">
                <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <h3 className="mt-2 text-sm font-medium text-gray-900">No node selected</h3>
                <p className="mt-1 text-sm text-gray-500">Select a node from the sidebar to view or edit its contents.</p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
