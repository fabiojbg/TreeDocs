import { useState, useCallback } from 'react';

const getDropPosition = (e, rect) => {
  const clientY = e.clientY;
  const beforeThreshold = rect.height * 0.25;
  const afterThreshold = rect.height * 0.75;

  if (clientY < rect.top + beforeThreshold) {
    return 'before';
  } else if (clientY > rect.top + afterThreshold) {
    return 'after';
  } else {
    return 'inside';
  }
};

const isDescendant = (nodes, parent, childId) => {
  const parentNode = nodes.find(n => n.id === parent.id);
  if (!parentNode || !parentNode.children) return false;
  for (const c of parentNode.children) {
    if (c.id === childId) return true;
    if (isDescendant(nodes, c, childId)) return true;
  }
  return false;
};

export const useNodeDragAndDrop = (nodes, onMoveNode) => {
  const [draggedNode, setDraggedNode] = useState(null);
  const [draggedOverNodeId, setDraggedOverNodeId] = useState(null);
  const [dropPositionState, setDropPositionState] = useState(null);

  const handleDragStart = useCallback((e, node) => {
    setDraggedNode(node);
    e.dataTransfer.effectAllowed = "move";
    e.dataTransfer.setData("nodeId", node.id);
  }, []);

  const handleDragEnter = useCallback((e, targetNode) => {
    e.preventDefault();
    if (draggedNode && draggedNode.id !== targetNode.id) {
      setDraggedOverNodeId(targetNode.id);
    }
  }, [draggedNode]);

  const handleDragLeave = useCallback((e, targetNode) => {
    e.preventDefault();
    if (draggedOverNodeId === targetNode.id) {
      setDraggedOverNodeId(null);
      setDropPositionState(null);
    }
  }, [draggedOverNodeId]);

  const handleDragOver = useCallback((e, targetNode) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = "move";

    if (draggedNode && draggedNode.id !== targetNode.id) {
      const rect = e.currentTarget.getBoundingClientRect();
      const currentDropPosition = getDropPosition(e, rect);

      setDraggedOverNodeId(targetNode.id);
      setDropPositionState(currentDropPosition);
    }
  }, [draggedNode]);

  const handleDrop = useCallback(async (e, targetNode, explicitDropPosition = null) => {
    e.preventDefault();
    setDraggedOverNodeId(null);
    setDropPositionState(null);

    const draggedNodeId = e.dataTransfer.getData("nodeId");
    if (draggedNodeId === targetNode.id) return;

    if (draggedNode && isDescendant(nodes, draggedNode, targetNode.id)) {
      console.warn("Cannot drop a node into its own descendant.");
      return;
    }

    let dropPosition = explicitDropPosition;
    if (!dropPosition) {
      const rect = e.currentTarget.getBoundingClientRect();
      dropPosition = getDropPosition(e, rect);
    }

    if (draggedNode && onMoveNode) {
      onMoveNode(draggedNode.id, targetNode.id, dropPosition);
    }
    setDraggedNode(null);
  }, [draggedNode, onMoveNode, nodes]);

  const handleDragEnd = useCallback(() => {
    setDraggedNode(null);
    setDraggedOverNodeId(null);
    setDropPositionState(null);
  }, []);

  return {
    draggedNode,
    draggedOverNodeId,
    dropPositionState,
    handleDragStart,
    handleDragEnter,
    handleDragLeave,
    handleDragOver,
    handleDrop,
    handleDragEnd,
  };
};
