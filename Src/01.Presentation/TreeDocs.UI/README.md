# TreeDocs UI

This is the React-based frontend for the TreeDocs application, a hierarchical note-taking tool.

## Features

- User authentication (login/register)
- Hierarchical note management
- Rich text editing
- Tree view navigation
- Context menu for nodes (add child, delete)

## Tech Stack

- React 18
- React Router v6
- Tailwind CSS
- Axios for API requests

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
├── components/
│   ├── auth/
│   ├── nodes/
│   └── layout/
├── pages/
│   ├── Login.jsx
│   ├── Register.jsx
│   └── Dashboard.jsx
├── services/
│   ├── api.js
│   ├── authService.js
│   └── nodeService.js
├── context/
│   └── AuthContext.jsx
├── App.jsx
└── main.jsx
```

## API Integration

The UI communicates with the TreeDocs backend service through REST APIs. The Vite proxy is configured to forward `/api` requests to the backend service running on `http://localhost:5555`.
