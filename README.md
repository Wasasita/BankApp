to run from terminal use:
dotnet build BankingAPI/BankingAPI.csproj
dotnet run --project BankingAPI/BankingAPI.csproj

or press play/excecute from the Program.cs file

## for the tests
dotnet new xunit -n BankingAPI.Tests

## added a reference

like this:
dotnet add BankingAPI.Tests reference BankingAPI

reason: That creates a link: Tests → can see Controllers, Services, Models