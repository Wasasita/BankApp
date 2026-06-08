to run from terminal use:
dotnet build BankingAPI/BankingAPI.csproj
dotnet run --project BankingAPI/BankingAPI.csproj

or press play/excecute from the Program.cs file

## after adding mongo db, we now need to run in terminal:

$env:MongoDbSettings__ConnectionString="your_local_or_atlas_connection"
$env:MongoDbSettings__DatabaseName="BankApp"
dotnet run

## for the tests
dotnet new xunit -n BankingAPI.Tests

## added a reference

like this:
dotnet add BankingAPI.Tests reference BankingAPI

reason: That creates a link: Tests → can see Controllers, Services, Models

## to test 

run "dotnet test" at the root of the folder. This will use CitiBank.sln to run.   


## for mongo db run :
dotnet add package MongoDB.Driver
dotnet user-secrets init  
dotnet user-secrets set "MongoDbSettings:ConnectionString" "the connection string"

# Start MongoDB (Docker recommended)
docker run -d -p 27017:27017 mongo

# Then run tests
dotnet test BankingAPI.Tests/BankingAPI.Tests.csproj