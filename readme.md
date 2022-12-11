
# Weather service

Web app and api that integrates with OpenWeatherMap API to retrieve the current 
weather prediction for a city.

Technical test completed by Michael Mulvogue


## Features

ASP.NET Core api
- [x] Integration with OpenWeatherMap.com
- [x] Request validation 
- [ ] Rate limiting - AspNetCoreRateLimiter NuGet package
- [ ] Rate limiting - Own implementation
	- Cache to store counter value per client/api key that expires 1 hr after created
	- MiddleWare to increment counter for each request
	- Locking for counter updates
	- Distributed cache for current counter value
- [x] API key authentication
- [x] Logging
- [x] Error handling

React web app
- [x] Setup basic UI with Bootstrap
- [x] Create form for input to weather api
- [x] Input validation
- [x] Integrate with weather api
- [x] Setup api key auth
- [x] Error handling
- [x] Unit tests

## Run Locally

Clone the project

```powershell
  git clone https://link-to-project
```

### From Visual Studio
Open the solution in Visual Studio (I used VS 2022)

Launch the api project (MM.WeatherService.Api) without debugging (Ctrl+F5)

Launch the app project (mm-weatherservice-app) without debugging (Ctrl+F5)


### From command line

#### Start api

Open the api project in the clone location
```powershell
  cd <clone-location>/MM.WeatherService.Api
```

Start the server

```powershell
  dotnet run
```

#### Start web app

Open the web app project in the clone location
```powershell
  cd <clone-location>/mm-weatherservice-app
```

Install dependencies

```powershell
  npm install
```

Start the server

```powershell
  npm run start
```


## Running Tests

### Api
To run the api tests, run the following

Open the web project 
```powershell
  cd <clone-location>
```

Start the tests
```powershell
  dotnet test
```

### Web app
To run the web app tests

Open the web project 
```powershell
  cd <clone-location>/mm-weatherservice-app  
```

Start the tests
```powershell
