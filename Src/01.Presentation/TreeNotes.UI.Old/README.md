# TreeNotes UI

This is the React-based frontend for the TreeNotes application, a hierarchical note-taking tool.

## Features

- User authentication (login/register) with JWT tokens.
- Hierarchical note management (CRUD operations on nodes).
- Rich text/HTML editing for node content.
- Tree view navigation with expandable/collapsible nodes.
- Context menu for nodes (add child, delete).
- Drag-and-drop for node re-ordering and re-parenting.
- Theming (Light/Dark mode).
- Real-time notifications via `react-hot-toast`.

## Tech Stack

- React 18
- React Router v6
- Tailwind CSS for styling.
- Vite for fast development and optimized builds.
- Axios for API requests.
- react-quill for rich text editing.
- Zustand for node state management.
- React Context API for authentication and theming.
- react-hot-toast for notifications.

## Development

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the development server:
   ```bash
   npm run dev
   ```

The application will be available at http://localhost:3000

## Project Structure

```
src/
├── components/          # Reusable UI components (general)
│   └── layout/          # Layout components
│       └── Header.jsx
├── features/            # Feature-based modules
│   ├── auth/            # Authentication related features
│   │   ├── pages/       # Pages specific to auth
│   │   │   ├── Login.jsx
│   │   │   └── Register.jsx
│   │   ├── context/     # React context providers for authentication
│   │   │   └── AuthContext.jsx
│   │   └── services/    # API service implementations for authentication
│   │       └── authService.js
│   ├── nodes/           # Node-related features (tree, editor, management)
│   │   ├── components/  # Reusable UI components specific to nodes
│   │   │   ├── NodeTree.jsx
│   │   │   ├── NodeItem.jsx
│   │   │   └── NodeEditor.jsx
│   │   ├── hooks/       # Custom hooks for node-related logic
│   │   │   ├── useNodeManagement.js
│   │   │   └── useNodeDragAndDrop.js
│   │   ├── pages/       # Pages specific to nodes
│   │   │   └── Dashboard.jsx
│   │   ├── services/    # API service implementations for nodes
│   │   │   └── nodeService.js
│   │   └── store/       # Zustand stores for global node state
│   │       └── nodeStore.js
│   └── theme/           # Theming
│       ├── components/
│       ├── context/
│       └── store/
├── services/            # Global API service configuration
│   └── api.js
├── styles/              # Global CSS and Tailwind configuration
│   └── tailwind.css
├── App.jsx              # Main application component with routing and layout
└── main.jsx             # Application entry point initializing React and context
```

## API Integration

The UI communicates with the TreeNotes backend service (`TreeNotes.Service`) through REST APIs. The Vite proxy is configured to forward `/api` requests to the backend service running on `http://localhost:5555`.

Key Endpoints:

- **Authentication**:
  - `POST /api/User/Login`: User login.
  - `POST /api/User`: User registration.
- **Node Management**:
  - `GET /api/v1/nodes`: Retrieves all nodes for the authenticated user (without `Contents`).
  - `GET /api/v1/nodes/{nodeId}`: Retrieves a specific node's details (including `Contents`).
  - `POST /api/v1/nodes`: Creates a new node.
  - `PUT /api/v1/nodes`: Updates an existing node.
  - `DELETE /api/v1/nodes/{nodeId}`: Deletes a node and its children.

Error handling is managed via API interceptors in `src/services/api.js`, which parse the backend's `RequestResult` error structure and display toast notifications using `react-hot-toast`.

## Key Functionality Details

### Authentication
- **Login Page (`/login`)**: Email and password input, with navigation to registration.
- **Registration Page (`/register`)**: Full name, email, password, and confirm password inputs, with validation.
- **AuthContext**: Manages user authentication state and token persistence.

### Dashboard (`/dashboard`)
- **Header**: Displays user name, logout button, and theme toggle.
- **Main Content**:
  - **Left Sidebar (Node Tree)**: Displays the hierarchical note structure, an "Add" button for top-level nodes, and a loading spinner.
  - **Right Content Area (Node Editor)**: Shows a rich text editor for the selected node or a "No node selected" message.
- **Keyboard Shortcuts**:
  - `Ctrl + N`: Creates a new child node.
  - `Delete`: Deletes the selected node after confirmation.

### Node Tree (`NodeTree.jsx` & `NodeItem.jsx`)
- Renders a recursive list of nodes.
- Nodes can be expanded/collapsed.
- Clicking a node selects it and displays its content in the editor.
- Right-clicking a node opens a context menu with "Add Child" and "Delete" options.
- Implements drag-and-drop for re-ordering (before/after) and re-parenting (inside) nodes with visual indicators.

### Node Editor (`NodeEditor.jsx`)
- Uses `react-quill` for rich text editing.
- Contains an editable node name input.
- Features auto-saving with debouncing or on focus loss.
- Exposes a `savePendingChanges` method for saving before navigation or unmount.

### State Management
- **Authentication & Theming**: React Context API.
- **Nodes**: Zustand store for managing nodes, `selectedNode`, `openNodes`, `loading`, and `error` states.
