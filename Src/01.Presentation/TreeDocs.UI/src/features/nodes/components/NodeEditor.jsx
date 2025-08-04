import React, { useState, useEffect, useRef, useCallback, useImperativeHandle, forwardRef } from 'react'
import ReactQuill from 'react-quill'
import Quill from 'quill'
import 'react-quill/dist/quill.snow.css'
import './NodeEditor.css'
import toast from 'react-hot-toast'

// Register custom fonts with Quill
const Font = Quill.import('formats/font');
Font.whitelist = [
  'arial', 'times-new-roman', 'courier-new', 'georgia', 
  'verdana', 'comic-sans-ms', 'impact', 'trebuchet-ms', 'lucida-console'
];
Quill.register(Font, true);

// NOTE: The "findDOMNode is deprecated" warning originates from the ReactQuill library
// (https://github.com/zenoamaro/react-quill/issues/1126).
// This is not an issue with the component's direct code, but an internal warning from the library itself.
// It typically requires an update to the ReactQuill package to resolve.
export default forwardRef(function NodeEditor({ node, onUpdate, onEditorFocusChange }, ref) {
  const [name, setName] = useState(node?.name || '')
  const [contents, setContents] = useState(node?.contents || '')
  const [isEditingName, setIsEditingName] = useState(false)
  const [saving, setSaving] = useState(false)
  const [isDirty, setIsDirty] = useState(false)
  const reactQuillRef = useRef(null)
  const isMounted = useRef(true)

  useEffect(() => {
    if (node) {
      setName(node.name || '')
      setContents(node.contents || '')
      setIsDirty(false)
    }
    return () => {
      isMounted.current = false
    }
  }, [node])

  useImperativeHandle(ref, () => ({
    savePendingChanges: savePendingChanges,
    isDirty: isDirty,
    // Add any other methods/states you want to expose to the parent
  }))

  // Cleanup on unmount - save pending changes
  useEffect(() => {
    return () => {
      if (isMounted.current && isDirty && node) {
        // Attempt to save any pending changes when component unmounts
        savePendingChanges()
      }
    }
  }, [isDirty, node])

  const checkIfContentsChanged = useCallback(() => {
    if (!node) return false
    return contents !== node.contents || name !== node.name
  }, [contents, name, node])

  const savePendingChanges = useCallback(async () => {
    if (!node || !checkIfContentsChanged() || saving) return

    try {
      const updates = {
        name: name,
        contents: contents,
        parentId: node.parentId,
        childrenOrder: node.childrenOrder
      }
      
      const result = await onUpdate(node.id, updates);
      if (result && result.node && isMounted.current) {
        setIsDirty(false)
        console.log('Auto-saved pending changes on unmount')
      }
    } catch (err) {
      console.error('Error auto-saving pending changes:', err)
    }
  }, [node, name, contents, onUpdate, saving, checkIfContentsChanged])

  const handleSave = async () => {
    if (!node || !checkIfContentsChanged() || saving) return
    
    setSaving(true)
    
    try {
      const updates = {
        name: name,
        contents: contents,
        parentId: node.parentId,
        childrenOrder: node.childrenOrder
      }
      
      const result = await onUpdate(node.id, updates);
      if (result && result.node && isMounted.current) {
        setName(result.node.name);
        setContents(result.node.contents);
        setIsDirty(false);
        toast.success('Node saved successfully!');
      }
    } catch (err) {
      console.error('Error saving node:', err)
    } finally {
      if (isMounted.current) {
        setSaving(false)
      }
    }
  }

  const handleNameSave = async () => {
    if (!node || name.trim() === node.name) {
      setIsEditingName(false)
      return
    }
    
    setSaving(true)
    
    try {
      const updates = {
        name: name.trim(),
        contents: contents,
        parentId: node.parentId,
        childrenOrder: node.childrenOrder
      }
      
      const result = await onUpdate(node.id, updates);
      if (result && result.node && isMounted.current) {
        setName(result.node.name);
        setContents(result.node.contents);
        setIsDirty(false);
        toast.success('Node name updated successfully!');
      }
      setIsEditingName(false)
    } catch (err) {
      console.error('Error saving node name:', err)
    } finally {
      if (isMounted.current) {
        setSaving(false)
      }
    }
  }

  // Expose save function to parent component via ref
  useEffect(() => {
    if (reactQuillRef.current) {
      reactQuillRef.current.saveBeforeUnmount = savePendingChanges
    }
  }, [savePendingChanges])

  const handleContentsChange = useCallback((value) => {
    setContents(value)
    setIsDirty(true);
  }, [])

  const handleNameChange = useCallback((value) => {
    setName(value)
    setIsDirty(true);
  }, [])

  // Recalculate dirty state when name or contents change, *after* they've been set
  useEffect(() => {
    setIsDirty(checkIfContentsChanged());
  }, [name, contents, checkIfContentsChanged]);

  if (!node) {
    return null
  }

  return (
<div className="h-full flex flex-col bg-white dark:bg-gray-900">
      {/* Header */}
      <div className="border-b border-gray-200 px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex-1">
            {isEditingName ? (
              <div className="flex items-center space-x-2">
                <input
                  type="text"
                  value={name}
                  onChange={(e) => handleNameChange(e.target.value)}
                  className="text-lg font-semibold text-gray-900 border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                  onKeyPress={(e) => {
                    if (e.key === 'Enter') {
                      handleNameSave()
                    } else if (e.key === 'Escape') {
                      setName(node.name || '')
                      setIsEditingName(false)
                      setIsDirty(false)
                    }
                  }}
                  autoFocus
                />
                <button
                  onClick={handleNameSave}
                  disabled={saving}
                  className="text-sm text-indigo-600 hover:text-indigo-500 disabled:opacity-50"
                >
                  Save
                </button>
                <button
                  onClick={() => {
                    setName(node.name || '')
                    setIsEditingName(false)
                    setIsDirty(false)
                  }}
                  className="text-sm text-gray-500 hover:text-gray-700"
                >
                  Cancel
                </button>
              </div>
            ) : (
              <div className="flex items-center space-x-2">
                  <h1
                    className="text-lg font-semibold text-gray-900 dark:text-white cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-700 px-2 py-1 rounded"
                    onClick={() => setIsEditingName(true)}
                  >
                    {node.name || 'Untitled'}
                    {isDirty && <span className="text-xs text-orange-500 dark:text-orange-400 ml-2">*</span>}
                  </h1>
                  <button
                    onClick={() => setIsEditingName(true)}
                    className="text-gray-400 hover:text-gray-600 dark:text-gray-200 dark:hover:text-white"
                >
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                </button>
              </div>
            )}
          </div>
          
          <div className="flex items-center space-x-2">
            {isDirty && (
              <button
                onClick={handleSave}
                disabled={saving}
                className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
              >
                {saving ? 'Saving...' : 'Save Changes'}
              </button>
            )}
          </div>
        </div>
        
        <div className="mt-2 text-sm text-gray-400">
          <span>Created: {new Date(node.createdOn).toLocaleDateString()}</span>
          {node.updatedOn && (
            <span className="ml-4">Updated: {new Date(node.updatedOn).toLocaleDateString()}</span>
          )}
        </div>
      </div>
      
      {/* Content Editor */}
      <div className="flex-1 overflow-hidden">
        <ReactQuill
          ref={reactQuillRef}
          value={contents}
          onChange={handleContentsChange}
          className="h-full"
          modules={{
            toolbar: [
              [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
              [{ 'font': ['arial', 'times-new-roman', 'courier-new', 'georgia', 'verdana', 'comic-sans-ms', 'impact', 'trebuchet-ms', 'lucida-console'] }],
              [{ 'size': ['small', false, 'large', 'huge'] }],
              [{ 'color': [] }, { 'background': [] }], 
              ['bold', 'italic', 'underline', 'strike'],
              [{ 'list': 'ordered'}, { 'list': 'bullet' }],
              [{ 'indent': '-1'}, { 'indent': '+1' }],
              [{ 'align': [] }],
              ['link', 'image', 'code-block', 'blockquote'],
              ['clean']
            ]
          }}
          formats={[
            'header',
            'font',
            'size',
            'color', 'background',
            'bold', 'italic', 'underline', 'strike',
            'list', 'bullet', 'indent',
            'align',
            'link', 'image', 'code-block', 'blockquote'
          ]}
          placeholder="Start writing your note here..."
          onFocus={() => onEditorFocusChange && onEditorFocusChange(true)}
          onBlur={() => {
            onEditorFocusChange && onEditorFocusChange(false);
            handleSave();
          }}
          onKeyDown={(event) => {
            if (event.key === 'Delete') {
              event.stopPropagation();
            }
          }}
        />
      </div>
    </div>
  )
})
