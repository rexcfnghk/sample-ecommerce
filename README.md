# Sample eCommerce System

This is a simple backend system written in C# to simulate an eCommerce system.

## Dependencies

If you are building this locally, you should have the following installed:

1. [.NET SDK 8.0.7](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)
2. [ASP.NET Core Runtime 8.0.7](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
    - For Windows, the [Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.7-windows-hosting-bundle-installer) is recommended
3. [Microsoft SQL Server LocalDB](https://go.microsoft.com/fwlink/?linkid=2215160)

## Settings/Secrets

The solution requires two settings/secrets to run:

1. A connection string to LocalDB. This can be configured under `appsettings.Development.json`
2. A 128-bit AES Key to encrypt passwords generated for users. This can be configured by:
    1. `cd ./SampleECommerce.Web`
    2. `dotnet user-secrets set "Aes:Key" "{your 128-bit AES key}"` (Note that the key is in the format of a comma-separated string, e.g. `1,2,3,4...`)

## SQL seed scripts

Before running the application, the seed script should be run to create the tables. This is found under `assets/seed-scripts.sql`.

## Features

- Swagger UI at `/swagger`
- Containerised setup `docker compose up`

## Endpoints

TBD

## Limitations

1. The secret store used is only for development purpose. A more robust key vault should be used in production scenarios.
2. The LocalDB used is only for development purpose. A proper version of Microsoft SQL Server should be used in production scenarios.
