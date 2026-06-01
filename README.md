# CitiBank React + C# Template

This workspace contains
- `client/` - React app built with Vite
- `server/` - ASP.NET Core backend with an add-numbers service

## What it does

1.
- Shows a `Hello World` message in the browser
- Calls a C# service to add two numbers
- Displays the sum directly under the greeting
2.
- you can run the console by itself by running only the backend and provide answers in the terminal menu

## Run the frontend

```powershell
cd client
npm install
npm run dev
```

## Run the backend

```powershell
dotnet restore server/Backend.Api.csproj
dotnet run --project server/Backend.Api.csproj
```

The API listens on `http://localhost:5234` in development.
