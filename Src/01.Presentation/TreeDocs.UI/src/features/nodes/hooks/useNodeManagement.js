import { useEffect, useCallback } from 'react';
import { nodeService } from '../services/nodeService';
import { useNodeStore } from '../store/nodeStore';

export const useNodeManagement = (user) => {
  const { nodes, selectedNode, loading, error, openNodes, setNodes, setSelectedNode, setLoading, setError, addNode, updateNode, deleteNode, toggleNode: toggleNodeStore } = useNodeStore();

  const findNodeInTree = useCallback((currentNodes, nodeId, callback) => {
    for (let i = 0; i < currentNodes.length; i++) {
        const node = currentNodes[i];
        if (node.id === nodeId) {
            return callback(node, currentNodes, i);
        }
        if (node.children && node.children.length > 0) {
            const found = findNodeInTree(node.children, nodeId, callback);
            if (found) return found;
        }
    }
    return null;
  }, []);

  const getSiblingsAndIndex = useCallback((allNodes, targetId) => {
    let result = { siblings: [], index: -1 };

    findNodeInTree(allNodes, targetId, (node, siblingsArray, index) => {
        result = { siblings: siblingsArray, index: index };
        return true; 
    });
    return result;
  }, [findNodeInTree]);

  const getVisibleNodes = useCallback((nodesArray, openNodesState) => {
    let visible = [];

    const traverse = (currentNodes) => {
      currentNodes.forEach(node => {
        visible.push(node);
        if (node.children && openNodesState[node.id]) {
          traverse(node.children);
        }
      });
    };

    traverse(nodesArray);
    return visible;
  }, []);

  const loadUserNodes = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await nodeService.getUserNodes();
      setNodes(data.nodes || []);
      
      if (!selectedNode && data.nodes && data.nodes.length > 0) {
        setSelectedNode(data.nodes[0]);
      }
    } catch (err) {
      setError('Failed to load nodes');
      console.error('Error loading nodes:', err);
    } finally {
      setLoading(false);
    }
  }, [selectedNode, setNodes, setSelectedNode, setLoading, setError]);

  useEffect(() => {
    if (user && user.token) {
      loadUserNodes();
    }
  }, [user, loadUserNodes]);

  const handleNodeSelect = useCallback(async (node) => {
    setSelectedNode(node);
    setError(null);
    try {
      const detailedNode = await nodeService.getNodeById(node.id);
      setSelectedNode({
        ...node, // Keep existing properties like children
        ...detailedNode.node, // Overlay detailed properties like contents
      });
    } catch (err) {
      setError(`Failed to load content for ${node.name}`);
      console.error('Error fetching node details:', err);
    }
  }, [setSelectedNode, setError]);

  const handleNodeCreate = useCallback(async (parentId, name, nodeType, contents) => {
    setError(null);
    try {
      const createResponse = await nodeService.createNode(parentId, name, nodeType, contents);
      const newNodeId = createResponse.id;
      const detailedNewNodeResponse = await nodeService.getNodeById(newNodeId);
      const detailedNewNode = detailedNewNodeResponse.node;
      addNode(detailedNewNode);
      setSelectedNode(detailedNewNode); 
      return detailedNewNode;
    } catch (err) {
      console.error('Error creating node:', err);
      throw err;
    }
  }, [addNode, setSelectedNode, setError]);

  const handleNodeUpdate = useCallback(async (nodeId, updates) => {
    setError(null);
    try {
      const updatedNodeResponse = await nodeService.updateNode(nodeId, updates.parentId, updates.name, updates.contents, updates.childrenOrder);
      const updatedNode = updatedNodeResponse.node;
      updateNode(updatedNode);
      return updatedNodeResponse;
    } catch (err) {
      console.error('Error updating node:', err);
      throw err;
    }
  }, [updateNode, setError]);

  const handleNodeDelete = useCallback(async (nodeId) => {
    setError(null);
    try {
      await nodeService.deleteNode(nodeId);
      deleteNode(nodeId);
    } catch (err) {
      console.error('Error deleting node:', err);
      throw err;
    }
  }, [deleteNode, setError]);

  const handleKeyboardNavigation = useCallback((event) => {
    if (!selectedNode || nodes.length === 0) {
      if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(event.key)) {
        event.preventDefault();
      }
      return;
    }

    const visibleNodes = getVisibleNodes(nodes, openNodes);
    const selectedIndex = visibleNodes.findIndex(node => node.id === selectedNode.id);

    let nextNode = null;

    switch (event.key) {
      case 'ArrowUp':
        event.preventDefault();
        if (selectedIndex > 0) {
          nextNode = visibleNodes[selectedIndex - 1];
        }
        break;
      case 'ArrowDown':
        event.preventDefault();
        if (selectedIndex < visibleNodes.length - 1) {
          nextNode = visibleNodes[selectedIndex + 1];
        }
        break;
      case 'ArrowLeft':
        event.preventDefault();
        if (selectedNode.children && openNodes[selectedNode.id]) {
          // Collapse if currently open
          toggleNodeStore(selectedNode.id);
        } else if (selectedNode.parentId) {
          // Move to parent if not collapsed and has parent
          findNodeInTree(nodes, selectedNode.parentId, (parent) => {
            nextNode = parent;
            return true;
          });
        }
        break;
      case 'ArrowRight':
        event.preventDefault();
        if (selectedNode.children && selectedNode.children.length > 0) {
          // Expand if has children
          if (!openNodes[selectedNode.id]) {
            toggleNodeStore(selectedNode.id);
          } else {
            // Move to first child if already expanded
            nextNode = selectedNode.children[0];
          }
        }
        break;
      default:
        break;
    }

    if (nextNode) {
      handleNodeSelect(nextNode);
    }
  }, [nodes, selectedNode, openNodes, toggleNodeStore, findNodeInTree, getVisibleNodes, handleNodeSelect]);

  return {
    nodes,
    selectedNode,
    loading,
    error,
    loadUserNodes,
    handleNodeSelect,
    handleNodeCreate,
    handleNodeUpdate,
    handleNodeDelete,
    handleKeyboardNavigation,
  };
};
