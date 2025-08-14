$(function() {
    // API_BASE_URL is now defined in api-config.js
    let quill;
    let selectedNodeId = null;
    let isContentModified = false; // New flag to track content changes
    let nodeToSelectAfterReload = null;

    // --- Content Management ---
    function showSaveChangesButton() {
        $('#saveChangesButton').show();
    }

    function hideSaveChangesButton() {
        $('#saveChangesButton').hide();
    }

    // --- Authentication and Initialization ---
    function checkAuth() {
        const authToken = localStorage.getItem('authToken');
        const userEmail = localStorage.getItem('userEmail');
        const userName = localStorage.getItem('userName'); // Assuming userName is also stored

        if (!authToken || !userEmail) {
            window.location.href = 'login.html'; // Redirect to login if not authenticated
            return;
        }
        $('#loggedInUserFullName').text(userName || userEmail); // Display full name
        $('#userInitial').text(userName ? userName.charAt(0).toUpperCase() : userEmail.charAt(0).toUpperCase()); // Display initial
        initDashboard();
    }

    // --- Resizable Splitter Logic ---
    const splitter = document.getElementById('splitter');
    const nodeTreeSidebar = document.getElementById('nodeTreeSidebar');
    const nodeEditorPane = document.getElementById('nodeEditorPane');
    const minTreeWidth = 290; // Minimum width for the tree sidebar
    let maxTreeWidth;         // Will be calculated dynamically based on half of the main content area width
    const splitterWidth = 8;  // Matches the CSS width of the splitter

    let isDragging = false;

    // Load saved width or set default
    const savedTreeWidth = localStorage.getItem('nodeTreeWidth');
    if (savedTreeWidth) {
        nodeTreeSidebar.style.width = savedTreeWidth;
    } else {
        nodeTreeSidebar.style.width = '300px'; // Default width
    }

    splitter.addEventListener('mousedown', function(e) {
        isDragging = true;
        // Calculate maxTreeWidth based on current main-content-area width
        const mainContentArea = document.getElementById('main-content-area');
        maxTreeWidth = mainContentArea.offsetWidth / 2;

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
        // Add a class to body to prevent text selection during dragging
        document.body.classList.add('no-select');
    });

    function handleMouseMove(e) {
        if (!isDragging) return;

        let newWidth = e.clientX; // Mouse X position is the new width of sidebar

        // Constrain width
        newWidth = Math.max(minTreeWidth, newWidth);
        newWidth = Math.min(maxTreeWidth, newWidth);

        nodeTreeSidebar.style.width = `${newWidth}px`;
        // The flex-grow-1 on nodeEditorPane will automatically adjust its width
    }

    function handleMouseUp() {
        isDragging = false;
        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleMouseUp);
        document.body.classList.remove('no-select');
        // Save the new width to local storage
        localStorage.setItem('nodeTreeWidth', nodeTreeSidebar.style.width);
    }

    // Adjust maxTreeWidth if window is resized
    window.addEventListener('resize', function() {
        const mainContentArea = document.getElementById('main-content-area');
        maxTreeWidth = mainContentArea.offsetWidth / 2;
        // Re-apply width if current width exceeds new max
        const currentWidth = parseInt(nodeTreeSidebar.style.width);
        if (currentWidth > maxTreeWidth) {
            nodeTreeSidebar.style.width = `${maxTreeWidth}px`;
            localStorage.setItem('nodeTreeWidth', nodeTreeSidebar.style.width);
        } else if (currentWidth < minTreeWidth) {
            // Also ensure it doesn't go below min on resize
            nodeTreeSidebar.style.width = `${minTreeWidth}px`;
            localStorage.setItem('nodeTreeWidth', nodeTreeSidebar.style.width);
        }
    });

    $('#logoutButton').on('click', function() {
        localStorage.removeItem('authToken');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userName'); // Clear userName on logout
        toastr.info('Logged out successfully.');
        window.location.href = 'login.html';
    });

    $('#refreshTreeButton').on('click', function() {
        loadNodes();
        toastr.info('Tree refreshed!');
    });

    $('#saveChangesButton').on('click', async function() {
        if (selectedNodeId && isContentModified) {
            await saveNodeContents(selectedNodeId, quill.root.innerHTML, true); // Pass true to indicate manual save
        } else {
            toastr.info('No changes to save.');
        }
    });

    function initDashboard() {
        // Initialize Quill editor
        quill = new Quill('#nodeEditor', {
            theme: 'snow',
            placeholder: 'Start typing your note here...',
            modules: {
                toolbar: [
                    ['bold', 'italic', 'underline', 'strike'],
                    ['blockquote', 'code-block'],
                    [{ 'list': 'ordered'}, { 'list': 'bullet' }],
                    [{ 'script': 'sub'}, { 'script': 'super' }],
                    [{ 'indent': '-1'}, { 'indent': '+1' }],
                    [{ 'direction': 'rtl' }],
                    [{ 'size': ['small', false, 'large', 'huge'] }],
                    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                    [{ 'color': [] }, { 'background': [] }],
                    [{ 'font': [] }],
                    [{ 'align': [] }],
                    ['link', 'image', 'video'],
                    ['clean']
                ]
            }
        });

        // Set up "Change Password" button click handler
        $('#changePasswordButton').on('click', function() {
            const changePasswordModal = new bootstrap.Modal(document.getElementById('changePasswordModal'));
            changePasswordModal.show();
        });

        // Set up "Edit Node Name" icon click handler
        $('#editNodeNameIcon').on('click', function() {
            if (selectedNodeId) {
                const currentNodeName = $('#noteTitle').text();
                showNodeNameModal('Rename Node', currentNodeName).then(newName => {
                    if (newName && newName !== '' && newName !== currentNodeName) {
                        const tree = $('#nodeTree').jstree(true);
                        tree.set_text(selectedNodeId, newName);
                        updateNodeName(selectedNodeId, newName);
                    }
                });
            } else {
                toastr.info('Please select a node to rename.');
            }
        });

        // Set up "Save Password" button handler
        $('#savePasswordButton').on('click', async function() {
            const oldPassword = $('#oldPasswordInput').val();
            const newPassword = $('#newPasswordInput').val();
            const confirmNewPassword = $('#confirmNewPasswordInput').val();

            if (!oldPassword || !newPassword || !confirmNewPassword) {
                toastr.error('All password fields are required.');
                return;
            }

            if (newPassword !== confirmNewPassword) {
                toastr.error('New password and confirmation do not match.');
                $('#newPasswordInput').val('');
                $('#confirmNewPasswordInput').val('');
                return;
            }

            // You might want to add more robust password validation (e.g., min length, complexity) here

            try {
                const authToken = localStorage.getItem('authToken');
                const userEmail = localStorage.getItem('userEmail'); // Get userEmail for the payload

                const response = await fetch(`${API_BASE_URL}/api/User/ChangePassword`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${authToken}`
                    },
                    body: JSON.stringify({
                        UserEmail: userEmail, // Use userEmail as identifier
                        OldPassword: oldPassword,
                        NewPassword: newPassword
                    })
                });

                if (response.ok) {
                    toastr.success('Password changed successfully!');
                    // Clear fields and hide modal
                    $('#oldPasswordInput').val('');
                    $('#newPasswordInput').val('');
                    $('#confirmNewPasswordInput').val('');
                    bootstrap.Modal.getInstance(document.getElementById('changePasswordModal')).hide();
                } else {
                    const errData = await response.json();
                    toastr.error(errData._Message || 'Failed to change password. Please check your old password.');
                }
            } catch (err) {
                toastr.error('Network error during password change.');
            }
        });

        // Initialize jsTree
        $('#nodeTree').jstree({
            'core': {
                'data': [], // Will be loaded dynamically
                'check_callback': true, // Allow drag & drop, etc.
                'themes': {
                    'name': 'default',
                    'responsive': true,
                    'variant': 'small' // Use 'small' variant for a compact look
                },
                'sort': function (a, b) { // Custom sort function to respect ChildrenOrder
                    const tree = $('#nodeTree').jstree(true);
                    const nodeA = tree.get_node(a);
                    const nodeB = tree.get_node(b);

                    const parentId = nodeA.parent;
                    const parentNode = tree.get_node(parentId);

                    if (parentNode && parentNode.data && parentNode.data.childrenOrder) {
                        const order = parentNode.data.childrenOrder;
                        const indexA = order.indexOf(nodeA.id);
                        const indexB = order.indexOf(nodeB.id);

                        if (indexA !== -1 && indexB !== -1) {
                            return indexA - indexB;
                        }
                        if (indexA !== -1) return -1;
                        if (indexB !== -1) return 1;
                    }
                    return nodeA.text.localeCompare(nodeB.text); // Default alphabetical sort
                }
            },
            'plugins': ['dnd', 'contextmenu', 'wholerow'], // Drag & Drop, Context Menu, Whole Row selection
            'contextmenu': {
                'items': customMenu
            }
        });

        loadNodes();
        setupJSTreeEvents();
        setupQuillAutoSave();
        updateDarkMode(true); // Force dark mode as default
        setupKeyboardShortcuts();
    }

    // Custom context menu for jsTree
    function customMenu(node) {
        var tree = $('#nodeTree').jstree(true);
        var items = {
            'create': {
                'separator_before': false,
                'separator_after': true,
                'label': 'Create New Node',
                'action': function (obj) {
                    // Default new node name
                    var newName = 'New Node';
                    showNodeNameModal('Create New Node', newName).then(nodeName => {
                        if (nodeName) {
                            createNode(node.id, nodeName);
                        }
                    });
                }
            },
            'rename': {
                'separator_before': false,
                'separator_after': false,
                'label': 'Rename',
                'action': function (obj) {
                    showNodeNameModal('Rename Node', node.text).then(newName => {
                        if (newName && newName !== node.text) {
                            tree.set_text(node.id, newName); // Update text in jstree
                            updateNodeName(node.id, newName);
                        }
                    });
                }
            },
            'delete': {
                'separator_before': false,
                'separator_after': false,
                'label': 'Delete',
                'action': function (obj) {
                    showDeleteConfirmationModal().then(confirmed => {
                        if (confirmed) {
                            deleteNode(node.id);
                        }
                    });
                }
            }
        };
        // For the root node created by the system, we might want to disable delete/rename
        // if (node.parent === '#') {
        //     delete items.delete;
        //     delete items.rename;
        // }
        // This is a decision for a real root node scenario. For now, all are fair game.
        return items;
    }

    // Helper function to show Bootstrap modal for node name input
    function showNodeNameModal(title, defaultValue = '') {
        return new Promise(resolve => {
            const modal = new bootstrap.Modal(document.getElementById('nodeNameModal'));
            const nodeNameInput = $('#nodeNameInput');
            
            $('#nodeNameModalLabel').text(title);
            nodeNameInput.val(defaultValue); // Set default value

            // Clear previous event listeners
            $('#saveNodeNameButton').off('click');
            nodeNameInput.off('keydown');

            $('#saveNodeNameButton').on('click', function() {
                modal.hide();
                resolve(nodeNameInput.val());
            });

            nodeNameInput.on('keydown', function(event) {
                if (event.key === 'Enter') {
                    event.preventDefault(); // Prevent default form submission
                    $('#saveNodeNameButton').click(); // Trigger save button click
                }
            });

            // Handle modal close (via X button or clicking outside)
            $('#nodeNameModal').on('hidden.bs.modal', function () {
                resolve(null); // Resolve with null if modal is closed without saving
                $('#nodeNameModal').off('hidden.bs.modal'); // Remove listener to prevent multiple calls
            });

            modal.show();
            // Handle modal shown event to focus the input
            $('#nodeNameModal').on('shown.bs.modal', function () {
                nodeNameInput.focus(); // Focus on input when modal is fully shown
            });

            modal.show();
        });
    }

    // Helper function to show Bootstrap modal for delete confirmation
    function showDeleteConfirmationModal() {
        return new Promise(resolve => {
            const modal = new bootstrap.Modal(document.getElementById('deleteConfirmationModal'));
            
            // Clear previous event listeners
            $('#confirmDeleteButton').off('click');

            $('#confirmDeleteButton').on('click', function() {
                modal.hide();
                resolve(true);
            });

            // Handle modal close (via X button or clicking outside)
            $('#deleteConfirmationModal').on('hidden.bs.modal', function () {
                resolve(false); // Resolve with false if modal is closed without confirming
                $('#deleteConfirmationModal').off('hidden.bs.modal'); // Remove listener
            });

            modal.show();
        });
    }

    // --- jsTree Events (Drag & Drop, Rename) ---
    function setupJSTreeEvents() {
        $('#nodeTree').on('rename_node.jstree', function (e, data) {
            // The rename action is now handled by the custom context menu item/keyboard shortcut
            // We just need to update the node name here if jstree's built-in rename is used directly
            // However, since we're replacing the direct edit with a modal, this event often won't fire for manual edits.
            // If it fires programmatically from set_text, we still want to save.
            updateNodeName(data.node.id, data.text);
        }).on('move_node.jstree', async function (e, data) {
            const tree = $('#nodeTree').jstree(true);
            const movedNodeId = data.node.id;
            const newParentId = data.parent === '#' ? null : data.parent;
            const oldParentId = data.old_parent === '#' ? null : data.old_parent;

            let success = true;

            // 1. Update the moved node's parent
            const updateMovedNodePayload = { NodeId: movedNodeId, ParentId: newParentId };
            success = await sendNodeUpdate(updateMovedNodePayload);
            if (!success) {
                loadNodes(); // Revert tree on failure
                return;
            }

            // 2. Update the new parent's children order
            if (newParentId !== null) {
                const newParentNode = tree.get_node(data.parent);
                const newNodeChildrenOrder = newParentNode.children;
                if (newNodeChildrenOrder.length > 0) {
                    const updateNewParentOrderPayload = { NodeId: newParentId, ChildrenOrder: newNodeChildrenOrder };
                    success = await sendNodeUpdate(updateNewParentOrderPayload);
                    if (!success) {
                        loadNodes(); // Revert tree on failure
                        return;
                    }
                }
            }

            // 3. If parent changed, update the old parent's children order
            if (oldParentId !== null && oldParentId !== newParentId) {
                const oldParentNode = tree.get_node(data.old_parent);
                const oldNodeChildrenOrder = oldParentNode.children;
                // Only send update if there are still children to order in the old parent
                if (oldNodeChildrenOrder.length > 0) {
                    const updateOldParentOrderPayload = { NodeId: oldParentId, ChildrenOrder: oldNodeChildrenOrder };
                    success = await sendNodeUpdate(updateOldParentOrderPayload);
                    if (!success) {
                        loadNodes(); // Revert tree on failure
                        return;
                    }
                }
            }

            if (success) {
                toastr.success('Node moved/reordered successfully!');
                loadNodes(); // Reload tree to reflect all changes and ensure sync
            }
        }).on('refresh.jstree', function() { // Changed to 'refresh.jstree'
            setTimeout(function() { // Add a small delay to ensure DOM is fully updated
                if( nodeToSelectAfterReload)
                {
                    const tree = $('#nodeTree').jstree(true);
                    if( tree )
                        tree.select_node(nodeToSelectAfterReload);
                    nodeToSelectAfterReload = null;
                }
            }, 0); // Delay by 0ms to defer execution
        });
    }

    async function createNode(parentId, name) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/nodes`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                },
                body: JSON.stringify({ ParentId: parentId, Name: name, NodeType: 0 }) // Assuming NodeType 0 is default/document
            });
            if (response.ok) {
                const newNode = await response.json();
                toastr.success('Node created successfully!');
                nodeToSelectAfterReload = newNode.Id;
                loadNodes(); // Reload tree to reflect changes
                // Potentially select the new node
                // $('#nodeTree').jstree(true).select_node(newnode.Id);
            } else {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to create node.');
            }
        } catch (err) {
            toastr.error('Network error during node creation.');
        }
    }

    async function updateNodeName(nodeId, newName) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/nodes`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                },
                body: JSON.stringify({ NodeId: nodeId, Name: newName })
            });
            if (response.ok) {
                toastr.success('Node renamed successfully!');
                $('#noteTitle').text(newName); // Update the note header with the new name
            } else {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to rename node.');
                // Revert JSTree if API call fails
                loadNodes();
            }
        } catch (err) {
            toastr.error('Network error during node rename.');
            loadNodes(); // Revert JSTree
        }
    }

    async function deleteNode(nodeId) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/nodes/${nodeId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });
            if (response.ok) {
                toastr.success('Node deleted successfully!');
                $('#nodeTree').jstree(true).delete_node(nodeId); // Remove from tree locally
                selectedNodeId = null; // Clear selected node
                quill.setContents(''); // Clear editor
            } else {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to delete node.');
            }
        } catch (err) {
            toastr.error('Network error during node deletion.');
        }
    }

    async function sendNodeUpdate(payload) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/nodes`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to update node.');
                loadNodes(); // Revert tree to server state on error
                return false; // Indicate failure
            }
            return true; // Indicate success

        } catch (err) {
            toastr.error('Network error during node update.');
            loadNodes(); // Revert tree to server state on error
            return false; // Indicate failure
        }
    }

    // --- Keyboard Shortcuts ---
    function setupKeyboardShortcuts() {
        $(document).on('keydown', function(e) {
            // Alt + N: Create new child node
            if (e.altKey && e.key === 'n' && selectedNodeId) {
                e.preventDefault();
                $('#nodeTree').jstree(true)._invoke_safely(function() {
                    $('#nodeTree').jstree(true).create_node(selectedNodeId, 'New Child');
                });
            }
            // Alt + R: Rename selected node
            if (e.altKey && e.key === 'r' && selectedNodeId) {
                e.preventDefault();
                const tree = $('#nodeTree').jstree(true);
                const node = tree.get_node(selectedNodeId);
                showNodeNameModal('Rename Node', node.text).then(newName => {
                    if (newName && newName !== node.text) {
                        tree.set_text(node.id, newName);
                        updateNodeName(node.id, newName);
                    }
                });
            }
            // Alt + D: Delete selected node
            if (e.altKey && e.key === 'd' && selectedNodeId) {
                e.preventDefault();
                showDeleteConfirmationModal().then(confirmed => {
                    if (confirmed) {
                        deleteNode(selectedNodeId);
                    }
                });
            }
            // Ctrl + S: Save (auto-save handles this, but can trigger manually as well)
            if (e.ctrlKey && e.key === 's') {
                e.preventDefault();
                if (selectedNodeId) {
                    saveNodeContents(selectedNodeId, quill.root.innerHTML);
                } else {
                    toastr.info('Select a node to save its content.');
                }
            }
        });
    }

    // --- Quill Content Change Tracking & Save Button Visibility ---
    function setupQuillAutoSave() {
        quill.on('text-change', function(delta, oldDelta, source) {
            if (source === 'user') {
                isContentModified = true; // Mark content as modified
                showSaveChangesButton(); // Show the save button
            }
        });
    }

    // --- Node Tree (jsTree) Operations ---
    async function loadNodes() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/nodes`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                const jstreeData = convertNodesToJSTreeFormat(data.Nodes);
                if( !nodeToSelectAfterReload)
                    nodeToSelectAfterReload = jstreeData.length>0 ? jstreeData[0].id : null;
                $('#nodeTree').jstree(true).settings.core.data = jstreeData;
                $('#nodeTree').jstree(true).refresh();
            } else {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to load nodes.');
                if (response.status === 401) { // Unauthorized
                    // setTimeout(() => window.location.href = 'login.html', 1500);
                }
            }
        } catch (err) {
            toastr.error('Network error while loading nodes.');
            // setTimeout(() => window.location.href = 'login.html', 1500);
        }
    }

    // Function to format dates
    function formatDate(dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', { year: 'numeric', month: 'numeric', day: 'numeric' });
    }

    function convertNodesToJSTreeFormat(nodes) {
        const jstreeNodes = [];

        function processNode(node, parentId) {
            let iconClass = 'jstree-themeicon-file'; // Default to file icon

            // Check if NodeType exists and if it's 1 (Folder)
            if (node.NodeType !== undefined && node.NodeType === 1) { // Assuming NodeType 1 is Folder
                iconClass = 'jstree-themeicon-folder';
            }

            jstreeNodes.push({
                id: node.Id,
                parent: parentId,
                text: node.Name,
                state: { opened: true }, // Keep nodes expanded by default
                icon: iconClass, // Set custom icon
                data: {
                    originalNode: node, // Store full original node data
                    childrenOrder: node.ChildrenOrder // Store children order for custom sorting
                }
            });

            if (node.Children && node.Children.length > 0) {
                node.Children.forEach(child => {
                    processNode(child, node.Id);
                });
            }
        }

        // Assuming 'nodes' here is the top-level array received from data.Nodes
        // and these are the true root nodes that have no parent, or their parentId is null
        nodes.forEach(node => {
            processNode(node, node.ParentId || '#'); // Pass '#' for top-level roots
        });

        return jstreeNodes;
    }


    $('#nodeTree').on('select_node.jstree', async function(e, data) {
        // Before changing selectedNodeId, check if the previous node's content was modified
        if (selectedNodeId && isContentModified) {
            await saveNodeContents(selectedNodeId, quill.root.innerHTML, false); // Auto-save on node change, false for no toast
        }

        selectedNodeId = data.node.id;
        quill.enable(false); // Disable editor while loading
        quill.setText('Loading note content...'); // Show loading message

        // Hide save button and reset flag on new node selection
        hideSaveChangesButton();
        isContentModified = false;

        // Clear previous note header details
        $('#noteTitle').text('Loading...');
        $('#nodeCreatedOn').text('N/A');
        $('#nodeUpdatedOn').text('N/A');

        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/nodes/${selectedNodeId}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });

            if (response.ok) {
                const nodeData = await response.json();
                const node = nodeData.Node;

                $('#noteTitle').text(node.Name);
                $('#nodeCreatedOn').text(formatDate(node.CreatedOn));
                $('#nodeUpdatedOn').text(formatDate(node.UpdatedOn));

                quill.setContents(quill.clipboard.convert(node.Contents || ''));
                quill.enable(true);
            } else {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to load note content.');
                quill.setText('Error loading content.');
            }
        } catch (err) {
            toastr.error('Network error loading note content.');
            quill.setText('Network error.');
        }
    });

    async function saveNodeContents(nodeId, contents, isManualSave = false) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/nodes`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                },
                body: JSON.stringify({ NodeId: nodeId, NodeContents: contents })
            });

            if (response.ok) {
                isContentModified = false; // Reset flag after successful save
                hideSaveChangesButton(); // Hide the button after saving
                if (isManualSave) { // Only show toast for manual saves
                    toastr.success('Note saved successfully!');
                }
            } else {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to save note.');
            }
        } catch (err) {
            toastr.error('Network error during save.');
        }
    }

    // --- Dark Mode Toggle ---
    function updateDarkMode(enable) {
        // Always force dark mode if 'enable' is true, otherwise respect localStorage/intended behaviour.
        // For this task, we want to always enable it and hide the toggle.
        if (enable) {
            $('body').addClass('dark-mode').removeClass('bg-light');
            $('.navbar').removeClass('navbar-light bg-light').addClass('navbar-dark bg-dark');
            const nodeTreeSidebar = $('#nodeTreeSidebar');
            nodeTreeSidebar.removeClass('bg-light text-dark').addClass('bg-dark text-white');
        } else {
            // This 'else' block effectively becomes unreachable or irrelevant if we always pass 'true'
            // However, keeping it for logical completeness of the original function's intent if it were ever called with 'false'
            $('body').removeClass('dark-mode').addClass('bg-light');
            $('.navbar').removeClass('navbar-dark bg-dark').addClass('navbar-light bg-light');
            const nodeTreeSidebar = $('#nodeTreeSidebar');
            nodeTreeSidebar.removeClass('bg-dark text-white').addClass('bg-light text-dark');
        }
        localStorage.setItem('darkMode', enable); // Still store preference in localStorage
    }

    // This function is now redundant as we force dark mode and hide the button,
    // but leaving it in its original place for file context.
    function updateDarkModeToggleIcon(isDarkMode) {
        const iconElement = $('#darkModeToggle i');
        if (isDarkMode) {
          iconElement.removeClass('bi-moon-fill').addClass('bi-sun-fill');
        } else {
          iconElement.removeClass('bi-sun-fill').addClass('bi-moon-fill');
        }
    }

    // --- Document Ready ---
    $(document).ready(function() {
        // Initial check for authentication and dashboard setup
        checkAuth();
    });
});
