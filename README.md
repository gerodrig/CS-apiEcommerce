# cs-apiEcommerce

A simple e-commerce API built with C# and .NET 8. This project serves as a basic template for creating a RESTful API using Entity Framework Core for data access and Swagger for documentation.

## Features

*   **RESTful API:** Built on .NET 8 Web API.
*   **Database:** Uses Entity Framework Core with a SQL Server provider.
*   **API Documentation:** Integrated Swagger (OpenAPI) for easy API exploration and testing.
*   **Migrations:** Includes Entity Framework Core migrations to set up the initial database schema.
*   **Docker Support:** Comes with a `docker-compose.yaml` for containerized deployment.

## Prerequisites

Before you begin, ensure you have the following installed:

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   A running instance of [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express).
*   [Docker](https://www.docker.com/products/docker-desktop) (Optional, for running with Docker Compose)

## Getting Started

Follow these steps to get the project up and running on your local machine.

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd cs-apiEcommerce
```

### 2. Configure the Database Connection

You need to set up your database connection string in the `appsettings.Development.json` file. Open the file and modify the `SqlConnection` entry to point to your SQL Server instance.

**Example `appsettings.Development.json`:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "SqlConnection": "Server=your_server_name;Database=cs-apiEcommerceDb;User Id=your_username;Password=your_password;Trusted_Connection=False;Encrypt=True;"
  }
}
```

### 3. Apply Database Migrations

This command will create the database (if it doesn't exist) and apply the initial schema based on the existing migrations.

```bash
dotnet ef database update
```

### 4. Run the Application

Once the database is set up, you can run the application using the .NET CLI.

```bash
dotnet run
```

The API will be running and accessible at `https://localhost:7123` or a similar port displayed in the console output.

### 5. Accessing the API

*   **Swagger UI:** Navigate to `https://localhost:<port>/swagger` in your web browser to view the interactive API documentation.
*   **API Endpoints:** You can make requests to the controllers, for example, `https://localhost:<port>/api/Category`.

## Running with Docker

Alternatively, you can run the application using Docker Compose.

1.  **Ensure Docker is running.**
2.  **Run the following command from the project root:**

    ```bash
    docker-compose up --build
    ```
    
    This will build the Docker image and start the container. The API will be accessible at the port mapped in the `docker-compose.yaml` file.

## Installed NuGet Packages

This project uses the following NuGet packages:

*   **Microsoft.AspNetCore.OpenApi:** For OpenAPI (Swagger) support in ASP.NET Core.
*   **Microsoft.EntityFrameworkCore.Design:** Provides design-time tools for Entity Framework Core.
*   **Microsoft.EntityFrameworkCore.SqlServer:** SQL Server database provider for Entity Framework Core.
*   **Microsoft.EntityFrameworkCore.Tools:** Command-line tools for Entity Framework Core (e.g., for migrations).
*   **Swashbuckle.AspNetCore:** A library for generating Swagger documentation.
