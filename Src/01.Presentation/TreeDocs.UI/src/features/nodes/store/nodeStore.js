import { create } from 'zustand';
import { nodeService } from '../services/nodeService';

// Helper to find a node in the tree
const findNodeInTree = (nodes, nodeId, callback) => {
    for (const node of nodes) {
        if (node.id === nodeId) {
            return callback(node);
        }
        if (node.children) {
            const found = findNodeInTree(node.children, nodeId, callback);
            if (found) return found;
        }
    }
    return null;
};

// Helper to get siblings and index of a node
const getSiblingsAndIndex = (nodes, nodeId) => {
    for (let i = 0; i < nodes.length; i++) {
        if (nodes[i].id === nodeId) {
            return { siblings: nodes, index: i };
        }
        if (nodes[i].children) {
            const result = getSiblingsAndIndex(nodes[i].children, nodeId);
            if (result) return result;
        }
    }
    return null;
};

export const useNodeStore = create((set) => ({
  nodes: [],
  selectedNode: null,
  loading: false,
  error: null,
  openNodes: {}, // Object to store expanded/collapsed state of nodes

  setNodes: (newNodes) => set({ nodes: newNodes }),
  setSelectedNode: (node) => set({ selectedNode: node }),
  setLoading: (isLoading) => set({ loading: isLoading }),
  setError: (err) => set({ error: err }),
  
  toggleNode: (nodeId) => set((state) => ({
    openNodes: {
      ...state.openNodes,
      [nodeId]: !state.openNodes[nodeId],
    },
  })),

  initializeAllNodesAsOpen: (nodesToOpen) => set((state) => ({
    openNodes: {
      ...state.openNodes,
      ...nodesToOpen,
    },
  })),

  fetchNodeById: async (nodeId) => {
    set({ loading: true });
    try {
      const detailedNode = await nodeService.getNodeById(nodeId);
      set((state) => ({
        selectedNode: {
          ...state.selectedNode,
          ...detailedNode.node,
        },
        loading: false,
      }));
    } catch (err) {
        set({ error: `Failed to load content for ${nodeId}`, loading: false });
      console.error(`Failed to load content for ${nodeId}`, err);
    }
  },

  addNode: (newNode) => set((state) => {
    const addNodeToTree = (nodesArray) => {
      if (!newNode.parentId) {
        return [...nodesArray, newNode];
      }
      return nodesArray.map(node => {
        if (node.id === newNode.parentId) {
          return {
            ...node,
            children: node.children ? [...node.children, newNode] : [newNode]
          };
        }
        if (node.children && node.children.length > 0) {
          return {
            ...node,
            children: addNodeToTree(node.children)
          };
        }
        return node;
      });
    };
    return { nodes: addNodeToTree(state.nodes) };
  }),

  updateNode: (updatedNode) => set((state) => {
    const updateNodeInTree = (nodesArray) => {
      return nodesArray.map(node => {
        if (node.id === updatedNode.id) {
          // Merge only the explicit updates from the API response, preserving existing fields
          return {
            ...node,
            name: updatedNode.name !== undefined ? updatedNode.name : node.name,
            contents: updatedNode.contents !== undefined ? updatedNode.contents : node.contents,
            // parentId and childrenOrder are typically updated via moveNode or updateNodeChildrenOrder,
            // but if they come from a general update, apply them.
            parentId: updatedNode.parentId !== undefined ? updatedNode.parentId : node.parentId,
            childrenOrder: updatedNode.childrenOrder !== undefined ? updatedNode.childrenOrder : node.childrenOrder,
            // Do not overwrite 'children' explicitly here, as the API might omit them on update.
            // If the API returns a 'children' array, it will be handled by the spread if it's explicitly present,
            // but in this fix, we are assuming it's absent and we want to preserve the existing one.
            // The existing 'children' array will be implicitly preserved because it's not present in updatedNode.
          };
        }
        if (node.children && node.children.length > 0) {
          return {
            ...node,
            children: updateNodeInTree(node.children)
          };
        }
        return node;
      });
    };
    
    const newNodes = updateNodeInTree(state.nodes);
    let newSelectedNode = state.selectedNode;
    if (state.selectedNode && state.selectedNode.id === updatedNode.id) {
      newSelectedNode = {
        ...state.selectedNode,
        name: updatedNode.name !== undefined ? updatedNode.name : state.selectedNode.name,
        contents: updatedNode.contents !== undefined ? updatedNode.contents : state.selectedNode.contents,
        parentId: updatedNode.parentId !== undefined ? updatedNode.parentId : state.selectedNode.parentId,
        childrenOrder: updatedNode.childrenOrder !== undefined ? updatedNode.childrenOrder : state.selectedNode.childrenOrder,
      };
    }
    return { nodes: newNodes, selectedNode: newSelectedNode };
  }),
  
  deleteNode: (nodeId) => set((state) => {
    const removeNodeFromTree = (nodesArray) => {
      return nodesArray.filter(node => node.id !== nodeId).map(node => {
        if (node.children && node.children.length > 0) {
          return {
            ...node,
            children: removeNodeFromTree(node.children)
          };
        }
        return node;
      });
    };
    
    const newNodes = removeNodeFromTree(state.nodes);
    let newSelectedNode = state.selectedNode;
    if (state.selectedNode && state.selectedNode.id === nodeId) {
      newSelectedNode = null;
    }
    return { nodes: newNodes, selectedNode: newSelectedNode };
  }),

  updateNodeChildrenOrder: (parentId, childrenOrder) => set((state) => {
    const updateParentOrder = (nodesArray) => {
      return nodesArray.map(node => {
        if (node.id === parentId) {
          // Update the childrenOrder property
          const updatedNode = { ...node, childrenOrder: [...childrenOrder] };

          // If the node has children, reorder them to match the new childrenOrder
          if (updatedNode.children && updatedNode.children.length > 0) {
            // Create a map for quick lookup
            const childrenMap = new Map();
            updatedNode.children.forEach(child => {
              childrenMap.set(child.id, child);
            });

            // Reorder the children array based on the new childrenOrder
            const reorderedChildren = childrenOrder
              .map(childId => childrenMap.get(childId))
              .filter(child => child !== undefined); // Filter out any children that might be missing

            updatedNode.children = reorderedChildren;
          }

          return updatedNode;
        }
        if (node.children && node.children.length > 0) {
          return {
            ...node,
            children: updateParentOrder(node.children)
          };
        }
        return node;
      });
    };
    
    return { nodes: updateParentOrder(state.nodes) };
  }),

  moveNode: (draggedNodeId, newParentId) => set((state) => {
    const nodes = JSON.parse(JSON.stringify(state.nodes)); // Deep copy to ensure immutability

    let draggedNode = null;
    let oldParent = null; // Store old parent reference

    // 1. Find and remove the dragged node from its original position
    const findAndRemove = (currentNodes, parent = null) => {
      for (let i = 0; i < currentNodes.length; i++) {
        const node = currentNodes[i];
        if (node.id === draggedNodeId) {
          draggedNode = { ...node, parentId: newParentId }; // Update parentId for the moved node
          oldParent = parent; // Save old parent reference
          currentNodes.splice(i, 1); // Remove from old location
          
          // If old parent had childrenOrder, remove from there as well
          if (oldParent && oldParent.childrenOrder) {
            oldParent.childrenOrder = oldParent.childrenOrder.filter(id => id !== draggedNodeId);
          }
          return true;
        }
        if (node.children && node.children.length > 0) {
          if (findAndRemove(node.children, node)) {
            return true;
          }
        }
      }
      return false;
    };

    findAndRemove(nodes);

    if (!draggedNode) {
      console.warn("Dragged node not found in state:", draggedNodeId);
      return {}; 
    }

    // 2. Add the dragged node to its new parent's children array
    if (newParentId === null) { // Moving to root level
      nodes.push(draggedNode);
    } else {
      const findAndAdd = (currentNodes) => {
        for (const node of currentNodes) {
          if (node.id === newParentId) {
            if (!node.children) {
              node.children = [];
            }
            node.children.push(draggedNode); // Temporarily add to end; order will be fixed by updateNodeChildrenOrder
            return true;
          }
          if (node.children && node.children.length > 0) {
            if (findAndAdd(node.children)) {
              return true;
            }
          }
        }
        return false;
      };
      findAndAdd(nodes);
    }

    return { nodes };
  }),
}));
