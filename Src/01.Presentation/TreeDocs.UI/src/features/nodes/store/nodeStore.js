import { create } from 'zustand';

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
          return { ...node, ...updatedNode };
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
      newSelectedNode = { ...state.selectedNode, ...updatedNode };
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
}));
