# SamplePWA
This is a sample Auto(SSR+CSR) PWA with .Net Core 8.0, Radzen Components, OData, OpenAPI and SQL Server

## How to run
1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Open the `Server/appsettings.json` or `Server/appsettings.Development.json` file and change the `ConnectionStrings.SampleDBConnection` to point to your SQL Server instance

```json
	{
	  "ConnectionStrings": {
		"SampleDBConnection": "Server=localhost;Database=SampleDB;Trusted_Connection=True;"
	  }
	}
```

4. Open the `SampleDB.sql` file and execute it in your SQL Server instance
5. (Optional) Modify `Server/Properties/launchSettings.json` to point to your SQL Server instance

```json
	{
      "iisSettings": {
        "windowsAuthentication": false,
        "anonymousAuthentication": true,
        "iisExpress": {
          "applicationUrl": "http://localhost:19712",
          "sslPort": 44395
        }
      },
      "profiles": {
        "SamplePWA.Server": {
          "commandName": "Project",
          "dotnetRunMessages": true,
          "launchBrowser": true,
          "applicationUrl": "https://*:5001;http://*:5000",
          "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
          }
        },
        "IIS Express": {
          "commandName": "IISExpress",
          "launchBrowser": true,
          "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
          }
        }
      }
    }
```

6. Run the application

```bash
    dotnet run --project Server
```

# Developed by
  [Akash Shah](https://www.buymeacoffee.com/aksbju)
- [GitHub](https://www.github.com/itsalfredakku)
- [Organization](https://www.github.com/devstroop)
- [Website](https://www.devstroop.com)
