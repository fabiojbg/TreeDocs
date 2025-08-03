import React, { useCallback, useState, useEffect, useRef } from 'react';
import { useAuth } from 'src/features/auth/context/AuthContext';
import Header from 'src/components/layout/Header';
import NodeTree from '../components/NodeTree';
import NodeEditor from '../components/NodeEditor';
import { useNavigate } from 'react-router-dom';
import { useNodeManagement } from '../hooks/useNodeManagement';

export default function DashboardPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isEditorFocused, setIsEditorFocused] = useState(false);
  const nodeEditorRef = useRef(null); // Create a ref for the NodeEditor
  const isMounted = useRef(true); // To track component mount state

  const {
    nodes,
    selectedNode,
    loading,
    loadUserNodes,
    handleNodeSelect,
    handleNodeCreate,
    handleNodeUpdate,
    handleNodeDelete,
    handleNodeMove,
  } = useNodeManagement(user, nodeEditorRef);

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
      } 
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [selectedNode, handleNodeCreate, handleNodeDelete, isEditorFocused]);

  // Cleanup on unmount - save pending changes and prevent state updates
  useEffect(() => {
    return () => {
      isMounted.current = false;
      if (nodeEditorRef.current && nodeEditorRef.current.isDirty) {
        console.log('Dashboard unmounting, attempting to save pending editor changes.');
        nodeEditorRef.current.savePendingChanges().catch(err => {
          console.error('Error saving changes on Dashboard unmount:', err);
        });
      }
    };
  }, []);

  // Save before navigation (e.g., clicking a link in Header that might cause Dashboard to unmount)
  const handleNavigation = useCallback(async (navigationCallback) => {
    if (nodeEditorRef.current && nodeEditorRef.current.isDirty) {
      console.log('Navigation detected, attempting to save pending editor changes.');
      try {
        await nodeEditorRef.current.savePendingChanges();
      } catch (err) {
        console.error('Error saving changes before navigation:', err);
        // Optionally, you could ask for user confirmation here before navigating
      }
    }
    if (isMounted.current) {
      navigationCallback();
    }
  }, [nodeEditorRef]);

  const handleLogout = useCallback(async () => {
    logout();
    navigate('/login');
  }, [logout, navigate]);

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
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
      <Header user={user} onLogout={handleLogout} />
      
      <div className="flex h-screen pt-16">
        {/* Sidebar - Node Tree */}
        <div className="w-1/4 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 overflow-y-auto">
          <div className="p-4">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Your Notes</h2>
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
                onMoveNode={handleNodeMove} // Pass handleNodeMove to NodeTree
              />
            )}
          </div>
        </div>

        {/* Main Content - Node Editor */}
        <div className="flex-1 overflow-hidden bg-white dark:bg-gray-800">
          {selectedNode ? (
            <NodeEditor
              ref={nodeEditorRef} // Pass the ref here
              node={selectedNode}
              onUpdate={handleNodeUpdate}
              onEditorFocusChange={setIsEditorFocused}
            />
          ) : (
            <div className="flex items-center justify-center h-full">
              <div className="text-center">
                <svg className="mx-auto h-12 w-12 text-gray-400 dark:text-gray-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <h3 className="mt-2 text-sm font-medium text-gray-900 dark:text-white">No node selected</h3>
                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">Select a node from the sidebar to view or edit its contents.</p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
