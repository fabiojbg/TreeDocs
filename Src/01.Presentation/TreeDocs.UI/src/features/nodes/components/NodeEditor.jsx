import React, { useState, useEffect } from 'react'
import ReactQuill from 'react-quill'
import 'react-quill/dist/quill.snow.css'
import toast from 'react-hot-toast'

// NOTE: The "findDOMNode is deprecated" warning originates from the ReactQuill library
// (https://github.com/zenoamaro/react-quill/issues/1126).
// This is not an issue with the component's direct code, but an internal warning from the library itself.
// It typically requires an update to the ReactQuill package to resolve.
export default function NodeEditor({ node, onUpdate }) {
  const [name, setName] = useState(node?.name || '')
  const [contents, setContents] = useState(node?.contents || '')
  const [isEditingName, setIsEditingName] = useState(false)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (node) {
      setName(node.name || '')
      setContents(node.contents || '')
    }
  }, [node])

  const handleSave = async () => {
    if (!node) return
    
    setSaving(true)
    
    try {
      const updates = {
        name: name,
        contents: contents,
        parentId: node.parentId,
        childrenOrder: node.childrenOrder
      }
      
      const result = await onUpdate(node.id, updates);
      if (result && result.node) {
        setName(result.node.name);
        setContents(result.node.contents);
        toast.success('Node saved successfully!');
      }
    } catch (err) {
      console.error('Error saving node:', err)
    } finally {
      setSaving(false)
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
      if (result && result.node) {
        setName(result.node.name);
        setContents(result.node.contents);
        toast.success('Node name updated successfully!');
      }
      setIsEditingName(false)
    } catch (err) {
      console.error('Error saving node name:', err)
    } finally {
      setSaving(false)
    }
  }

  if (!node) {
    return null
  }

  return (
    <div className="h-full flex flex-col bg-white">
      {/* Header */}
      <div className="border-b border-gray-200 px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex-1">
            {isEditingName ? (
              <div className="flex items-center space-x-2">
                <input
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="text-lg font-semibold text-gray-900 border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                  onKeyPress={(e) => {
                    if (e.key === 'Enter') {
                      handleNameSave()
                    } else if (e.key === 'Escape') {
                      setName(node.name || '')
                      setIsEditingName(false)
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
                  }}
                  className="text-sm text-gray-500 hover:text-gray-700"
                >
                  Cancel
                </button>
              </div>
            ) : (
              <div className="flex items-center space-x-2">
                <h1
                  className="text-lg font-semibold text-gray-900 cursor-pointer hover:bg-gray-100 px-2 py-1 rounded"
                  onClick={() => setIsEditingName(true)}
                >
                  {node.name || 'Untitled'}
                </h1>
                <button
                  onClick={() => setIsEditingName(true)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                </button>
              </div>
            )}
          </div>
          
          <div className="flex items-center space-x-2">
            <button
              onClick={handleSave}
              disabled={saving}
              className="inline-flex items-center px-3 py-1 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
            >
              {saving ? (
                <>
                  <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Saving...
                </>
              ) : 'Save'}
            </button>
          </div>
        </div>
        
        <div className="mt-2 text-sm text-gray-500">
          <span>Created: {new Date(node.createdOn).toLocaleDateString()}</span>
          {node.updatedOn && (
            <span className="ml-4">Updated: {new Date(node.updatedOn).toLocaleDateString()}</span>
          )}
        </div>
      </div>
      
      {/* Content Editor */}
      <div className="flex-1 overflow-hidden">
        <ReactQuill
          value={contents}
          onChange={setContents}
          className="h-full"
          modules={{
            toolbar: [
              [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
              ['bold', 'italic', 'underline', 'strike'],
              [{ 'list': 'ordered'}, { 'list': 'bullet' }],
              [{ 'indent': '-1'}, { 'indent': '+1' }],
              [{ 'align': [] }],
              ['link', 'image'],
              ['clean']
            ]
          }}
          formats={[
            'header',
            'bold', 'italic', 'underline', 'strike',
            'list', 'bullet', 'indent',
            'align',
            'link', 'image'
          ]}
          placeholder="Start writing your note here..."

        onBlur={handleSave}
        />
      </div>
    </div>
  )
}
