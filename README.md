# Sample eCommerce System ![CI Build](https://github.com/rexcfnghk/sample-ecommerce/actions/workflows/dotnet.yml/badge.svg)

This is a simple backend system written in C# to simulate an eCommerce system.

## Dependencies

If you are building this locally, you should have the following installed:

1. [.NET SDK 8.0.7](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)
2. [ASP.NET Core Runtime 8.0.7](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
    - For Windows, the [Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.7-windows-hosting-bundle-installer) is recommended
3. [Microsoft SQL Server LocalDB](https://go.microsoft.com/fwlink/?linkid=2215160)

If you are building this via [Docker Compose](https://docs.docker.com/compose/), you should have [Docker Desktop](https://docs.docker.com/engine/install/) installed.

## Settings/Secrets

The solution requires a few settings/secrets to run:

### Running locally

1. A connection string to LocalDB. This can be configured under `appsettings.Development.json`
2. A 128-bit AES Key to encrypt passwords generated for users. This can be configured by:
    1. `cd ./SampleECommerce.Web`
    2. `dotnet user-secrets set "Aes:Key" "{your 128-bit AES key}"` (Note that the key is in the format of comma-separated byte values, e.g. `1,2,3,4...`)
3. A security key used to sign JSON Web Tokens used for authentication. Note that the algorithm used (configured in `appsettings.Development.json`, defaults to `HS256`) controls the number of bits required for the security key. The security key can be configured by:
    1. `cd ./SampleECommerce.Web`
    2. `dotnet user-secrets set "Jwt:SecurityKey" "{your security key}"`

### Running in Docker

Since secrets are passed as [Docker secrets](https://docs.docker.com/engine/swarm/secrets/), you are required to provide the above 3 secrets and 1 additional secret via a file mechanism:

1. (This corresponds the point 1 in Running Locally) You should provide a text file named `connectionstrings_development.txt` under `./` with your connection string to the Microsoft SQL Server 2022 container, as specified in the `docker-compose.yml`.
2. (This corresponds the point 2 in Running Locally) You should provide a text file named `aes_key.txt` under `./` with your AES key in the format of comma-separated byte values, e.g. `1,2,3,4...`.
3. (This corresponds the point 3 in Running Locally) You should provide a text file named `jwt_securitykey.txt` under `./` with your security key used to sign JSON Web Tokens.
4. You should provide a text file named `sql_sa_password.txt` under `./` to configure the password used for the database system administrator (SA). Note that this is subject to Microsoft SQL Server's password requirements (at least 8 characters including uppercase, lowercase letters, base-10 digits and/or non-alphanumeric symbols).

## SQL seed scripts

### Running locally

Before running the application, the seed script should be run to create the tables. This is found under `assets/setup.sql`.

### Running in Docker

A Docker container is automatically run to seed script to seed the database. No manual intervention is needed.

## How to run

### Running locally

`dotnet run SampleECommerce.Web/SampleECommerce.Web.csproj -c Release`

You should note the port that the service is hosted on via the console output.

### Running in Docker

`docker compose up --build`

The web application is preconfigured to run on port `8080` and the Microsoft SQL Server 2022 container is preconfigured to run on port `1433`.

## Features

- Swagger UI at `/swagger`
- Containerised setup `docker compose up`

## Endpoints

When the app is up, visit `{baseUrl}/swagger` for an interactive page to examine endpoints, headers, request, and response formats.

The Postman collection (explained below) also shows the expected HTTP call sequence.

## Tests

### Unit Tests

Unit tests can be run by `dotnet test SampleECommerce.Tests/SampleECommerce.Tests.csproj`.

### Integration Tests

A happy path integration test case can be run by importing the [Postman](https://www.postman.com/) collection under `postman-collections/Sample ECommerce Happy Path Integration Test.postman_collection.json`. The variable `url` should be set to point to URL where the app is listening on.

The collection does the following:

1. Calls the `POST /Users` endpoint with a randomly generated username and password.
2. Calls the `POST /Session` endpoint using `Basic` authentication with the username and password generated from the previous step.
3. Calls the `GET /Users/Orders` endpoint with the signed JWT generated from the previous step to list orders made by the user (expectation: empty).
4. Calls the `POST /Users/Orders` endpoint with a known-to-exist `productId` and `quantity` to create a new order.
5. Calls the `GET /Users/Orders` endpoint again to check whether the order made from the previous step exists and can be returned by the endpoint (expectation: one single order with one single order item).

## Limitations

1. The rows in the [Product] table are assumed to be immutable, i.e. Changes to products after users made an order are not propagated.
2. The local version is running on a self-signed HTTPS certificate which is not suitable for production use.
3. The local version makes use of .NET's secret store is only for development purpose. A more robust key vault should be used in production scenarios.
4. The LocalDB used is only for development purpose. A proper version of Microsoft SQL Server should be used in production scenarios.
5. The signing key used to sign the JWTs could be rotated by using a JSON Web Key Set (JWKS) to mitigate exposed keys being abused to sign new tokens.
6. The JWT generated is currently only signed but not encrypted. Depending on the security requirements this might have to be changed to a JWE.
7. In the containerised setup, the communication is downgraded from HTTPS to HTTP due to the assumption that this backend is sitting behind an API gateway that performs HTTPS downgrading.
8. Some of the logic is heavily coupled with SQL commands/queries, encryption/decryption, or encoding/decoding. Abstractions are built around these impure logic and a best effort is used to unit test the logic that uses impure components.
9. Error handling for this app is, in functional programming terms, monadic instead of applicative. This is because C# and the ASP.NET Core framework is built around object-oriented programming concepts, not functional programming. This means that responses would only indicate the first error encountered when the request contains errors at multiple points of execution. This can be improved with a functional language that supports a [Validation applicative function](https://hackage.haskell.org/package/validation-1.1.3/docs/Data-Validation.html) like Haskell.
