# CitiBank React + C# Template

This workspace contains a simple full-stack starter:

- `client/` - React app built with Vite
- `server/` - ASP.NET Core backend with an add-numbers service

## What it does

- Shows a `Hello World` message in the browser
- Calls a C# service to add two numbers
- Displays the sum directly under the greeting

## Run the backend

```powershell
dotnet restore server/Backend.Api.csproj
dotnet run --project server/Backend.Api.csproj
```

The API listens on `http://localhost:5234` in development.

## Run the frontend

```powershell
cd client
npm install
npm run dev
```

The Vite dev server proxies `/api` requests to the backend.

## sumary:
to run the C# I use dotnet run
to run the react I une npm run dev

## Notes

- The frontend currently adds `12 + 30` on load.
- You can change the numbers in `client/src/App.jsx`.
