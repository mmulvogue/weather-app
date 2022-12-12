
# Weather service

A web app and api that integrates with OpenWeatherMap API to retrieve the current weather prediction for a requested city.

## Features
The api uses .NET 6 and the web app uses React v18.2

### Api key authentication
This feature was added using a custom authentication handler that extends from `AuthenticationHandler`.
The underlying data store for api keys is just a hard-coded array for simplicity inside the `HardCodedApiKeyStore` class.
The keys are prefixed with `API-` to allow for a simple shape based validation rule to limit data store calls.

### Rate limiter
.NET 7 now comes with rate limiting built-in and there is also an open source library called AspNetCoreRateLimit that provides this feature.
So I would most likely use those two solutions before custom building this. 

However using a library felt like cheating for a technical test 
so I have built a custom solution in this project.

This feature was added using a custom middleware that utilises the api key as a client id and tracks the number of requests made between a fixed time window.
The underlying data store for the client request count is the MemoryCache, however this would likely need a distributed cache like Redis/Memcached in a real solution.

Concurrency issues around updating the client request count have been solved here with a wrapper around the SemaphoreSlim lock as well as key based locking to allow concurrent updates for different api keys. The implementation for the AsyncKeyedLock was derived from these two projects:
 - https://github.com/SixLabors/ImageSharp.Web/
 - https://github.com/stefanprodan/AspNetCoreRateLimit

## Run Locally

The .NET 6 runtime and SDK will need to be installed, I used the version installed by Visual Studio setup. Node.js also needs to be installed, I used v18.12 installed via NVM for windows.

Clone the project

```powershell
  git clone https://github.com/mmulvogue/weather-app.git
```

**Start api**

Open the api project in the clone location
```powershell
  cd <clone-location>\MM.WeatherService.Api
```

Ensure the open weather map api key is set in the `appsettings.Development.json` file
```json
{
  "OpenWeatherMapApiClient": {
    "ApiKey": "<Replace with OpenWeatherMap api key>"
  }
}
```

Start the server

```powershell
  dotnet run
```

**Start web app**

Open the web app project in the clone location
```powershell
  cd <clone-location>\mm-weatherservice-app
```

Install dependencies

```powershell
  npm install
```

Start the server

```powershell
  npm start
```
### From Visual Studio
Open the solution in Visual Studio (I used VS 2022)

Ensure the Start-up projects for the solution is set to current selection

Select the api project (MM.WeatherService.Api) and launch without debugging (Ctrl+F5)

Select the app project (mm-weatherservice-app) and launch without debugging (Ctrl+F5)

## Running Tests

### Api
The integration tests for the api depend on the hard coded apikey values 
and having the rate limiter set to a 5 request limit

To run the all the test suites:

Open the web project 
```powershell
  cd <clone-location>
```

Start the tests
```powershell
  dotnet test
```

### Web app
To run the web app tests:

Open the web project 
```powershell
  cd <clone-location>\mm-weatherservice-app  
```

Start the tests
```powershell
  npm run test
```