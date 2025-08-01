import React, { useEffect, useCallback, useState } from 'react'
import NodeItem from './NodeItem'
import { useNodeStore } from '../store/nodeStore';

export default function NodeTree({ nodes, selectedNode, onSelect, onCreate, onDelete }) {
  const { openNodes, toggleNode: toggleNodeStore } = useNodeStore();
  const [contextMenu, setContextMenu] = useState({ visible: false, x: 0, y: 0, node: null });

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

    return (
      <div key={node.id}>
        <NodeItem
          node={node}
          level={level}
          isExpanded={isExpanded}
          isSelected={isSelected}
          hasChildren={hasChildren}
          onSelect={onSelect}
          onToggle={toggleNode}
          onContextMenu={handleContextMenu}
        />
        
        {isExpanded && hasChildren && (
          <div className="ml-4">
            {node.children.map(child => renderNode(child, level + 1))}
          </div>
        )}
      </div>
    )
  }, [openNodes, selectedNode, onSelect, toggleNode, handleContextMenu]);

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
