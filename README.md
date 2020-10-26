# Introduction
Orleans POC simulating attendance distribution of a real scenario, used on a TDC Recife .Net presentation.

# Getting Started

Create a database named Orleans and run all scripts located on DatabaseSetup folder on the following order:
1 - Main
2 - Clustering
3 - Persistence
4 - Reminders

## Monitoring

Use http://localhost:8080 to access Microsoft Orleans Monitoring Dashboard, where you can see all the requests done, time spent on them and other informations.

# Build and Test

Open it on VS code and follow the next steps:

1. dotnet build DeskOrleans.sln
2. Comment azure connections in DeskOrleans.Client/Program.cs and DeskOrleans.SiloHost/Program.cs
3. Execute SiloHost program
   3.1. dotnet run --project DeskOrleans.SiloHost\DeskOrleans.SiloHost.csproj
4. Execute Client dotnet run --project DeskOrleans.Client\DeskOrleans.Client.csproj
