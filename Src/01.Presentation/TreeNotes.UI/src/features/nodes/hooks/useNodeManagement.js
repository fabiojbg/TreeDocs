import { useEffect, useCallback, useRef } from 'react'; // Import useRef
import { nodeService } from '../services/nodeService';
import { useNodeStore } from '../store/nodeStore';

window.__treeNeedsReloading = true;
export const useNodeManagement = (user, nodeEditorRef) => { // Accept nodeEditorRef as a parameter
  const { nodes, selectedNode, loading, error, openNodes, setNodes, setSelectedNode, setLoading, setError, addNode, updateNode, deleteNode, toggleNode: toggleNodeStore, moveNode, fetchNodeById, updateNodeChildrenOrder, initializeAllNodesAsOpen } = useNodeStore();

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

  const getNodeByIdFromTree = useCallback((allNodes, nodeId) => {
    let foundNode = null;
    findNodeInTree(allNodes, nodeId, (node) => {
      foundNode = node;
      return true; // Stop traversal
    });
    return foundNode;
  }, [findNodeInTree]);

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

  const processNodesToOpen = (nodesArray, nodesToOpen = {}) => {
    nodesArray.forEach(node => {
      nodesToOpen[node.id] = true;
      if (node.children && node.children.length > 0) {
        processNodesToOpen(node.children, nodesToOpen);
      }
    });
    return nodesToOpen;
  };

  const loadUserNodes = useCallback(async () => {
    // Only proceed if a reload is explicitly requested or if nodes are not loaded yet
    if( !window.__treeNeedsReloading && nodes.length > 0 && selectedNode) {
        // If nodes are already loaded, and a selected node exists, and no explicit reload is requested, do nothing.
        return; 
    }

    setLoading(true);
    setError(null);
    
    // Capture the current state of the reload flag
    const shouldSelectRootImmediately = window.__treeNeedsReloading; 
    window.__treeNeedsReloading = false; // Reset flag as we are about to load

    try {
      const data = await nodeService.getUserNodes();
      setNodes(data.nodes || []);
      
      if (data.nodes && data.nodes.length > 0) {
        const allNodesToOpen = processNodesToOpen(data.nodes);
        initializeAllNodesAsOpen(allNodesToOpen);

        // Select the root node if it's the initial load (selectedNode is null) 
        // or a forced reload after logout (shouldSelectRootImmediately is true).
        if (shouldSelectRootImmediately || !selectedNode) { 
            // Call handleNodeSelect directly which is now defined.
            await handleNodeSelect(data.nodes[0]);
        }
      } else {
        // If no nodes exist for the user (e.g., brand new account), ensure no node is selected.
        setSelectedNode(null);
      }
    } catch (err) {
      setError('Failed to load nodes');
      console.error('Error loading nodes:', err);
    } finally {
      setLoading(false);
    }
  }, [nodes.length, selectedNode, setNodes, setSelectedNode, setLoading, setError, initializeAllNodesAsOpen]); // handleNodeSelect is now a stable reference

  useEffect(() => {
    if (user && user.token) {
      loadUserNodes();
    }
  }, [user, loadUserNodes]);



  const handleNodeSelect = useCallback(async (node) => {
    setError(null);
    
    // Check if the current editor has unsaved changes and save them
    if (nodeEditorRef.current && nodeEditorRef.current.isDirty) {
      console.log('Detected unsaved changes, saving before new node selection.');
      await nodeEditorRef.current.savePendingChanges();
    }

    try {
      // Only set the selected node after fetching its detailed content
      const detailedNodeResponse = await nodeService.getNodeById(node.id);
      const detailedNode = detailedNodeResponse.node;

      setSelectedNode(detailedNode);
      // Ensure the editor loses focus when a new node is selected
      // This is generally handled by React component lifecycle, but can be forced if needed.
    } catch (err) {
      setError(`Failed to load node content: ${err.message}`);
      console.error('Error loading node content:', err);
    } 
  }, [setSelectedNode, setError, nodeEditorRef]); // Add nodeEditorRef to dependencies

  const generateUniqueNodeName = useCallback((baseName, siblings) => {
    if (!siblings || siblings.length === 0) {
      return baseName;
    }

    const siblingNames = new Set(siblings.map(sibling => sibling.name));
    let candidateName = baseName;
    let counter = 2;

    while (siblingNames.has(candidateName)) {
      candidateName = `${baseName} ${counter}`;
      counter++;
    }

    return candidateName;
  }, []);

  const handleNodeCreate = useCallback(async (parentId, name, nodeType, contents) => {
    setError(null);
    try {
      let parentSiblings = [];
      if (parentId) {
        const parent = getNodeByIdFromTree(nodes, parentId);
        if (parent) {
          parentSiblings = parent.children || [];
        }
      } else {
        // If parentId is null, siblings are at the root level
        parentSiblings = nodes;
      }

      const uniqueName = generateUniqueNodeName(name, parentSiblings);

      const createResponse = await nodeService.createNode(parentId, uniqueName, nodeType, contents);
      const newNodeId = createResponse.id;
      const detailedNewNodeResponse = await nodeService.getNodeById(newNodeId);
      const detailedNewNode = detailedNewNodeResponse.node;
      window.__treeNeedsReloading = true;
      addNode(detailedNewNode);
      setSelectedNode(detailedNewNode); 
      return detailedNewNode;
    } catch (err) {
      console.error('Error creating node:', err);
      throw err;
    }
  }, [addNode, setSelectedNode, setError, nodes, getNodeByIdFromTree, generateUniqueNodeName]);

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
      window.__treeNeedsReloading = true;
      deleteNode(nodeId);
    } catch (err) {
      console.error('Error deleting node:', err);
      throw err;
    }
  }, [deleteNode, setError]);

  const calculateNewChildrenOrder = useCallback((parentNode, draggedNodeId, targetNodeId, dropPosition) => {
    if (!parentNode || !parentNode.children) return [];
    
    const currentChildren = [...parentNode.children];
    const draggedIndex = currentChildren.findIndex(child => child.id === draggedNodeId);
    const targetIndex = currentChildren.findIndex(child => child.id === targetNodeId);
    
    if (draggedIndex === -1 || targetIndex === -1) return parentNode.childrenOrder || [];

    // Remove the dragged node from its current position
    const [draggedNode] = currentChildren.splice(draggedIndex, 1);
    
    // Find where to insert it based on the target
    let insertIndex = currentChildren.findIndex(child => child.id === targetNodeId);
    
    if (dropPosition === 'before') {
      // Insert before the target
      currentChildren.splice(insertIndex, 0, draggedNode);
    } else if (dropPosition === 'after') {
      // Insert after the target
      currentChildren.splice(insertIndex + 1, 0, draggedNode);
    } else {
      // Insert inside (at the end)
      currentChildren.push(draggedNode);
    }
    
    // Return the new order of child IDs
    return currentChildren.map(child => child.id);
  }, []);

  const calculateInsertionOrderForNewParent = useCallback((newParentChildren, draggedNodeId, targetNodeId, dropPosition) => {
    const currentChildren = [...newParentChildren];
    const targetIndex = currentChildren.findIndex(child => child.id === targetNodeId);
    
    if (targetIndex === -1) return currentChildren.map(child => child.id);
    
    let insertIndex = targetIndex;
    
    if (dropPosition === 'before') {
      // Insert before the target
      currentChildren.splice(insertIndex, 0, { id: draggedNodeId });
    } else if (dropPosition === 'after') {
      // Insert after the target
      currentChildren.splice(insertIndex + 1, 0, { id: draggedNodeId });
    } else {
      // Insert inside (at the end)
      currentChildren.push({ id: draggedNodeId });
    }
    
    // Return the new order of child IDs
    return currentChildren.map(child => child.id);
  }, []);

  const handleNodeMove = useCallback(async (draggedNodeId, targetNodeId, dropPosition) => {
    setError(null);
    try {
        const draggedNode = getNodeByIdFromTree(nodes, draggedNodeId);
        const targetNode = getNodeByIdFromTree(nodes, targetNodeId);

        if (!draggedNode || !targetNode || draggedNode.id === targetNode.id) {
            return;
        }
        
        window.__treeNeedsReloading = true;

        const newParentId = dropPosition === 'inside' ? targetNode.id : targetNode.parentId;
        const parentIdChanged = draggedNode.parentId !== newParentId;

        if (parentIdChanged) {
          // Parent changed - update the dragged node's parentId and calculate new childrenOrder for new parent
          const newParentNode = getNodeByIdFromTree(nodes, newParentId);
          
          if (!newParentNode) return;

          // Calculate the new childrenOrder for the new parent based on drop position
          const newChildrenOrderForNewParent = calculateInsertionOrderForNewParent(
            newParentNode.children || [], 
            draggedNodeId, 
            targetNodeId, 
            dropPosition
          );

          // First API call: Update the dragged node's parentId
          await nodeService.updateNode(draggedNodeId, newParentId, null, null, null);
          
          // Second API call: Update the new parent's childrenOrder
          await nodeService.updateNode(newParentId, null, null, null, newChildrenOrderForNewParent);
          
          // Reload all user nodes to ensure the UI is in sync with the backend's new tree structure.
          // This is more reliable than complex optimistic updates for tree-altering changes.
          await loadUserNodes();

          // After reloading, find the moved node (which should now exist in the tree with its new parentId)
          // and select it. Also, ensure its new parent is expanded.
          setTimeout(async () => { // Use setTimeout to ensure state from loadUserNodes has settled
              const movedNode = getNodeByIdFromTree(nodes, draggedNodeId);
              if (movedNode) {
                // Fetch full details of the moved node to ensure content is loaded
                await handleNodeSelect(movedNode); 
                if (movedNode.parentId && !openNodes[movedNode.parentId]) {
                  toggleNodeStore(movedNode.parentId);
                }
              } else {
                // Fallback: try to select by ID if getNodeByIdFromTree fails immediately
                // This might happen if the node is no longer visible in the limited 'nodes' state due to filtering or other issues
                console.warn('Moved node not found in current nodes state after loadUserNodes. Attempting re-fetch by ID.');
                await handleNodeSelect({ id: draggedNodeId });
              }
          }, 0);
                                                                                    
        } else {
          // Same parent - reorder children
          const parentNode = getNodeByIdFromTree(nodes, draggedNode.parentId);
          if (!parentNode) return;

          // Calculate new children order
          const newChildrenOrder = calculateNewChildrenOrder(parentNode, draggedNodeId, targetNodeId, dropPosition);
          
          // Check if order actually changed
          const currentOrder = parentNode.childrenOrder || parentNode.children?.map(child => child.id) || [];
          if (JSON.stringify(currentOrder) !== JSON.stringify(newChildrenOrder)) {
            // Update parent's childrenOrder via API
            await nodeService.updateNode(parentNode.id, null, null, null, newChildrenOrder);
            
            // Optimistically update the UI
            updateNodeChildrenOrder(parentNode.id, newChildrenOrder);
          }
        }

    } catch (err) {
        console.error('Error moving node:', err);
        setError('Failed to move node');
        throw err;
    }
}, [nodes, getNodeByIdFromTree, moveNode, setError, calculateNewChildrenOrder, calculateInsertionOrderForNewParent, updateNodeChildrenOrder, loadUserNodes, handleNodeSelect, openNodes, toggleNodeStore]);

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
    handleNodeMove, // Expose handleNodeMove
  };
};
