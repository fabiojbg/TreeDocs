# Project Brief
This project is a web application where users can create rich text or html notes in a hierarchical way (tree). 
Every node in the tree can have notes. 
When a node is selected, the user can see and change the rich text note of it.
The user should be able to reposition the nodes using drag and drop where:
  1) The user can move a node to another parent node
  2) The user can re-order a node in the same parent because the order of to nodes in a parent can be persisted and changed
  3) The user can move a node to another parent in any position order.
At the start, the app starts with a root node. The user can add child nodes using the context menu of each node. The context menu give options to add a child node, or remove the node and its children (upon confirmation popup)

# Project Guidelines

- To know how the project is structured, read the sections 'Simplified Project Folder Structure' and 'Module Responsibilities' below. You must keep these sections up to date with changes in the project.
- The main service APIs are documented in these files
    1) '.clinerules/TreenotesServices_NodesAPI.md': APIs operations with nodes.
    2) '.clinerules/TreenotesServices_UserAPI.md': APIs for user management.
- Pay attention to the MCP Tools available to use them whenever necessary
- This project is being develop in a windows operating system, when emitting multiple commands in a single line to the terminal, use a semi-colon to separate them (ex: 'cd newFolder; npm run dev')

# How to work 

- use the Task Manager tool to manage the progress of the tasks/steps when a user request requires more than 1 step to be acomplished

# Code Style & Patterns

- This project is created uses C# and .NET 6.0 and uses Domain Driven Design / Clean architecture
- The project is API-First. The TreeNotes.Service module is the project main service with all service APIs. This module does not have UI interfaces
- Prefer use object oriented approach
- Project frameworkds & Patterns
    > Clean Architecture
    > Domain Driven-Driven Design (DDD)
    > API First (TreeNotes.Service module)
    > JWT Authentication tokens
    > MediatR/CQRS
    > Mongo DBMongoDB for Databases
    > Redis Cache
    > Blazor-Client Client (Old version)
    > Unit tests with xUnit
    > Docker 

# Build and Test

The easiest way to execute this app is by using the Docker Compose files in the project´s root directory. Just use the command **"docker compose up"** to build and run all project services and dependencies, like MongoDB, Redis, and Seq.
Once running, open your browser and type http://localhost:5555.

For development of the React UI, navigate to the TreeNotes.UI directory and run:
```bash
npm install
npm run dev
```
The React development server will be available at http://localhost:3000

# Project Structure

- The project is structured to separate concerns and maintain a clean architecture. 
You must mantain this structure updated in case of any change in it or in file´s responsabilities

# Simplified Project Folder Structure

```
/Project_Root_Folder/ 
├── docker-compose.dcproj
├── docker-compose.override.yml
├── docker-compose.yml
├── Dockerfile_TreeNotesClientApp
├── Dockerfile_TreeNotesService
├── README.md
├── TreeNotes.sln
├── Src/ # Source code
│   ├── 01.Presentation/ # Presentation layer - UI and API endpoints
│   │   ├── TreeNotes.ClientApp/ # Old Client-side web application (Blazor). Replaced by TreeNotes.UI
│   │   │   ├── _Imports.razor
│   │   │   ├── App.razor
│   │   │   ├── Program.cs
│   │   │   ├── TreeNotes.ClientApp.csproj
│   │   │   ├── Components/ # Reusable UI components
│   │   │   ├── Model/ # Client-side data models
│   │   │   ├── Nginx/ # Nginx configuration for the client app
│   │   │   ├── Pages/ # Blazor pages
│   │   │   ├── Properties/
│   │   │   ├── Services/ # Client-side services for data interaction
│   │   │   └── Shared/ # Shared UI elements and layouts
│   │   │   └── wwwroot/ # Static assets (CSS, JS, images)
│   │   ├── TreeNotes.UI/ # New React-based client application for hierarchical note-taking functionality
│   │   │   ├── public/ # Static assets and main HTML entry point (index.html)
│   │   │   ├── src/ # Application source code
│   │   │   │   ├── components/ # Reusable UI components (general, non-feature specific)
│   │   │   │   │   └── layout/ # Layout components
│   │   │   │   │       └── Header.jsx # Application header with navigation and user menu
│   │   │   │   ├── features/ # Feature-based modules
│   │   │   │   │   ├── auth/ # Authentication related features
│   │   │   │   │   │   ├── pages/ # Pages specific to auth
│   │   │   │   │   │   │   ├── Login.jsx # User authentication page for login credentials
│   │   │   │   │   │   │   └── Register.jsx # User registration page for new account creation
│   │   │   │   │   │   ├── context/ # React context providers for authentication
│   │   │   │   │   │   │   └── AuthContext.jsx # Manages authentication state and methods throughout the application
│   │   │   │   │   │   └── services/ # API service implementations for authentication
│   │   │   │   │   │       └── authService.js # Handles authentication API calls (login, registration, tokens)
│   │   │   │   │   ├── nodes/ # Node-related features (tree, editor, management)
│   │   │   │   │   │   ├── components/ # Reusable UI components specific to nodes
│   │   │   │   │   │   │   ├── NodeTree.jsx # Main component for displaying hierarchical tree structure
│   │   │   │   │   │   │   ├── NodeItem.jsx # Individual node component with context menu and selection handling
│   │   │   │   │   │   │   └── NodeEditor.jsx # Rich text editor for creating/editing node content
│   │   │   │   │   │   ├── pages/ # Pages specific to nodes
│   │   │   │   │   │   │   └── Dashboard.jsx # Primary application interface containing node tree and editor
│   │   │   │   │   │   ├── hooks/ # Custom hooks for node-related logic
│   │   │   │   │   │   │   ├── useNodeManagement.js # Centralizes and reuses stateful logic for node operations
│   │   │   │   │   │   │   └── useNodeDragAndDrop.js # Custom hook for drag and drop logic
│   │   │   │   │   │   ├── services/ # API service implementations for nodes
│   │   │   │   │   │   │   └── nodeService.js # Manages node-related API operations (CRUD for tree nodes)
│   │   │   │   │   │   └── store/ # Zustand stores for global node state
│   │   │   │   │   │       └── nodeStore.js # Manages nodes, selectedNode, loading, error states
│   │   │   │   ├── services/ # Global API service configuration
│   │   │   │   │   └── api.js # Configures base API instance with axios and authentication interceptors
│   │   │   │   ├── styles/ # Global CSS and Tailwind configuration
│   │   │   │   ├── App.jsx # Main application component with routing and layout
│   │   │   │   └── main.jsx # Application entry point initializing React and context
│   │   │   ├── package.json # Project dependencies, npm scripts, and configuration
│   │   │   ├── postcss.config.js # PostCSS configuration for Tailwind CSS processing
│   │   │   ├── README.md # UI-specific documentation and development setup
│   │   │   ├── tailwind.config.js # Tailwind CSS theme and plugin configuration
│   │   │   └── vite.config.js # Vite build settings and API proxy configuration
│   │   ├── TreeNotes.UI2/ # New vanilla JavaScript-based client application for hierarchical note-taking functionality
│   │   │   ├── assets/ # Static assets like images and fonts
│   │   │   ├── css/ # Custom and third-party CSS files (e.g., Bootstrap, style.css)
│   │   │   ├── js/ # JavaScript files for login, registration, dashboard, and API configuration
│   │   │   │   ├── api-config.js # Defines the base URL for API calls
│   │   │   │   ├── dashboard.js # Main script for dashboard functionalities (jsTree, Quill, node operations)
│   │   │   │   ├── login.js # Script for login page logic
│   │   │   │   └── register.js # Script for registration page logic
│   │   │   ├── login.html # Static HTML for the user login interface
│   │   │   ├── register.html # Static HTML for new user registration
│   │   │   └── dashboard.html # Static HTML for the main application dashboard
│   │   └── TreeNotes.Service/ # Backend API service. Has no UI
│   │       ├── appsettings.Development.json
│   │       ├── appsettings.json
│   │       ├── Program.cs
│   │       ├── Startup.cs
│   │       ├── TreeNotes.Service.csproj
│   │       ├── ApiController/
│   │       ├── Docs/
│   │           └── TreeNotesServices_API.md  # main project service API Documentation
│   │       ├── Authorization/ # API authorization logic
│   │       ├── Controllers/ # Main project API controllers
│   │       ├── Filters/ # API filters (e.g., for error handling, validation)
│   │       ├── Logs/ # Application logs
│   │       ├── Services/ # Services consumed by the API controllers
│   ├── 02.Application/ # Application layer - business logic orchestration
│   │   └── Application/ # Application services and commands
│   │       ├── Application.csproj
│   │       ├── DependencyInjection.cs # Dependency injection configuration for this layer
│   │       ├── Common/ # Common application concerns (e.g., interfaces, base classes)
│   ├── 03.Domain/ # Domain layer - core business entities and rules
│   │   ├── Domain/ # Domain entities, value objects, and domain services
│   │   │   ├── DependencyInjection.cs
│   │   │   ├── TreeNotes.Domain.csproj
│   │   │   ├── Commands/ # Domain-specific commands (e.g., for MediatR)
│   │   │   ├── Constants/ # Domain-specific constants
│   │   │   ├── Repositories/ # Interfaces for domain repositories
│   │   │   └── Services/ # Domain services
│   │   ├── Shared/ # Shared domain types (if any)
│   │   │   └── TreeNotes.Shared/
│   │   └── TreeNotes.Domain.RequestResponses/ # DTOs for domain requests and responses
│   │       ├── TreeNotes.Domain.RequestResponses.csproj
│   │       ├── Constants/
│   │       ├── Entities/ # DTO entities
│   │       ├── Enums/ # DTO enums
│   │       ├── Requests/ # Requests (commands/queries) to the domain
│   │       └── Responses/ # Responses from the domain
│   ├── 04.Persistence/ # Persistence layer - data access implementation
│   │   ├── Audit.Persistence.MongoDb/ # MongoDB specific audit log implementation
│   │   │   ├── Audit.Persistence.MongoDb.csproj
│   │   │   ├── AuditEntry.cs
│   │   │   ├── AuditTrail.cs
│   │   │   ├── DependencyInjection.cs
│   │   ├── Db.Services.Interfaces/ # Interfaces for database services and repositories
│   │   │   ├── IDbUnitOfWork.cs
│   │   │   ├── IRepository.cs
│   │   │   ├── Repository.Interfaces.csproj
│   │   ├── MongoDb/
│   │   │   └── Database.MongoDb/ # MongoDB database context and concrete implementations
│   │   ├── MongoDb.Common/ # Common utilities for MongoDB persistence
│   │   │   ├── BaseUnitOfWork.cs
│   │   │   ├── DbEntity.cs
│   │   │   ├── IBaseUnitOfWork.cs
│   │   │   ├── MongoDb.Common.csproj
│   │   │   ├── Configuration/
│   │   │   ├── Extensions/
│   │   │   └── Repository/
│   │   ├── TreeNotes.Data.Services/ # Data services for TreeNotes entities
│   │   │   ├── IBasicService.cs
│   │   │   ├── INodeServices.cs
│   │   │   ├── NodeServices.cs
│   │   │   ├── TreeNotes.Data.Services.csproj
│   │   │   └── RepositoryInterfaces/
│   │   └── TreeNotes.Db.Entities/ # Database entities/models
│   │       ├── TreeNotes.Db.Entities.csproj
│   │       ├── User.cs
│   │       ├── UserNode.cs
│   ├── 06.Authorization/ # Authorization layer - security and user management
│   │   ├── Auth.Domain.RequestsResponses/ # DTOs for authentication requests/responses
│   │   │   ├── Auth.Domain.RequestsResponses.csproj
│   │   │   ├── Constants.cs
│   │   │   ├── Requests/
│   │   │   └── Responses/
│   │   ├── TreeNotes.AuthDomain/ # Core authentication domain logic
│   │   │   ├── Auth.Domain.csproj
│   │   │   ├── DependencyInjection.cs
│   │   │   ├── Commands/
│   │   │   ├── Constants/
│   │   │   ├── Entities/
│   │   │   ├── Repository/
│   │   │   └── Services/
│   │   └── TreeNotes.AuthDomain.Persistence.MongoDb/ # MongoDB persistence for authentication
│   │       ├── AuthDomain.Persistence.MongoDb.csproj
│   │       ├── DependencyInjection.cs
│   │       ├── Configuration/
│   │       ├── DbModels/
│   │       ├── Extensions/
│   │       ├── Repositories/
│   │       └── UnitOfWork/
│   ├── 10.Common/ # Common utilities and contracts shared across layers
│   │   └── TreeNotes.Service.Contracts/ # API contracts/DTOs for services
│   │       ├── RequestResult.cs
│   │       ├── TreeNotes.Service.Contracts.csproj
│   │       ├── Authentication/
│   ├── 11.Sdk/ # SDK components for client applications
│   │   ├── Apps.Blazor.Components/ # Reusable Blazor UI components for SDK
│   │   │   ├── _Imports.razor
│   │   │   ├── AppComponentBase.cs
│   │   │   ├── Apps.Blazor.Components.csproj
│   │   │   ├── DependencyInjection.cs
│   │   │   ├── Alerts/
│   │   │   ├── Common/
│   │   │   ├── Extensions/
│   │   │   ├── Navigation/
│   │   │   └── wwwroot/
│   │   ├── Apps.Sdk/ # Core SDK utilities and services
│   │   │   ├── Apps.Sdk.Web.csproj
│   │   │   ├── DependencyInjection.cs
│   │   │   ├── Cache/
│   │   │   ├── Exceptions/
│   │   │   ├── Extensions/
│   │   │   ├── Helpers/
│   │   │   ├── Middlewares/
│   │   │   └── Util/
│   │   └── Apps.Sdk.DependencyInjection/ # Dependency injection configuration for SDK
│   │   │   └── Apps.Sdk.DependencyInjection.csproj
│   │   └── Apps.Sdk.Util/ # SDK utility functions
│   └── 99.Tests/ # Test projects
│       └── AuthDomain.Tests/ # Tests for AuthDomain module
```

# Module Responsibilities

- **TreeNotes.Service**: Main backend service providing all service APIs. API-first approach, no UI.
- **TreeNotes.ClientApp**: Client-side Blazor web application for user interaction. (Deprecated - will be replaced by TreeNotes.UI and TreeNotes.UI2)
- **TreeNotes.UI**: React-based client application for hierarchical note-taking functionality. Built with React, Tailwind CSS, and Vite.
- **TreeNotes.UI2**: New vanilla JavaScript-based client application providing hierarchical note-taking functionality. Built with Bootstrap, jQuery, jsTree, Quill, and toastr. It integrates directly with the TreeNotes.Service API.
- **02.Application (Application layer)**: Orchestrates business logic, handles use cases, and coordinates between domain and persistence layers.
- **03.Domain (Domain layer)**: Contains core business entities, aggregates, value objects, and domain services. Defines business rules and logic.
  - **TreeNotes.Domain**: Core domain models and logic.
  - **TreeNotes.Domain.RequestResponses**: Defines DTOs for requests and responses related to domain operations.
- **04.Persistence (Persistence layer)**: Deals with data storage and retrieval. Contains repository implementations and database contexts.
  - **Audit.Persistence.MongoDb**: Handles audit logging specific to MongoDB.
  - **Db.Services.Interfaces**: Defines interfaces for generic database services and unit of work patterns.
  - **MongoDb.Common**: Provides common utilities and base classes for MongoDB interactions.
  - **TreeNotes.Data.Services**: Specific data services for TreeNotes entities, implementing repository interfaces.
  - **TreeNotes.Db.Entities**: Defines the database entities (data models).
- **06.Authorization (Authorization layer)**: Manages user authentication and authorization concerns.
  - **Auth.Domain.RequestsResponses**: DTOs for authentication-related requests and responses.
  - **TreeNotes.AuthDomain**: Core domain logic for authentication and user management.
  - **TreeNotes.AuthDomain.Persistence.MongoDb**: MongoDB persistence implementation for authorization data.
- **10.Common**: Contains shared contracts and utilities used across different layers.
  - **TreeNotes.Service.Contracts**: Defines common request/response contracts for service APIs.
- **11.Sdk**: Provides SDK components for client applications.
  - **Apps.Blazor.Components**: Reusable UI components built with Blazor for the SDK.
  - **Apps.Sdk**: Core SDK functionalities, including caching, exception handling, and middleware.
  - **Apps.Sdk.DependencyInjection**: Handles dependency injection for the SDK.
  - **Apps.Sdk.Util**: General utility functions for the SDK.
- **99.Tests**: Contains all test projects for the solution.
  - **AuthDomain.Tests**: Tests specifically for the Authorization domain.

## TreeNotes.Service - ApiControllers 
The TreeNotes.Service module is the API-first, backend service that provides all REST APIs for the service. 
This module has no user interfaces and its APIs are documentes in file TreeNotesServices_API.md in folder \Src\01.Presentation\TreeNotes.Service\ApiDocs
The main APIs are:

### `ApiBaseController.cs`
**Responsibility**: Foundational base controller that provides common infrastructure for all API controllers in the TreeNotes.Service project. This controller:
- **Authentication Context**: Provides access to the authenticated user via `TokenService.GetAuthenticatedUser()`
- **Dependency Resolution**: Manages lazy-loaded core services including:
  - `IMediator` - Command/Query handling via MediatR pattern
  - `IConfiguration` - Application configuration access
  - `ILogger` - Structured logging across all derived controllers
  - `User` - Current logged-in user information via `ILoggedUserService`
- **Response Standardization**: Contains `ToActionResult<T>()` method that converts `RequestResult<T>` objects into standardized HTTP responses, handling:
  - Success (200 OK)
  - Unauthorized (401)
  - Not Found (404)
  - Internal Server Error (500)
  - Bad Request (400) for other failures

### `NodesController.cs`
**Responsibility**: Core API controller for handling the hierarchical note-taking functionality, managing all node/notes-related operations. This controller:
- **Endpoints**:
  - `GET api/v1/nodes` - Retrieves all nodes for authenticated user
  - `GET api/v1/nodes/{nodeId}` - Gets specific node details
  - `POST api/v1/nodes` - Creates new nodes in the tree
  - `PUT api/v1/nodes` - Updates existing node content and metadata
  - `DELETE api/v1/nodes/{nodeId}` - Deletes node and its children (with confirmation handled client-side)
- **Tree Management**: Facilitates the core tree operations enabling users to create, read, update, and delete hierarchical notes
- **Security**: Requires authentication via `[Authorize]` and custom `[AuthenticationRequired]` filters
- **Error Handling**: Comprehensive logging and standardized error responses for all node-related operations

### `UserController.cs`
**Responsibility**: Comprehensive user management controller handling user accounts, authentication, and user data operations. This controller:
- **Authentication & Registration**:
  - `POST api/User/Login` - Authenticates users and issues JWT tokens with 7-day expiration
  - `POST api/User` - Creates new user accounts (anonymous access allowed)
- **User Profile Operations**:
  - `GET api/User/Authenticated` - Returns currently authenticated user info
  - `GET api/User/{userEmail}` - Retrieves user details by email (requires auth)
  - `PUT api/User` - Updates user profile information
  - `PUT api/User/ChangePassword` - Handles password changes
- **Testing/Background Operations**:
  - `GET api/User/test/{userEmail}` - Demonstrates background job execution using `BackgroundWorker` while completing the request immediately
- **Security**: Most endpoints require authentication, with exceptions for registration (`POST api/User`)
