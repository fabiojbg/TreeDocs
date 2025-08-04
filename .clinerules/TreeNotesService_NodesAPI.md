# TreeNotes Services - Nodes API Documentation

This document provides an overview of the REST APIs exposed by the `TreeNotes.Service` project to edit user nodes

---

## 1. NodesController

**Base Route**: `/api/v1/nodes`

**Responsibility**: This controller manages all operations related to user nodes within the hierarchical note-taking system, including creating, retrieving, updating, and deleting nodes.

**Authentication**: Requires authentication via `[Authorize]` and `[AuthenticationRequired]` filters.

**Endpoints**:

### `GET /api/v1/nodes`
- **Description**: Retrieves all nodes associated with the authenticated user. It is important to note that this call ommits `Contents` field because this field can be quite large. This endpoint is mainly focused to allow UI to show the node's tree. For the Contents, refer to `GET /api/v1/nodes/{nodeId}` endpoint
- **Authentication**: Required
- **Parameters**: None
- **Returns**: `200 OK` with a list of user nodes, or an error status code.
- **Contracts**:
    - **Request**: `GetUserNodesRequest`
        - `UserId` (string): The ID of the user whose nodes are to be retrieved.
    - **Response**: `GetUserNodesResponse` or `RequestResult` in case of errors (see [2. Error Handling Instructions](#2-error-handling-instructions))
        - `OwnerId` (string): The ID of the owner of the nodes.
        - `Nodes` (IEnumerable<Node>): A collection of Node objects without its contents
            - **Node Object**:
                - `Id` (string): The unique identifier of the node.
                - `ParentId` (string): The ID of the parent node (null for root nodes).
                - `Name` (string): The name of the node.
                - `NodeType` (NodeType enum): The type of the node (e.g., Folder, Document).
                - `Contents` (string): Omitted or null
                - `CreatedOn` (DateTime): The creation timestamp of the node.
                - `UpdatedOn` (DateTime): The last update timestamp of the node.
                - `Children` (List<Node>): A list of child Node objects (recursive). The children nodes are returned in the order they must be shown in the tree. The nodes is shown in the order user decides and not a predefined order
                - `ChildrenOrder` (List<string>): The list of order of the immediate children nodes.

### `GET /api/v1/nodes/{nodeId}`
- **Description**: Retrieves details for a specific node by its ID. In this endpoint, the `Children` field is ommitted, so be carefull
- **Authentication**: Required
- **Parameters**:
    - `nodeId` (`string`, Path Parameter): The unique identifier of the node to retrieve.
- **Returns**: `200 OK` with the requested node's details, `404 Not Found` if the node does not exist, or an error status code.
- **Contracts**:
    - **Request**: `GetUserNodeRequest`
        - `NodeId` (string): The ID of the specific node to retrieve.
    - **Response**: `GetUserNodeResponse` or `RequestResult` in case of errors (see [2. Error Handling Instructions](#2-error-handling-instructions))
        - `OwnerId` (string): The ID of the owner of the node.
        - `Node` <Node>): Node object without the Children field.
            - **Node Object**:
                - `Id` (string): The unique identifier of the node.
                - `ParentId` (string): The ID of the parent node (null for root nodes).
                - `Name` (string): The name of the node.
                - `NodeType` (NodeType enum): The type of the node (e.g., Folder, Document).
                - `Contents` (string): The rich text content of the node. 
                - `CreatedOn` (DateTime): The creation timestamp of the node.
                - `UpdatedOn` (DateTime): The last update timestamp of the node.
                - `Children` (List<Node>) - ommited or null
                - `ChildrenOrder` (List<string>): The list of order of the immediate children nodes. The tree must reflect this order that is choosen by the user.

### `POST /api/v1/nodes`
- **Description**: Creates a new node in the user's tree structure.
- **Authentication**: Required
- **Returns**: `200 OK` with the result of the creation, or an error status code.
- **Contracts**:
    - **Request Body**: `CreateNodeRequest`
        - `ParentId` (string): The ID of the parent node under which to create the new node. Can not be null. Only the root node has ParentId=null and its created on user registration.
        - `Name` (string): The name of the new node. ( 3 characters minimum, 100 characters maximum)
        - `NodeType` (NodeType enum): The type of the new node (e.g., Folder, Document).
        - `Contents` (string): (Optional) The initial rich text content for the new node.
    - **Response**: `CreateNodeResponse` or `RequestResult` in case of errors (see [2. Error Handling Instructions](#2-error-handling-instructions))
        - `Id` (string): The ID of the newly created node.

### `PUT /api/v1/nodes`
- **Description**: Updates an existing node's content and metadata. `NodeId` field is mandatory, all others are optional, the ommited fields will remain unchanged.
- **Authentication**: Required
- **Returns**: `200 OK` with the result of the update, or an error status code.
- **Contracts**:
    - **Request Body**: `UpdateUserNodeDataRequest`
        - `NodeId` (string): (mandatory) The ID of the node to update.
        - `ParentId` (string): (Optional) The new parent ID for the node. If ommited, the current state is not changed
        - `Name` (string): The new name for the node. If ommited, the current name is not changed
        - `NodeContents` (string): The new rich text content for the node. If ommited, the content is not changed
        - `ChildrenOrder` (List<string>): (Optional) An ordered list of child node IDs for reordering children. If ommited, the current order is not changed. This list allowed the user to reorder the children nodes of the editing node. The children nodes of any nodes can be ordered in any form.
    - **Response**: `UpdateUserNodeDataResponse` or `RequestResult` in case of errors (see [2. Error Handling Instructions](#2-error-handling-instructions))
        - `Node` (Node): The updated Node object. (See Node Object definition above)

### `DELETE /api/v1/nodes/{nodeId}`
- **Description**: Deletes a node and all its child nodes (client-side confirmation is expected before calling).
- **Authentication**: Required
- **Parameters**:
    - `nodeId` (`string`, Path Parameter): The unique identifier of the node to delete.
- **Returns**: `200 OK` with the result of the deletion, or an error status code.
- **Contracts**:
    - **Request**: `DeleteNodeRequest`
        - `NodeId` (string): The ID of the node to delete.
    - **Response**: `DeleteNodeResponse` or `RequestResult` in case of errors (see [2. Error Handling Instructions](#2-error-handling-instructions))
        - (Empty, indicates successful deletion)

## 2. Node Handling
The node API is made to allow the repositioning of the node in a tree. With the `PUT /api/v1/nodes` endpoint you can:
1) move a node to another parent just changing the node "ParentId" parameter
2) move one or more nodes position inside a parent by managing the "ChildrenOrder" parameter in the parent node.

## 3. Error Handling Instructions
For all API endpoints, the following error structure is returned in case of any error. This structure is returned instead of the expected response. For ease of identification by the caller, the properties returned start with the character '_'. No API responses have properties starting with this character.

```c#
    public class RequestResult
    {
        public string _Message; // The main error message. Always returned.
        public List<Notification> _Notifications; // Optional, a list of notifications or errors, useful for toast messages.
        public RequestResultType _Result; // One of the types below.
    }
```
where
```c# 
    public enum RequestResultType
    { 
        InvalidRequest = -1,
        Unauthorized = -2,
        ObjectNotFound = -3,
        OperationError = -4
    }

    public class Notification
    {
        public string Property;
        public string Message;
    }
```

These structures are returned in one of these HTTP status codes:

400 (BadRequest) - For an invalid request.
401 (Unauthorized) - If the user login is invalid, a call has an invalid or missing authentication token, or the user is trying to access something they cannot.
404 (ObjectNotFound) - For objects not found, like trying to read a nonexistent node.
500 (InternalServerError) - An unexpected server error.


---

