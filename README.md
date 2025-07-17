# File Management System (Backend Service)

This is a **.NET 8 backend-only service** that manages folders, files, and their histories using an in-memory data model with **JSON file-based persistence** — no external database required.

The API supports common file system operations like **creating**, **renaming**, **moving**, **deleting**, and **retrieving history** of both files and folders.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Run the Project

```bash
dotnet run
```

## Folder Operations

### Create a folder
```bash
curl -X POST -H "Content-Type: application/json" \
     -d '{ "name": "My Folder", "parentFolderId": null }' \
     http://localhost:5237/api/folders
```
### Get folder contents
```bash
curl http://localhost:5237/api/folders/contents?folderId=<folder_id>
```
### Rename a folder
```bash
curl -X PUT -H "Content-Type: application/json" \
     -d '{ "newName": "Renamed Folder" }' \
     http://localhost:5237/api/folders/<folder_id>/rename
```
### Move a folder
```bash
curl -X PUT -H "Content-Type: application/json" \
     -d '{ "newParentFolderId": "<new_parent_folder_id>" }' \
     http://localhost:5237/api/folders/<folder_id>/move
```
### Delete a folder
```bash
curl -X DELETE http://localhost:5237/api/folders/<folder_id>
```
---

## File Operations

### Upload a file

```bash
curl -X POST \
     -F "metadata={\"name\": \"my-file.txt\", \"type\": \"text/plain\"};type=application/json" \
     -F "file=@/path/to/your/file.txt" \
     http://localhost:5237/api/files/upload
```

### Download a file

```bash
curl http://localhost:5237/api/files/<file_id>/download -o my-file.txt
```

### Get file metadata

```bash
curl http://localhost:5237/api/files/<file_id>
```

### Rename a file

```bash
curl -X PUT -H "Content-Type: application/json" \
     -d '{ "newName": "renamed-file.txt" }' \
     http://localhost:5237/api/files/<file_id>/rename
```
### Move a file

```bash
curl -X PUT -H "Content-Type: application/json" \
     -d '{ "newFolderId": "<new_folder_id>" }' \
     http://localhost:5237/api/files/<file_id>/move
```
### Delete a file

```bash
curl -X DELETE http://localhost:5237/api/files/<file_id>
```
### Get file history

```bash
curl http://localhost:5237/api/history/file/<file_id>
```
### Get file metadata

```bash
curl http://localhost:5237/api/files/<file_id>
```
### Get file metadata

```bash
curl http://localhost:5237/api/files/<file_id>

```
##  Tech Stack

- **Language:** C# (.NET 8)
- **Framework:** ASP.NET Core (Minimal APIs)
- **Persistence:** JSON file-based (custom JsonPersistenceManager)
- **Data Storage:** No external database — uses disk-based `data.json`

---

## Project Structure
│
├── Constants/ # constants regarding files
│ ├── FilesConstants.cs
|
├── Controllers/ # API endpoints for files, folders, and history
│ ├── FilesController.cs
│ ├── FoldersController.cs
│ └── FileHistoriesController.cs
|
├── DTOs/ # to structure data to and from APIs
│ ├── FileDto.cs
│ ├── FolderDto.cs
│ └── FileHistoryDto.cs
│
├── Models/ # Data models used across the system
│ ├── FileModel.cs
│ ├── FolderModel.cs
│ └── FileHistoryModel.cs
│
├── Services/ # Business logic layer
│ ├── Interfaces/
│ │ ├── IFileService.cs
│ │ ├── IFolderService.cs
│ │ └── IFileHistoryService.cs
│ ├── FileHistoryService.cs
│ ├── FileService.cs
│ └── FolderService.cs
│
├── Repositories/ # Handles file-based data access
│ ├── Interfaces/
│ │ ├── IFolderRepository.cs
│ │ ├── IFileRepository.cs
│ │ └── IFileHistoryRepository.cs
│ ├── FolderRepository.cs
│ ├── FileRepository.cs
│ └── FileHistoryRepository.cs
│
├── Persistence/ # JSON read/write to disk
│ └── JsonPersistenceManager.cs
|
├── Validation/ # To run checks on files and folders
│ ├── FileValidator.cs
│ ├── FolderValidator.cs
│ └── ValidationUtilities.cs
│
└── Program.cs # Entry point and route configuration
