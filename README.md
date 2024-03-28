# FISH MARKET

This is a Fish Market application through which fish market brokers can evaluate
the current instantaneous market rates of various fishes. 

The application features:

- [FishMarket.Api](src/FishMarket.Api) - An ASP.NET Core REST API backend using minimal APIs.

It showcases:

- Minimal APIs
- Using EntityFramework and SQLite for data access
- OpenAPI
- User management with ASP.NET Core Identity
- JWT authentication
- Writing integration tests for your REST API

## Prerequisites

### .NET

1. [Install .NET 8](https://dotnet.microsoft.com/en-us/download)

### Database

1. Install the **dotnet-ef** tool: `dotnet tool install dotnet-ef -g`
2. Navigate to the `FishMarket.Api` folder.
    - Run `mkdir .db` to create the local database folder.
    - Run `dotnet ef database update` to create the database.

### JWT 

1. To initialize the keys for JWT generation, run `dotnet user-jwts` in to [FishMarket.Api](src/FishMarket.Api) folder:

    ```
    dotnet user-jwts create
    ```
## Running the application

To run the application, you can run the [FishMarket.Api](src/FishMarket.Api)
    
   - **Terminal/CLI** - Open up the terminal window, in the project folder and run: 
   
      ```
      dotnet watch run -lp https
      ```

      This will run the application with the `https` profile.

### Using the API standalone
The FishMarket REST API can run standalone as well. You can run the [FishMarket.Api](src/FishMarket.Api) project and make requests to various endpoints using the Swagger UI (or a client of your choice):


Before executing any requests, you need to create a user and get an auth token.

1. To create a new user, run the application and POST a JSON payload to `/users` endpoint:

    ```json
    {
      "email": "user@example.com",
      "password": "<put a password here>"
    }
    ```
2. Get the returned token and add it to the AUTHORIZE window. You should be able to use this token to make authenticated requests to the protected endpoints.

> [!WARNING] 
> Due to a bug on Swagger UI you cannot test the file uplaod endpoint using it. See [https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2625](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2625) for further details.