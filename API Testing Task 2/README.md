## How to run
dotnet test

## Configuration
Edit appsettings.json:
- Api:BaseUrl
- Api:ApiKeyHeader / Api:ApiKeyValue
- Api:TimeoutSeconds
- Logging:LogFile / Logging:LogLevel

## Structure
- src/Petstore.Client: HTTP + typed clients + models + settings
- tests/Petstore.Tests: fixtures + utils + tests

## Logging
Each request logs:
- method, url, payload
- status code, response body (truncated)
- duration (ms)
- correlation id per test

## Known issues
Petstore is a public shared demo API. Collisions and flaky behavior may occur.
