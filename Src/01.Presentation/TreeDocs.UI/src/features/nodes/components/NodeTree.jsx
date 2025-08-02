import React, { useEffect, useCallback, useState } from 'react'
import NodeItem from './NodeItem'
import { useNodeStore } from '../store/nodeStore';
import { useNodeDragAndDrop } from '../hooks/useNodeDragAndDrop';

export default function NodeTree({ nodes, selectedNode, onSelect, onCreate, onDelete, onMoveNode }) {
  const { openNodes, toggleNode: toggleNodeStore } = useNodeStore();
  const [contextMenu, setContextMenu] = useState({ visible: false, x: 0, y: 0, node: null });
  
  const {
    draggedNode,
    draggedOverNodeId,
    dropPositionState,
    handleDragStart,
    handleDragEnter,
    handleDragLeave,
    handleDragOver,
    handleDrop,
  handleDragEnd,
  } = useNodeDragAndDrop(nodes, onMoveNode);

  const toggleNode = useCallback((nodeId) => {
    toggleNodeStore(nodeId);
  }, [toggleNodeStore]);


  const handleContextMenu = useCallback((e, node) => {
    e.preventDefault()
    setContextMenu({
      visible: true,
      x: e.clientX,
      y: e.clientY,
      node: node
    })
  }, []);

  const handleAddChild = useCallback(() => {
    if (contextMenu.node) {
      onCreate(contextMenu.node.id, 'New Child', 'Document', '')
      setContextMenu({ visible: false, x: 0, y: 0, node: null })
    }
  }, [contextMenu.node, onCreate]);

  const handleDelete = useCallback(() => {
    if (contextMenu.node) {
      if (window.confirm(`Are you sure you want to delete "${contextMenu.node.name}" and all its children?`)) {
        onDelete(contextMenu.node.id)
      }
      setContextMenu({ visible: false, x: 0, y: 0, node: null })
    }
  }, [contextMenu.node, onDelete]);

  const handleClickOutside = useCallback((e) => {
    if (contextMenu.visible) {
      const menu = document.getElementById('context-menu')
      if (menu && !menu.contains(e.target)) {
        setContextMenu({ visible: false, x: 0, y: 0, node: null })
      }
    }
  }, [contextMenu.visible]);

  useEffect(() => {
    document.addEventListener('click', handleClickOutside)
    return () => {
      document.removeEventListener('click', handleClickOutside)
    }
  }, [handleClickOutside]);

  const renderNode = useCallback((node, level = 0) => {
    const isExpanded = openNodes[node.id];
    const isSelected = selectedNode && selectedNode.id === node.id;
    const hasChildren = node.children && node.children.length > 0;
    const isDraggedOver = draggedOverNodeId === node.id;

    return (
      <div key={node.id} className="relative">
        {/* Render 'before' line indicator */}
        {isDraggedOver && dropPositionState === 'before' && draggedNode && draggedNode.id !== node.id && (
            <div 
                className={`absolute top-0 left-0 right-0 h-0.5 bg-blue-500`} 
                style={{ marginLeft: `${level * 16 + 8}px` }} 
            />
        )}
        <NodeItem
          node={node}
          level={level}
          isExpanded={isExpanded}
          isSelected={isSelected}
          hasChildren={hasChildren}
          onSelect={onSelect}
          onToggle={toggleNode}
          onContextMenu={handleContextMenu}
          onDragStart={handleDragStart}
          onDragOver={handleDragOver}
          onDrop={handleDrop}
          onDragEnd={handleDragEnd}
          onDragEnter={handleDragEnter}
          onDragLeave={handleDragLeave}
          draggedOverNodeId={draggedOverNodeId}
          dropPositionState={dropPositionState} // Pass the drop position state
        />
        {/* Render 'after' line indicator */}
        {isDraggedOver && dropPositionState === 'after' && draggedNode && draggedNode.id !== node.id && (
            <div 
                className={`absolute bottom-0 left-0 right-0 h-0.5 bg-blue-500`} 
                style={{ marginLeft: `${level * 16 + 8}px` }} 
            />
        )}

        {/* Dedicated "inside" drop zone for folders when hovered */}
        {node.nodeType === 'Folder' && isDraggedOver && draggedNode && draggedNode.id !== node.id && (
          <div
            className={`absolute inset-0 border-2 border-transparent ${dropPositionState === 'inside' ? 'border-blue-500' : ''}`}
            onDragOver={(e) => handleDragOver(e, node)} // Continue dragOver to keep state
            onDrop={(e) => handleDrop(e, node, 'inside')} // Explicitly set dropPosition to 'inside'
            style={{ marginLeft: `${level * 16 + 8}px`, zIndex: 1 }} // Ensure it's on top and aligned
          />
        )}

        {isExpanded && hasChildren && (
          <div className="ml-4">
            {/* Dropzone for end of folder content - still useful for dropping to end of expanded folder */}
            {node.nodeType === 'Folder' && isExpanded && (
              <div
                className="h-2 w-full"
                onDragOver={(e) => handleDragOver(e, node)}
                onDrop={(e) => handleDrop(e, node, 'inside')} // Explicitly set dropPosition to 'inside'
                data-droptype="inside-end"
              />
            )}
            {node.childrenOrder
              .map(childId => node.children.find(c => c.id === childId))
              .filter(Boolean) // Filter out any null or undefined in case a childId in order isn't found
              .map(child => renderNode(child, level + 1))}
          </div>
        )}
      </div>
    )
  }, [openNodes, selectedNode, onSelect, toggleNode, handleContextMenu, handleDragStart, handleDragOver, handleDrop, handleDragEnd, handleDragEnter, handleDragLeave, draggedOverNodeId, dropPositionState, draggedNode, nodes]); 
  return (
    <div className="node-tree">
      {nodes.map(node => renderNode(node))}
      
      {/* Context Menu */}
      {contextMenu.visible && (
        <div
          id="context-menu"
          className="absolute bg-white rounded-md shadow-lg border border-gray-200 py-1 z-50"
          style={{
            left: contextMenu.x,
            top: contextMenu.y,
            minWidth: '160px'
          }}
        >
          <button
            onClick={handleAddChild}
            className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
          >
            Add Child
          </button>
          <button
            onClick={handleDelete}
            className="block w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50"
          >
            Delete
          </button>
        </div>
      )}
    </div>
  )
}
