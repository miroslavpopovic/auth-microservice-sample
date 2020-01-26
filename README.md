# Auth microservice sample

This repository contains a sample code for the talk "Building an auth microservice with ASP.NET Core Identity and IdentityServer4".

## Running the solution

The solution contains multiple samples, configured to run using IISExpress on Windows platforms. If you want to run it using Kestrel on any platform, you need to modify `Properties/launchSettings.json` files in all ASP.NET Core projects, in order to use the same ports as specified throughout the code. For instance, in `Auth/Properties/launchSettings.json`, you need to modify `applicationUrl`, under `Auth` section to point to `https://localhost:44396` (same SSL port as for IISExpress, defined above) instead of `https://localhost:5001`.

You also need to modify the user secrets for `Auth` project. It should look like this (provide correct client id and client secret for Google auth, or remove google auth from `Startup` class):

    {
      "Providers": {
        "Google": {
          "ClientId": "",
          "ClientSecret": ""
        },
        "IdentityServerDemo": {
          "ClientId": "native.code",
          "ClientSecret": "secret"
        }
      }
    }

## License

See [LICENSE](LICENSE) file.