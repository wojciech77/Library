# Library Management System

## Purpose of the Project

The Library Management System is designed to digitalize library records and simplify the process of borrowing books. Users can request books, managers can approve and prepare the orders, and the status of borrowed books can be tracked. When books are returned, they are archived for easy management.

## Features

- **User Registration & Login**: New users can register, log in securely with password hashing, and manage their profiles.
- **Borrowing System**: Users can browse the library, request books, and track the status of their borrow requests.
- **User Management**: Admins and managers can manage user roles and permissions (regular user, manager, admin).
- **Resource Management**: Books and other resources can be added, edited, and managed by library staff.
- **Role-based Access Control**: Different views and actions are available depending on the user's role (User, Manager, Admin).
- **Status Tracking**: Book status changes, including "Waiting for preparation," "Ready for pickup," and "Returned."
- **Session Management**: Users are authenticated using JWT tokens stored in cookies, ensuring secure sessions.

## Technologies and Libraries

- **C#** and **ASP.NET Core** for the backend.
- **Entity Framework** for database access and management.
- **LINQ** for querying data.
- **JWT Authentication** for secure user authentication and session management.
- **FluentValidation** for input validation.
- **Password Hashing** to securely store user credentials.
- **Session and Cookie Management** to handle authentication and user data.
- **Singleton Pattern** for sharing JWT across the application.

## How to Run the Project

1. Clone the repository:
    ```bash
    git clone https://github.com/wojciech77/Library
    ```
2. Navigate to the project directory:
    ```bash
    cd library-management-system
    ```
3. Set up the database:
    - Ensure you have SQL Server installed.
    - Configure the connection string in `appsettings.json`.
    - Run database migrations:
      ```bash
      dotnet ef database update
      ```
4. Run the application:
    ```bash
    dotnet run
    ```

## Future Improvements

- **Email Notifications**: Implement email notifications for borrowing status updates.
- **Advanced Search**: Improve the search functionality for resources.
## Contribution

Contributions are welcome! Please open an issue or submit a pull request with any changes or suggestions.

