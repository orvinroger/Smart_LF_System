# Smart Lost & Found System

## Overview
The Smart Lost & Found System is a web application designed to help users report and find lost or found items. The application allows users to create reports for lost or found items, view existing reports, and manage their claims.

## Project Structure
The project is organized into several key folders:

- **Controllers**: Contains the logic for handling user requests and returning appropriate views.
  - `HomeController.cs`: Manages the home page and navigation.
  - `ItemsController.cs`: Handles operations related to lost and found items.

- **Data**: Contains the database context for interacting with the SQL Server database.
  - `ApplicationDbContext.cs`: Manages database connections and item data.

- **Models**: Defines the data structures used in the application.
  - `LostFoundItem.cs`: Represents a lost or found item.
  - `ErrorViewModel.cs`: Used for error handling in views.

- **Views**: Contains the Razor views for rendering HTML.
  - `Home/Index.cshtml`: The home page view.
  - `Items/Index.cshtml`: Lists all reported items.
  - `Items/Create.cshtml`: Form for creating new item reports.
  - `Items/Details.cshtml`: Displays details of a specific item.
  - `Shared/_Layout.cshtml`: Main layout for the application.
  - `Shared/_ValidationScriptsPartial.cshtml`: Client-side validation scripts.

- **wwwroot**: Contains static files such as CSS and JavaScript.
  - `css/site.css`: Custom styles for the application.
  - `js/site.js`: Custom scripts for the application.

- **Configuration Files**:
  - `appsettings.json`: Configuration settings, including the database connection string.
  - `Program.cs`: Entry point of the application.
  - `SmartLostAndFound.csproj`: Project file defining dependencies and settings.

## Getting Started
To get started with the Smart Lost & Found System:

1. Clone the repository to your local machine.
2. Open the project in your preferred IDE.
3. Restore the NuGet packages.
4. Update the connection string in `appsettings.json` to point to your SQL Server database.
5. Run the application and navigate to the home page.

## Features
- User-friendly interface for reporting lost and found items.
- Ability to view and manage existing reports.
- Responsive design using Bootstrap for mobile compatibility.

## Future Enhancements
- User authentication and authorization.
- Notifications for users when items are claimed.
- Search functionality for reported items.

## License
This project is licensed under the MIT License.