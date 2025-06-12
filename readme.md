# HttpClient example

## How to run

- Start web api

```pwsh
dotnet run --project .\web_api\Web_api.csproj
```

- Start console cli

```pwsh
dotnet run --project .\src\cli\Cli.csproj [command] [options]
```

## Commands available

- good/g
	- Options:

		- --count/-c
- bad/b
	- Options:
		- --count/-c

- exhaust/eh

### Examples

Help

```pwsh
./Cli
```

Web requests with resilience (500 requests)

```pwsh
./Cli g -c 500
```

Web requests without resilience (500 requests)

```pwsh
./Cli bad --count 500
```

Cause a socket exhaustion

```pwsh
./Cli eh
```

## Technologies used

- EnvironmentAbstractions
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Http
- Microsoft.Extensions.Options
- Spectre.Console.Cli
- StyleCop.Analyzers