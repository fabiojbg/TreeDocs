import React, { useCallback } from 'react';
import { useAuth } from 'src/features/auth/context/AuthContext';
import Header from 'src/components/layout/Header';
import NodeTree from '../components/NodeTree';
import NodeEditor from '../components/NodeEditor';
import { useNavigate } from 'react-router-dom';
import { useNodeManagement } from '../hooks/useNodeManagement';
import { useEffect } from 'react';

export default function DashboardPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  
  const {
    nodes,
    selectedNode,
    loading,
    loadUserNodes,
    handleNodeSelect,
    handleNodeCreate,
    handleNodeUpdate,
    handleNodeDelete,
    handleKeyboardNavigation, // Destructure the new handler
  } = useNodeManagement(user);

  const handleLogout = useCallback(() => {
    logout();
    navigate('/login');
  }, [logout, navigate]);

  useEffect(() => {
    const handleKeyDown = (event) => {
      if (event.ctrlKey && event.key === 'n') {
        event.preventDefault();
        handleNodeCreate(selectedNode?.id || null, 'New Node', 'Document', '');
      } else if (event.key === 'Delete' && selectedNode) {
        event.preventDefault();
        if (window.confirm(`Are you sure you want to delete "${selectedNode.name}" and all its children?`)) {
          handleNodeDelete(selectedNode.id);
        }
      } else if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(event.key)) {
        handleKeyboardNavigation(event);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [selectedNode, handleNodeCreate, handleNodeDelete, handleKeyboardNavigation]); // Add handleKeyboardNavigation to dependencies

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
