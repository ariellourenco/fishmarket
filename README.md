# FISH MARKET

This is a Fish Market application through which fish market brokers can evaluate
the current instantaneous market rates of various fishes. 

The application features:

- FishMarket.Web - An ASP.NET Core hosted Blazor WASM front end application.
- [FishMarket.Api](src/FishMarket.Api/) - An ASP.NET Core REST API backend using minimal APIs.

It showcases:

- Blazor WebAssembly
- Minimal APIs
- Using EntityFramework and SQLite for data access
- OpenAPI
- User management with ASP.NET Core Identity
- Cookie authentication
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