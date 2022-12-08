
# Weather service

Web app and api that integrates with OpenWeatherMap API to retrieve the current 
weather prediction for a city.

Technical test completed by Michael Mulvogue


## Features

ASP.NET Core api
- [x] Integration with OpenWeatherMap.com
- [x] Request validation 
- [ ] API key authentication
- [ ] Rate limiting    
- [x] Logging
- [x] Error handling

React web app
- [ ] Setup basic UI with Bootstrap
- [ ] Create form for input to weather api
- [ ] Input validation
- [ ] Integrate with weather api
- [ ] Setup api key auth
- [ ] Error handling

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
  cd clone-location/MM.WeatherService.Api
```

Start the server

```powershell
  dotnet run
```

#### Start web app

Open the web app project in the clone location
```powershell
  cd clone-location/mm-weatherservice-app
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

To run teh api tests, run the following command

```powershell
  cd clone-location/MM.WeatherService.Api
  dotnet test
```

