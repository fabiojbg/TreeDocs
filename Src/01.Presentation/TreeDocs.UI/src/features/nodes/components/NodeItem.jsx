import React, { memo } from 'react'

const NodeItem = memo(({ node, level, isExpanded, isSelected, hasChildren, onSelect, onToggle, onContextMenu, onDragStart, onDragOver, onDrop, onDragEnd, onDragEnter, onDragLeave, draggedOverNodeId, dropPositionState }) => {
  const handleToggle = (e) => {
    e.stopPropagation()
    onToggle(node.id)
  }

  const handleSelect = () => {
    onSelect(node)
  }

  const handleRightClick = (e) => {
    onContextMenu(e, node)
  }

  return (
    <div
      className={`relative flex items-center py-1 px-2 rounded-md cursor-pointer hover:bg-gray-100 ${
        isSelected ? 'bg-indigo-50 border-l-4 border-indigo-500' : ''
      } ${draggedOverNodeId === node.id && dropPositionState === 'inside' ? 'border-2 border-blue-500' : ''}`}
      style={{ paddingLeft: `${level * 16 + 8}px` }}
      onClick={handleSelect}
      onContextMenu={handleRightClick}
      draggable="true"
      onDragStart={(e) => onDragStart(e, node)}
      onDragOver={(e) => onDragOver(e, node)}
      onDrop={(e) => onDrop(e, node)}
      onDragEnd={(e) => onDragEnd(e, node)}
      onDragEnter={(e) => onDragEnter(e, node)}
      onDragLeave={(e) => onDragLeave(e, node)}
    >
      {hasChildren && (
        <button
          onClick={handleToggle}
          className="mr-1 p-1 rounded hover:bg-gray-200 focus:outline-none"
        >
          <svg
            className={`w-4 h-4 text-gray-500 transform transition-transform ${
              isExpanded ? 'rotate-90' : ''
            }`}
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9 5l7 7-7 7"
            />
          </svg>
        </button>
      )}
      
      {!hasChildren && (
        <span className="mr-1 w-4" /> // Spacer to maintain alignment
      )}
      
      <span className="flex items-center">
        {node.nodeType === 'Folder' ? (
          <svg className="w-4 h-4 text-gray-500 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z" />
          </svg>
        ) : (
          <svg className="w-4 h-4 text-gray-500 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
        )}
        <span className="text-sm text-gray-700 truncate">{node.name}</span>
      </span>
    </div>
  )
})

export default NodeItem
