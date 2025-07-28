import React, { useState, useEffect } from 'react' // Import useEffect
import NodeItem from './NodeItem'

export default function NodeTree({ nodes, selectedNode, onSelect, onCreate, onDelete }) {
  // Initialize expandedNodes from localStorage
  const [expandedNodes, setExpandedNodes] = useState(() => {
    try {
      const storedExpandedNodes = localStorage.getItem('tree-expanded-nodes');
      return storedExpandedNodes ? new Set(JSON.parse(storedExpandedNodes)) : new Set();
    } catch (error) {
      console.error("Failed to parse expanded nodes from localStorage:", error);
      return new Set();
    }
  });
  const [contextMenu, setContextMenu] = useState({ visible: false, x: 0, y: 0, node: null })

  const toggleNode = (nodeId) => {
    setExpandedNodes(prevExpanded => {
      const newExpanded = new Set(prevExpanded);
      if (newExpanded.has(nodeId)) {
        newExpanded.delete(nodeId);
      } else {
        newExpanded.add(nodeId);
      }
      localStorage.setItem('tree-expanded-nodes', JSON.stringify(Array.from(newExpanded)));
      return newExpanded;
    });
  };

  const handleContextMenu = (e, node) => {
    e.preventDefault()
    setContextMenu({
      visible: true,
      x: e.clientX,
      y: e.clientY,
      node: node
    })
  }

  const handleAddChild = () => {
    if (contextMenu.node) {
      onCreate(contextMenu.node.id, 'New Child', 'Document', '')
      setContextMenu({ visible: false, x: 0, y: 0, node: null })
    }
  }

  const handleDelete = () => {
    if (contextMenu.node) {
      // Show confirmation dialog
      if (window.confirm(`Are you sure you want to delete "${contextMenu.node.name}" and all its children?`)) {
        onDelete(contextMenu.node.id)
      }
      setContextMenu({ visible: false, x: 0, y: 0, node: null })
    }
  }

  const handleClickOutside = (e) => {
    if (contextMenu.visible) {
      // Check if click is outside context menu
      const menu = document.getElementById('context-menu')
      if (menu && !menu.contains(e.target)) {
        setContextMenu({ visible: false, x: 0, y: 0, node: null })
      }
    }
  }

  // Close context menu when clicking anywhere
  React.useEffect(() => {
    document.addEventListener('click', handleClickOutside)
    return () => {
      document.removeEventListener('click', handleClickOutside)
    }
  }, [contextMenu.visible])

  const renderNode = (node, level = 0) => {
    const isExpanded = expandedNodes.has(node.id)
    const isSelected = selectedNode && selectedNode.id === node.id
    const hasChildren = node.children && node.children.length > 0

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
  }

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
