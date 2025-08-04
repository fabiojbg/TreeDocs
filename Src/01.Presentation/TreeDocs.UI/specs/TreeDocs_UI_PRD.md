# Product Requirements Document: TreeDocs.UI (React)

## 1. Introduction

This document provides a detailed Product Requirements Document (PRD) for the `TreeDocs.UI` React application. Its purpose is to comprehensively explain the existing functionalities, design, and UI interactions to enable an LLM programmer to re-create the application with a deep understanding of its core features and architecture.

## 2. Product Overview

TreeDocs.UI is a single-page web application built with React, Tailwind CSS, and Vite. It serves as the client-side interface for a hierarchical note-taking system. Users can create, organize, and manage rich text/HTML notes in a tree-like structure. The application integrates with a backend API (`TreeDocs.Service`) for authentication and node management.

**Core Functionalities:**
*   User Authentication (Login, Registration)
*   Hierarchical Note Management (CRUD operations on nodes)
*   Rich Text/HTML Editor for node content
*   Drag-and-Drop for node re-ordering and re-parenting
*   Theming (Light/Dark mode)
*   Real-time notifications via `react-hot-toast`

## 3. Authentication & User Management

### 3.1. Login Page (`/login`)
*   **Purpose**: Allows existing users to authenticate and gain access to their notes.
*   **UI Elements**:
    *   Title: "Sign in to TreeDocs"
    *   Subtitle: "Create and manage hierarchical notes"
    *   Email input field (required, type="email", `autocomplete="email"`)
    *   Password input field (required, type="password", `autocomplete="current-password"`)
    *   "Sign in" button (disabled during loading, shows spinner and "Signing in..." text)
    *   Link to "Register here" page for new users.
*   **Interactions**:
    *   Users enter email and password.
    *   On form submission:
        *   `loading` state is set to `true`, disabling the button and showing a spinner.
        *   `authService.login` is called with provided credentials.
        *   On successful login:
            *   User data is stored using `AuthContext`.
            *   A success toast notification "Logged in successfully!" is displayed.
            *   User is navigated to the `/dashboard` page.
        *   On failed login (error handled by API interceptor, console.error is used):
            *   `loading` state is set to `false`.
*   **Styling**: Centered layout with Tailwind CSS for responsiveness and dark mode compatibility.

### 3.2. Registration Page (`/register`)
*   **Purpose**: Allows new users to create an account.
*   **UI Elements**:
    *   Title: "Create an account"
    *   Subtitle: "Get started with TreeDocs"
    *   Full Name input field (required, type="text")
    *   Email input field (required, type="email", `autocomplete="email"`)
    *   Password input field (required, type="password")
    *   Confirm Password input field (required, type="password")
    *   "Create account" button (disabled during loading, shows spinner and "Creating account..." text)
    *   Link to "Sign in" page for existing users.
*   **Interactions**:
    *   Users enter name, email, password, and confirm password.
    *   On form submission:
        *   Client-side validation: Passwords must match. If not, an error toast "Passwords do not match" is displayed, and submission is aborted.
        *   `loading` state is set to `true`, disabling the button and showing a spinner.
        *   `authService.register` is called with provided details.
        *   On successful registration:
            *   A success toast "Account created successfully! Redirecting to login..." is displayed.
            *   Form fields are cleared.
            *   User is navigated to the `/login` page after a 2-second delay.
        *   On failed registration (error handled by API interceptor, console.error is used):
            *   `loading` state is set to `false`.
*   **Styling**: Centered layout with Tailwind CSS for responsiveness and dark mode compatibility.

### 3.3. Authentication Context (`AuthContext.jsx`)
*   Manages the authentication state (`user` and `isAuthenticated`).
*   Provides `login` and `logout` functions to update the context and manage token storage (via `localStorage`).
*   Interceptors in `api.js` automatically attach JWT tokens from `localStorage` to outgoing requests and handle token expiration/API errors.

## 4. Dashboard Layout (`/dashboard` or `/`)

*   **Purpose**: The main application interface where users manage their notes.
*   **Structure**:
    *   **Header**: Always present at the top.
        *   Displays the authenticated user's name.
        *   Includes a "Logout" button.
        *   Includes a Theme Toggle button (for Light/Dark mode).
    *   **Main Content Area (Split View)**:
        *   **Left Sidebar (Node Tree)**: Occupies 1/4th of the width.
            *   Displays the hierarchical structure of user notes.
            *   Includes an "Add" button to create a new top-level node.
            *   Shows a loading spinner when notes are being fetched.
        *   **Right Content Area (Node Editor)**: Occupies 3/4ths of the width.
            *   Displays the rich text editor for the currently selected node.
            *   Shows a "No node selected" message and icon if no node is active.
*   **Key Behavior**:
    *   Requires user authentication. If no `user` is present, it displays a "Please log in" message.
    *   Auto-saves pending editor changes before navigation or component unmount (e.g., when logging out).
    *   Keyboard Shortcuts:
        *   `Ctrl + N`: Creates a new child node under the currently selected node (or a root node if none selected).
        *   `Delete`: Deletes the currently selected node (after confirmation dialog).

## 5. Node Tree Functionality (`Src/01.Presentation/TreeDocs.UI/src/features/nodes/components/NodeTree.jsx`)

*   **Component**: `NodeTree` renders a recursive list of `NodeItem` components.
*   **Data Source**: Receives `nodes` (hierarchical array), `selectedNode`, `openNodes` (from Zustand store).
*   **Display**:
    *   Nodes are rendered with indentation based on their level in the hierarchy.
    *   Nodes can be expanded/collapsed (indicated by an icon, e.g., triangle/chevron).
    *   Selected node is highlighted.
*   **Node Item (`NodeItem.jsx`)**:
    *   Displays node icon (e.g., folder/document), name, and expansion toggle.
    *   Handles click events for selection and toggling expansion.
    *   Triggers context menu on right-click.
    *   Implements drag-and-drop attributes (`draggable`).
*   **Node Selection**:
    *   Clicking a `NodeItem` selects it, displaying its content in the `NodeEditor`.
    *   Uses `onSelect` prop to communicate selection to parent (`DashboardPage`).
*   **Node Expansion/Collapse**:
    *   Clicking the expand/collapse icon (or the node itself) toggles the visibility of its children.
    *   `useNodeStore` (Zustand) manages the `openNodes` state, persisting which nodes are expanded/collapsed across sessions.
*   **Context Menu**:
    *   Appears on right-click of a `NodeItem`.
    *   **Options**:
        *   **"Add Child"**: Creates a new 'Document' type child node under the right-clicked node.
        *   **"Delete"**: Prompts for confirmation then deletes the node and all its children.
    *   Disappears on outside click.
*   **Drag and Drop (`useNodeDragAndDrop.js`)**:
    *   Allows users to reposition nodes within the tree hierarchy.
    *   **Functionality**:
        1.  **Re-ordering**: Change the order of sibling nodes under the same parent.
        2.  **Re-parenting**: Move a node to become a child of a different parent node.
        3.  **Positioning**: Dragged node can be dropped `before`, `after`, or `inside` an existing node.
            *   **`before`/`after` indicators**: A thin blue line appears above/below the target node.
            *   **`inside` indicator**: A blue border appears around the target folder node (when it's a "Folder" type).
    *   **Visual Feedback**:
        *   Opacity changes for the dragged node.
        *   Horizontal blue lines indicating potential drop positions (`before`, `after`).
        *   Border around a folder indicating "inside" drop.
    *   **Mechanism**: The `onMoveNode` callback (from `useNodeManagement`) is called upon successful drop, updating the backend. This involves updating the `ParentId` of the dragged node and the `ChildrenOrder` array of the affected parent(s).

## 6. Node Editor Functionality (`Src/01.Presentation/TreeDocs.UI/src/features/nodes/components/NodeEditor.jsx`)

*   **Component**: `NodeEditor` displays and allows editing of a selected node's content.
*   **Rich Text Editor**: Uses `react-quill` for rich text editing capabilities.
*   **Fields**:
    *   **Node Name**: Editable text input for the node's name.
    *   **Node Content**: Rich Text Area for the node's content.
*   **Auto-Saving**:
    *   Implemented via `useRef` to manage the quill instance and a `useEffect` with a debounce mechanism or explicit save calls.
    *   Content changes are automatically saved periodically (e.g., debounced after user stops typing) or when the editor loses focus.
    *   The `onUpdate` callback (from `useNodeManagement`) is called to persist changes to the backend.
    *   A `isDirty` state or ref is maintained to track unsaved changes.
    *   `savePendingChanges` method is exposed via `useImperativeHandle` for external calls (e.g., before navigation, on unmount).
*   **Focus Management**: Notifies parent (`DashboardPage`) when the editor gains/loses focus via `onEditorFocusChange`.

## 7. Technical Considerations

*   **Framework**: React (using functional components and hooks).
*   **Styling**: Tailwind CSS for utility-first styling. Dark mode is supported.
*   **Build Tool**: Vite for fast development and optimized builds.
*   **Routing**: `react-router-dom` for client-side navigation.
*   **State Management**:
    *   **Authentication**: React Context API (`AuthContext`) for global user state.
    *   **Theming**: React Context API (`ThemeContext`) for global theme state.
    *   **Nodes**: Zustand (`nodeStore.js`) for managing global node data, `selectedNode`, `openNodes`, `loading`, and `error` states. This provides a lightweight and flexible state management solution.
*   **API Integration**:
    *   `axios` for HTTP requests.
    *   `api.js` configures the base API instance, handling authorization headers (JWT tokens) and global error interceptors (e.g., for showing toast notifications on API errors and logging).
    *   `authService.js` and `nodeService.js` encapsulate API calls specific to authentication and node operations, respectively.
*   **Notifications**: `react-hot-toast` for simple, accessible toast notifications (success, error).
*   **Code Structure**: Follows a feature-sliced architecture where related components, hooks, services, and state for a specific feature (e.g., `auth`, `nodes`, `theme`) are co-located.
*   **Project Structure (relevant to UI)**:
    ```
    TreeDocs.UI/
    ├── public/
    ├── src/
    │   ├── components/ # Reusable UI components (general)
    │   │   └── layout/
    │   │       └── Header.jsx
    │   ├── features/ # Feature-based modules
    │   │   ├── auth/ # Authentication
    │   │   │   ├── ... (pages, context, services)
    │   │   ├── nodes/ # Node management
    │   │   │   ├── components/ # Node-specific UI components
    │   │   │   │   ├── NodeEditor.jsx
    │   │   │   │   ├── NodeItem.jsx
    │   │   │   │   └── NodeTree.jsx
    │   │   │   ├── hooks/ # Node-specific custom hooks
    │   │   │   │   ├── useNodeDragAndDrop.js
    │   │   │   │   └── useNodeManagement.js
    │   │   │   ├── pages/
    │   │   │   │   └── Dashboard.jsx
    │   │   │   ├── services/
    │   │   │   │   └── nodeService.js
    │   │   │   └── store/
    │   │   │       └── nodeStore.js
    │   │   └── theme/ # Theming
    │   │       ├── ... (components, context, store)
    │   ├── services/ # Global API configuration
    │   │   └── api.js
    │   ├── styles/ # Global CSS
    │   │   └── tailwind.css
    │   ├── App.jsx # Main application component
    │   └── main.jsx # Entry point
    └── ... (package.json, tailwind.config.js, vite.config.js etc.)
    ```

## 8. API Interactions (Simplified as per TreeDocsServices_NodesAPI.md and Project Guidelines)

The `TreeDocs.UI` application interacts with the `TreeDocs.Service` backend via the following key endpoints:

*   **`POST /api/User/Login`**: Authenticates user, receives JWT token. (Used by `authService`)
*   **`POST /api/User`**: Registers new user. (Used by `authService`)
*   **`GET /api/v1/nodes`**: Retrieves all nodes for the authenticated user (without `Contents`). (Used by `nodeService`)
*   **`GET /api/v1/nodes/{nodeId}`**: Retrieves a specific node's details (including `Contents`, without `Children`). (Used by `nodeService`)
*   **`POST /api/v1/nodes`**: Creates a new node. (Used by `nodeService`)
*   **`PUT /api/v1/nodes`**: Updates an existing node's name, content, parent, or children order. (Used by `nodeService`)
*   **`DELETE /api/v1/nodes/{nodeId}`**: Deletes a node and its children. (Used by `nodeService`)

**Error Handling**:
*   API responses follow the `RequestResult` structure defined in the backend documentation for errors.
*   `api.js` interceptors automatically handle these error structures, displaying toast messages and logging.
