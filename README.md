# Auth microservice sample

***NOTE: There is [a newer sample](https://github.com/miroslavpopovic/auth-microservice-sample-dotnet6/) with .NET 6 and Duende IdentityServer.***

This repository contains a sample code for the talk "Building an auth microservice with ASP.NET Core Identity and IdentityServer4".

## Presentations

The presentations are in PowerPoint .pptx format.

- [December 2020, WinDays20](2020-12-windays-building-auth-microservice.pptx)
- [July 2020, Seavus CodeTalks](2020-07-seavus-codetalks-building-auth-microservice.pptx)

## Projects

### Auth

This is the main project, containing both ASP.NET Core Identity and IdentityServer4 implementation, working together to create a single authentication/authorization service. This project was created using the Web Application template (with Razor Pages) and the following things done after that:

- Scaffolding page for 2FA authenticator app - modified to display qrCode
- Scaffolding page for managing user profile and changing profile image
- Custom `AuthUser` class with `ProfileImageName` property - inherited from `IdentityUser`
- Custom `ApplicationDbContext`, inherited from `IdentityDbContext` and registered through DI, since we have a new user class
- Custom `IEmailSender` implementation with [MimeKit](http://www.mimekit.net/)

The next thing was adding and configuring IdentityServer4, by following [quickstarts](http://docs.identityserver.io/en/latest/quickstarts/0_overview.html).

### Auth.Admin

An ASP.NET Core application that contains administration for IdentityServer4 clients and resources, which are saved to SQL Server database.

### WeatherApi and WeatherSummaryApi

ASP.NET Core API projects (resources), that clients are connecting to. WeatherSummaryApi demonstrates accessing one API from another.

### ConsoleClient

A .NET console application client for WeatherApi. Demonstrates a simple usage of [IdentityModel](https://github.com/IdentityModel).

### WorkerClient

A .NET worker service client for WeatherApi. Demonstrates the usage of IdentityModel's `AccessTokenManagement`, `HttpClientFactory` and strongly-typed `HttpClient`.

### MvcClient

An ASP.NET Core application which demonstrates several different ways of using access tokens to access protected resources (APIs).

### AureliaClient

A combination of ASP.NET Core backend and Aurelia frontend app demonstrating usage of [`oidc-client`](https://github.com/IdentityModel/oidc-client-js). Note that this approach is not recommended. For alternatives, check the latest "best current practice" on [OAuth 2.0 for Browser-Based Apps](https://datatracker.ietf.org/doc/draft-ietf-oauth-browser-based-apps/?include_text=1).

### WpfClient

A .NET Core WPF application demonstrating another usage of IdentityModel, as well as the device flow. It simulates the device without browser (i.e. Smart TV or gaming console) and displays link, code and QR code for device flow auth.

## Preparing

This solution requires .NET 5.0 SDK or higher.

### Database connection strings

If you are running the project with Project Tye or `docker-compose` (see below), SQL Server will be served as a Docker container and you don't need to install anything else (other than Docker itself).

However, if running via IISExpress or Kestrel, the default connection string, defined in `appsettings.json` file of both Auth and Auth.Admin projects, assumes that you have SQL Server installed locally, as the default (non-named) instance, and that you will be using `AuthIdentity` as the database. If you have a named instance of SQL Server, or a non-local instance, or need to use another database name, override the setting in the user secrets for both projects:

    "ConnectionStrings": {
      "auth-db": "Server=.;Database=AuthIdentity;Trusted_Connection=True;MultipleActiveResultSets=true"
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

If you running via IISExpress or Kestrel, and want to have email sending working, you either need to have a local SMTP server, or modify the SMTP settings in `appsettings.json` file of Auth project. The easiest way to have local SMTP server is to use [smtp4dev](https://github.com/rnwood/smtp4dev). Install it with:

    dotnet tool install -g Rnwood.Smtp4dev

Then run it with:

    smtp4dev

It will now capture all emails sent from Auth project. You can see them on https://localhost:5001/.

If you are running your app with Project Tye or `docker-compose`, you'll have [MailHog](https://github.com/mailhog/MailHog) started as a service instead. The user interface is available at http://localhost:8025/.

## Running the solution

This solution is created to be as flexible as possible, by not imposing one way to run it. It can be run from command line, from Visual Studio, using the `docker-compose`, etc. However, the most easier way to run it is with Microsoft Project Tye.

### Using Project Tye

The purpose of Project Tye is to help with development and deployment of .NET microservice solutions. It is still in preview mode, so you can run into some missing pieces. I.e. a user friendly debugging story and integration with IDEs is not yet done. You can find more info in [project documentation on GitHub](https://github.com/dotnet/tye/tree/master/docs).

First, install the [latest version](https://www.nuget.org/packages/Microsoft.Tye) of Project Tye

    dotnet tool install --global Microsoft.Tye --version <version>

Then you can just run Project Tye from the root of the repository.

    tye run

It will run all the projects and services defined in `./tye.yaml` and serve a dashboard on http://localhost:8000/. From the Tye Dashboard, you can see all running services, open URLs in browser, view logs, etc.

### Using Kestrel or IISExpress

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

### Using Docker

The solution is ready to run with Docker too. It has `Dockerfile` files for each web project and `docker-compose.yml` and `docker-compose.override.yml` scripts for running all web projects.

However, while running all projects and communication between them was easy without Docker since we were using same `localhost`. Running in Docker is a bit tricky since we basically have multiple machines involved and each has its own `localhost` DNS entry. We can use internal Docker network, and refer to each machine through its DNS name, assigned by Docker Compose, but that would work only for machine to machine communication. When we add browser on the host to the mix, things start to fall apart. I.e. if we use `htpps://auth` as Authority in `auth-admin`, it will successfully retrieve OIDC config file, but will redirect the host browser to that address too, for login, and browser will fail, since host is not the part of the same network.

There are multiple ways this can be solved. For instance, we could configure the Docker Compose to use the host network, or we could use `host.docker.internal` DNS entry that Docker Compose creates in Windows `hosts` file (points to the current local IP address of the host), or we could modify DNS entries, etc.

The way it is solved in this repository is by defining a new DNS entry (similar to `host.docker.internal`) in `c:\Windows\system32\drivers\etc\hosts`. That host entry is named `auth.sample.local`. You can (and should) make sure that the entry exists in `hosts` file before running `docker-compose`. This is partially automated. Just run the **`update-hosts-entry.ps1`** script from the repostory root as an admin. It will pick up your current local IP address and create or update the entry in `hosts` file. Note that this works on Windows too. For Linux or Mac, it's even simpler. Just add/update the entry in `/etc/hosts` file.

All web projects have `appsettings.Docker.json` files with settings overrides for Docker environment.

To run everything, either run `docker-compose` project from Visual Studio, or run `docker-compose up` from the command line.

## License

See [LICENSE](LICENSE) file.
