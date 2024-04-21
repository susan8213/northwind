# Implementing .NET 8 Web API - Northwind Example

## Environment

- VS Code on Mac
- .NET SDK 8.0
- SQL Server 2022 on Docker Linux

## Database

Using [Northwind sample database](https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs) as Data sources, create a SQL Server database on Docker.

![Entity-Relationship Diagram](https://upload.wikimedia.org/wikiversity/en/a/ac/Northwind_E-R_Diagram.png)

Run SQL Server Docker Container
>`docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Pa$$wordString" -p 1433:1433 -d --name mssql mcr.microsoft.com/mssql/server:2022-latest`

Access SQL Server Container bash
>`docker exec -it mssql bash`

Use `sqlcmd` to Create Database Northwind
> /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P Pa$$wordString

## Initialize Web API Project

Create Webapi Project by using controllers template
>`dotnet new webapi --use-controllers -o Northwind`

Add Nuget Packages
>`dotnet add package Dapper`
>`dotnet add package Microsoft.Data.SqlClient`

Create Test Project
>`dotnet new mstest -o Northwind.Tests`

Add Main Project Reference to Test Project
>`dotnet add reference ../Northwind/Northwind.csproj`

## Features

- Layer structure: **Data Access** (db data / api data) - **Model** (domain object) - **Service** (business logic) - **Controller** (user input/output processing)
- Dependency injection for data access
- Implementing REST API for Order CRUD
- DTO and validating user input
- Global Exception Handling Middleware
- Built-in Logging & HttpLogging
- Unit Test

## Can be Better

- Security. (Authentication, Confidential Data Handling etc.)
- Data Driven Approach like agreggating suppliers business insight.