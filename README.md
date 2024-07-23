# Sample eCommerce System ![CI Build](https://github.com/rexcfnghk/sample-ecommerce/actions/workflows/dotnet.yml/badge.svg)

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
3. A security key used to sign JSON Web Tokens used for authentication. Note that the algorithm used (configured in `appsettings.Development.json`, defaults to `HS256`) controls the number of bits required for the security key. The security key can be configured by:
    1. `cd ./SampleECommerce.Web`
    2. `dotnet user-secrets set "Jwt:SecurityKey" "{your security key}"`

## SQL seed scripts

Before running the application, the seed script should be run to create the tables. This is found under `assets/seed-scripts.sql`.

## Features

- Swagger UI at `/swagger`
- Containerised setup `docker compose up`

## Endpoints

TBD

## Limitations

1. The rows in the [Product] table are assumed to be immutable, i.e. Changes to products after users made an order are not propagated.
2. The secret store used is only for development purpose. A more robust key vault should be used in production scenarios.
3. The LocalDB used is only for development purpose. A proper version of Microsoft SQL Server should be used in production scenarios.
4. The signing key used to sign the JWTs could be rotated by using a JSON Web Key Set (JWKS) to mitigate exposed keys being abused to sign new tokens.
5. The JWT generated is currently only signed but not encrypted. Depending on the security requirements this might have to be changed to a JWE.
6. The JWT authentication is currently configured to not validate the `issuer` nor the `audience`. This might be vulnerable to a [forwarding attack](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens.tokenvalidationparameters.validateissuer?view=msal-web-dotnet-latest).
