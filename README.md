# Auth microservice sample

This repository contains a sample code for the talk "Building an auth microservice with ASP.NET Core Identity and IdentityServer4".

## Projects

### Auth

This is the main project, containing both ASP.NET Core Identity and IdentityServer4 implementation, working together to create a single authentication/authorization service. This project was created using the Web Application template (with Razor Pages) and the following things done after that:

- Scaffolding page for 2FA authenticator app - modified to display qrCode
- Scaffolding page for managing user profile and changing profile image
- Custom `AuthUser` class with `ProfileImageName` property - inherited from `IdentityUser`
- Custom `ApplicationDbContext`, inherited from `IdentityDbContext` and registered through DI, since we have a new user class
- Custom `IEmailSender` implementation with [MimeKit](http://www.mimekit.net/)

The next thing was adding and configuring IdentityServer4, by following [quickstarts](http://docs.identityserver.io/en/latest/quickstarts/0_overview.html).

## Preparing

This project requires .NET Core 3.1 SDK or higher.

### Database connection strings

The default connection string, defined in `appsettings.json` file of both Auth and Auth.Admin projects, assumes that you have SQL Server installed locally, as the default (non-named) instance, and that you will be using `AuthIdentity` as the database. If you have a named instance of SQL Server, or a non-local instance, or need to use another database name, override the setting in the user secrets for both projects:

    "ConnectionStrings": {
      "DefaultConnection": "Server=.;Database=AuthIdentity;Trusted_Connection=True;MultipleActiveResultSets=true"
    }

### Google and IdentityServer Demo external providers

You also need to modify the user secrets for `Auth` project. It should look like this (provide correct [client id and client secret](https://console.cloud.google.com/apis/credentials) for Google auth):

    {
      // ... other settings
      "Providers": {
        "Google": {
          "ClientId": "<google_app_client_id>",
          "ClientSecret": "<google_app_client_secret>"
        },
        "IdentityServerDemo": {
          "ClientId": "native.code",
          "ClientSecret": "secret"
        }
      }
    }

Alternatively, just remove Google (and/or IdentityServer Demo) auth from `Startup` class of Auth project.

### Email sending

If you want to have email sending working, you either need to have a local SMTP server, or modify the SMTP settings in `appsettings.json` file of Auth project. The easiest way to have local SMTP server is to use [smtp4dev](https://github.com/rnwood/smtp4dev). Install it with:

    dotnet tool install -g Rnwood.Smtp4dev --version "3.1.0-*"

Then run it with:

    smtp4dev

It will now capture all emails sent from Auth project. You can see them on https://localhost:5001/.

## Running the solution

_Note: The solution contains multiple web projects, configured to run on specific ports. HTTPS addresses with ports are hard-coded throughout the code, for auth URLs and. The same ports are configured for both IISExpress and Kestrel, so you can use either._

If using Visual Studio 2019+, you can open `Auth.sln` solution. To run multiple projects, right click on the solution in Solution Explorer and choose "Set StartUp Projects...". Select "Multiple" and pick the ones you want to start.

If running from the command line, you can start the projects you need from the root folder, with:

    dotnet run --project src\Auth\Auth.csproj
    dotnet run --project src\Auth.Admin\Auth.Admin.csproj
    dotnet run --project src\Samples.WeatherApi\Samples.WeatherApi.csproj
    dotnet run --project src\Samples.WeatherSummaryApi\Samples.WeatherSummaryApi.csproj
    dotnet run --project src\Samples.WeatherApi.AureliaClient\Samples.WeatherApi.AureliaClient.csproj
    dotnet run --project src\Samples.WeatherApi.ConsoleClient\Samples.WeatherApi.ConsoleClient.csproj
    dotnet run --project src\Samples.WeatherApi.MvcClient\Samples.WeatherApi.MvcClient.csproj
    dotnet run --project src\Samples.WeatherApi.WorkerClient\Samples.WeatherApi.WorkerClient.csproj
    dotnet run --project src\Samples.WeatherApi.WpfClient\Samples.WeatherApi.WpfClient.csproj

If on Windows, there's a convenient PowerShell script to run all web projects at once:

    .\run-web-projects.ps1

## License

See [LICENSE](LICENSE) file.
