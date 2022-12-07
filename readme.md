
# Weather service

Web app and api that integrates with OpenWeatherMap API to retrieve the current 
weather prediction for a city.

Technical test completed by Michael Mulvogue


## Features

- ASP.NET Core api
    - API key authentication
    - Rate limiting
    - Integration with OpenWeatherMap.com
- React web app
    - Using standalone react app template in Visual Studio
    - Integrates with the api using the http-proxy-middleware library



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

Allow dotnet dev webserver https cert

```powershell
  npm run pre-start
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

