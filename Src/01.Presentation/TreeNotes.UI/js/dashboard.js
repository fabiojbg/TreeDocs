$(function() {
    // API_BASE_URL is now defined in api-config.js
    let quill;
    let selectedNodeId = null;
    let isContentModified = false; // New flag to track content changes
    let nodeToSelectAfterReload = null;
    let nodeTreeInstanceMobile = null; // Store jstree instance for mobile
    let nodeTreeInstanceDesktop = null; // Store jstree instance for desktop

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
        
        // Populate profile modal fields immediately on auth check
        $('#profileNameInput').val(userName || userEmail);
        $('#profileEmailInput').val(userEmail);

        initDashboard();
    }

    // --- Resizable Splitter Logic ---
    const splitter = document.getElementById('splitter');
    const nodeTreeSidebar = document.getElementById('nodeTreeSidebar'); // Desktop sidebar
    const nodeEditorPane = document.getElementById('nodeEditorPane');
    const minTreeWidth = 290; // Minimum width for the tree sidebar
    let maxTreeWidth;         // Will be calculated dynamically based on half of the main content area width
    const splitterWidth = 8;  // Matches the CSS width of the splitter

    let isDragging = false;

    function initSplitter() {
        // Only enable splitter on large screens where desktop sidebar is visible
        if (window.innerWidth >= 992) { // Corresponds to Bootstrap's 'lg' breakpoint
            // Load saved width or set default
            const savedTreeWidth = localStorage.getItem('nodeTreeWidth');
            if (savedTreeWidth) {
                nodeTreeSidebar.style.width = savedTreeWidth;
            } else {
                nodeTreeSidebar.style.width = '300px'; // Default width
            }

            splitter.addEventListener('mousedown', startDragging);
            // Add the resize listener only once, outside the if/else, to keep it persistent.
            // Removed: window.removeEventListener('resize', handleResizeForSplitter);
            // Removed: window.addEventListener('resize', handleResizeForSplitter);
        } else {
            // Disable dragging if on mobile/small screen
            splitter.removeEventListener('mousedown', startDragging);
            // Removed: window.removeEventListener('resize', handleResizeForSplitter);
        }
    }

    // Ensure the resize listener is always attached to the window
    // This listener will call initSplitter() on every resize, letting it re-evaluate
    // whether the splitter should be active or not.
    window.removeEventListener('resize', handleResizeForSplitter); // Remove if already exists to prevent duplicates
    window.addEventListener('resize', handleResizeForSplitter); // Add it once globally

    function startDragging(e) {
        isDragging = true;
        const mainContentArea = document.getElementById('main-content-area');
        maxTreeWidth = mainContentArea.offsetWidth / 2;

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', stopDragging);
        document.body.classList.add('no-select');
    }

    function handleMouseMove(e) {
        if (!isDragging) return;

        let newWidth = e.clientX; 

        newWidth = Math.max(minTreeWidth, newWidth);
        newWidth = Math.min(maxTreeWidth, newWidth);

        nodeTreeSidebar.style.width = `${newWidth}px`;
    }

    function stopDragging() {
        isDragging = false;
        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', stopDragging);
        document.body.classList.remove('no-select');
        localStorage.setItem('nodeTreeWidth', nodeTreeSidebar.style.width);
    }

    function handleResizeForSplitter() {
        initSplitter(); // Re-evaluate splitter state on resize
        const mainContentArea = document.getElementById('main-content-area');
        maxTreeWidth = mainContentArea.offsetWidth / 2;
        const currentWidth = parseInt(nodeTreeSidebar.style.width);
        if (currentWidth > maxTreeWidth) {
            nodeTreeSidebar.style.width = `${maxTreeWidth}px`;
            localStorage.setItem('nodeTreeWidth', nodeTreeSidebar.style.width);
        } else if (currentWidth < minTreeWidth && nodeTreeSidebar.style.display !== 'none') {
            nodeTreeSidebar.style.width = `${minTreeWidth}px`;
            localStorage.setItem('nodeTreeWidth', nodeTreeSidebar.style.width);
        }
    }


    $('#logoutButton').on('click', function() {
        localStorage.removeItem('authToken');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userName'); 
        toastr.info('Logged out successfully.');
        window.location.href = 'login.html';
    });

    // Event listener for mobile refresh button
    $('#refreshTreeButton').on('click', function() {
        loadNodes();
        toastr.info('Tree refreshed!');
    });

    // Event listener for desktop refresh button
    $('#refreshTreeButtonDesktop').on('click', function() {
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

        // Set up "Edit Profile" button click handler
        $('#editProfileButton').on('click', function() {
            const editProfileModal = new bootstrap.Modal(document.getElementById('editProfileModal'));
            // Populate current user info into the modal
            $('#profileNameInput').val(localStorage.getItem('userName') || localStorage.getItem('userEmail'));
            $('#profileEmailInput').val(localStorage.getItem('userEmail')); // Email is disabled
            editProfileModal.show();
        });

        // Set up "Save Profile" button handler
        $('#saveProfileButton').on('click', async function() {
            const newName = $('#profileNameInput').val();
            const userEmail = localStorage.getItem('userEmail');
            const authToken = localStorage.getItem('authToken');
            const userId = localStorage.getItem('userId'); // Assuming userId is stored, needed for update

            if (!newName) {
                toastr.error('Name cannot be empty.');
                return;
            }

            try {
                const response = await fetch(`${API_BASE_URL}/api/User`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${authToken}`
                    },
                    body: JSON.stringify({
                        UserId: userId, 
                        Name: newName,
                        Email: userEmail // Keep email unchanged
                    })
                });

                if (response.ok) {
                    const updatedUser = await response.json();
                    localStorage.setItem('userName', updatedUser.Name);
                    $('#loggedInUserFullName').text(updatedUser.Name);
                    $('#userInitial').text(updatedUser.Name.charAt(0).toUpperCase());
                    toastr.success('Profile updated successfully!');
                    bootstrap.Modal.getInstance(document.getElementById('editProfileModal')).hide();
                } else {
                    const errData = await response.json();
                    toastr.error(errData._Message || 'Failed to update profile.');
                }
            } catch (err) {
                toastr.error('Network error during profile update.');
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
                const userId = localStorage.getItem('userId'); // Assuming userId is available

                const response = await fetch(`${API_BASE_URL}/api/User/ChangePassword`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${authToken}`
                    },
                    body: JSON.stringify({
                        UserId: userId, // Use userId as identifier
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

        // Initialize jsTree instances by calling jstree() on elements.
        // It's possible jstree may return undefined if the element is hidden (display: none),
        // but we'll try to retrieve the instance explicitly afterwards.
        $('#nodeTree').jstree({
            'core': {
                'data': [],
                'check_callback': true,
                'themes': { 'name': 'default', 'responsive': true, 'variant': 'small' },
                'sort': customTreeSort
            },
            'plugins': ['contextmenu', 'wholerow'], // Removed 'dnd' for mobile (responsive)
            'contextmenu': { 'items': customMenu }
        });

        $('#nodeTreeDesktop').jstree({
            'core': {
                'data': [],
                'check_callback': true,
                'themes': { 'name': 'default', 'responsive': true, 'variant': 'small' },
                'sort': customTreeSort
            },
            'plugins': ['dnd', 'contextmenu', 'wholerow'],
            'contextmenu': { 'items': customMenu }
        });

        // Explicitly retrieve the jsTree instances after attempts to initialize them
        nodeTreeInstanceMobile = $('#nodeTree').jstree(true);
        nodeTreeInstanceDesktop = $('#nodeTreeDesktop').jstree(true);
        
        // Add console warnings if instances are not properly initialized
        if (!nodeTreeInstanceMobile) {
            console.warn('jsTree mobile instance is null or undefined after initialization attempt. It might be hidden on load.');
        }
        if (!nodeTreeInstanceDesktop) {
            console.warn('jsTree desktop instance is null or undefined after initialization attempt. It might be hidden on load.');
        }

        loadNodes(); // Load nodes into both trees initially

        // Set up jsTree events only if instances are valid
        if (nodeTreeInstanceMobile) {
            setupJSTreeEvents(nodeTreeInstanceMobile, '#nodeTree'); // Setup events for mobile tree
        }
        if (nodeTreeInstanceDesktop) {
            setupJSTreeEvents(nodeTreeInstanceDesktop, '#nodeTreeDesktop'); // Setup events for desktop tree
        }

        setupQuillAutoSave();
        updateDarkMode(true); // Force dark mode as default
        setupKeyboardShortcuts();
        initSplitter(); // Initialize splitter logic
    }

    // Custom sort function for jsTree
    function customTreeSort(a, b) {
        const tree = this.get_plugin('core'); // Access core plugin from within the context of the instance
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
    } // Missing closing brace added here
    // --- jsTree Events (Drag & Drop, Rename) ---
    function setupJSTreeEvents(jstreeInstance, treeSelector) {
        $(treeSelector).on('rename_node.jstree', function (e, data) {
            updateNodeName(data.node.id, data.text);
        }).on('move_node.jstree', async function (e, data) {
            const tree = jstreeInstance; // Use the passed instance
            const movedNodeId = data.node.id;
            const newParentId = data.parent === '#' ? null : data.parent;
            const oldParentId = data.old_parent === '#' ? null : data.old_parent;

            let success = true;

            const updateMovedNodePayload = { NodeId: movedNodeId, ParentId: newParentId };
            success = await sendNodeUpdate(updateMovedNodePayload);
            if (!success) { loadNodes(); return; }

            if (newParentId !== null) {
                const newParentNode = tree.get_node(data.parent);
                const newNodeChildrenOrder = newParentNode.children;
                if (newNodeChildrenOrder.length > 0) {
                    const updateNewParentOrderPayload = { NodeId: newParentId, ChildrenOrder: newNodeChildrenOrder };
                    success = await sendNodeUpdate(updateNewParentOrderPayload);
                    if (!success) { loadNodes(); return; }
                }
            }

            if (oldParentId !== null && oldParentId !== newParentId) {
                const oldParentNode = tree.get_node(data.old_parent);
                const oldNodeChildrenOrder = oldParentNode.children;
                if (oldNodeChildrenOrder.length > 0) {
                    const updateOldParentOrderPayload = { NodeId: oldParentId, ChildrenOrder: oldNodeChildrenOrder };
                    success = await sendNodeUpdate(updateOldParentOrderPayload);
                    if (!success) { loadNodes(); return; }
                }
            }

            if (success) {
                toastr.success('Node moved/reordered successfully!');
                loadNodes();
            }
        }).on('refresh.jstree', function() {
            setTimeout(function() {
                if( nodeToSelectAfterReload)
                {
                    const tree = jstreeInstance;
                    if( tree )
                        tree.select_node(nodeToSelectAfterReload);
                    nodeToSelectAfterReload = null;
                }
            }, 0);
        }).on('select_node.jstree', async function(e, data) { // Add select_node event here for both trees
            if ($(this).attr('id') === 'nodeTree' && window.innerWidth < 992) {
                // If on mobile, close the offcanvas when a node is selected from mobile tree
                const offcanvasElement = document.getElementById('offcanvasNodeTree');
                const bsOffcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);
                if (bsOffcanvas) {
                    bsOffcanvas.hide();
                }
            }

            if (selectedNodeId && isContentModified) {
                await saveNodeContents(selectedNodeId, quill.root.innerHTML, false);
            }

            selectedNodeId = data.node.id;
            quill.enable(false);
            quill.setText('Loading note content...');

            hideSaveChangesButton();
            isContentModified = false;

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
                if (nodeTreeInstanceMobile) {
                    nodeTreeInstanceMobile.delete_node(nodeId); // Remove from mobile tree locally
                }
                if (nodeTreeInstanceDesktop) {
                    nodeTreeInstanceDesktop.delete_node(nodeId); // Remove from desktop tree locally
                }
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
                if (!nodeToSelectAfterReload) {
                    nodeToSelectAfterReload = jstreeData.length > 0 ? jstreeData[0].id : null;
                }
                
                // Update both jstree instances
                if (nodeTreeInstanceMobile) {
                    nodeTreeInstanceMobile.settings.core.data = jstreeData;
                    nodeTreeInstanceMobile.refresh();
                }
                if (nodeTreeInstanceDesktop) {
                    nodeTreeInstanceDesktop.settings.core.data = jstreeData;
                    nodeTreeInstanceDesktop.refresh();
                }

            } else {
                const errData = await response.json();
                toastr.error(errData._Message || 'Failed to load nodes.');
                if (response.status === 401) {
                    // setTimeout(() => window.location.href = 'login.html', 1500); // Re-enable if needed
                }
            }
        } catch (err) {
            console.log('Network error while loading nodes. Error=' + err);
            toastr.error('Network error while loading nodes.');
            // setTimeout(() => window.location.href = 'login.html', 1500); // Re-enable if needed
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

    // This function is now redundant as we force dark mode and hide the button.
    // However, the dark mode toggle is part of the original HTML, so if it's ever
    // re-enabled, this function provides the icon update.
    function updateDarkModeToggleIcon(isDarkMode) {
        const iconElement = $('#darkModeToggle i');
        if (iconElement.length > 0) { // Check if the element exists before manipulating
            if (isDarkMode) {
              iconElement.removeClass('bi-moon-fill').addClass('bi-sun-fill');
            } else {
              iconElement.removeClass('bi-sun-fill').addClass('bi-moon-fill');
            }
        }
    }

    // --- Document Ready ---
    $(document).ready(function() {
        // Initial check for authentication and dashboard setup
        checkAuth();
    });
});
