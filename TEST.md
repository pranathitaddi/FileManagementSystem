# File Management System

This is a simple file management system with a RESTful API.

## How to Run

1.  **Run the application**:
    ```bash
    dotnet run
    ```
    The application will be running on `http://localhost:5237`.

## How to Test

You can use any HTTP client to test the API. Here are some examples using `curl`.

### Folders

*   **Create a folder**:
    ```bash
    curl -X POST -H "Content-Type: application/json" -d '{ "name": "My Folder", "parentFolderId": null }' http://localhost:5237/api/folders
    ```

*   **Get folder contents**:
    ```bash
    curl http://localhost:5237/api/folders/contents?folderId=<folder_id>
    ```

*   **Rename a folder**:
    ```bash
    curl -X PUT -H "Content-Type: application/json" -d '{ "newName": "My Renamed Folder" }' http://localhost:5237/api/folders/<folder_id>/rename
    ```

*   **Move a folder**:
    ```bash
    curl -X PUT -H "Content-Type: application/json" -d '{ "newParentFolderId": "<new_parent_folder_id>" }' http://localhost:5237/api/folders/<folder_id>/move
    ```

*   **Delete a folder**:
    ```bash
    curl -X DELETE http://localhost:5237/api/folders/<folder_id>
    ```

### Files

*   **Upload a file**:
    ```bash
    curl -X POST -F "metadata={\"name\": \"my-file.txt\", \"type\": \"text/plain\"};type=application/json" -F "file=@/path/to/your/file.txt" http://localhost:5237/api/files/upload
    ```

*   **Download a file**:
    ```bash
    curl http://localhost:5237/api/files/<file_id>/download -o my-file.txt
    ```

*   **Get file metadata**:
    ```bash
    curl http://localhost:5237/api/files/<file_id>
    ```

*   **Rename a file**:
    ```bash
    curl -X PUT -H "Content-Type: application/json" -d '{ "newName": "my-renamed-file.txt" }' http://localhost:5237/api/files/<file_id>/rename
    ```

*   **Move a file**:
    ```bash
    curl -X PUT -H "Content-Type: application/json" -d '{ "newFolderId": "<new_folder_id>" }' http://localhost:5237/api/files/<file_id>/move
    ```

*   **Delete a file**:
    ```bash
    curl -X DELETE http://localhost:5237/api/files/<file_id>
    ```

### History

*   **Get file history**:
    ```bash
    curl http://localhost:5237/api/history/file/<file_id>