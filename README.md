# .NET 9 Clean Architecture, REST API, CQRS, Event Sourcing, DDD, SOLID

A sample shop API implementing S.O.L.I.D, Clean Code and CQRS (Command Query Responsibility
Segregation) on .NET 9, with the write side on SQL Server and the read side on MongoDB.

> **Credit.** This project is based on
> [jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
> by Jean Francisco Flores Gatto, and remains MIT licensed under his copyright.

## **Technologies**

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- **EF Compiled Queries** (https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/compiled-queries-linq-to-entities)
- Unit & Integration Tests + xUnit + FluentAssertions + NSubstitute + Bogus
- Polly
- FluentValidation
- MediatR
- Ardalis.Result — the Result pattern used across the application layer
- ASP.NET API Versioning
- CorrelationId
- OpenApi
- **Scalar** - Interactive API Reference from OpenAPI/Swagger (https://github.com/scalar/scalar)
- MiniProfiler
- HealthChecks
- SQL Server
- MongoDB
- Redis (Cache)
- Docker & Docker Compose

## **Architecture**

![CQRS Pattern](img/cqrs-pattern.png "CQRS Pattern")

- Full architecture with responsibility separation concerns, SOLID and Clean Code
- Domain Driven Design (Layers and Domain Model Pattern)
- Domain Events
- Domain Validations
- CQRS
- Event Sourcing
- Unit of Work
- Repository Pattern
- Result Pattern

## Running the application

Copy `.env.example` to `.env` and set your own passwords:

```bash
cp .env.example .env
```

Then build and start everything:

```bash
docker compose up --build
```

Compose waits for SQL Server, MongoDB and Redis to report healthy before starting the API, which
then applies any pending migrations and creates the MongoDB collections on first run.

Open the API reference in a browser, using the port Compose mapped for `shop-webapi`:

```
http://localhost:{port}/scalar/v1
```

To build and test without Docker:

```bash
dotnet build Shop.sln
dotnet test Shop.sln
```

## Endpoints

| Method | Route | Notes |
| --- | --- | --- |
| `POST` | `/api/customers` | Registers a customer |
| `PUT` | `/api/customers` | Updates a customer's e-mail |
| `DELETE` | `/api/customers/{id}` | Deletes a customer |
| `GET` | `/api/customers/{id}` | Reads one customer from the read model |
| `GET` | `/api/customers?pageNumber=1&pageSize=20` | Reads one page of customers, page size capped at 100 |
| `GET` | `/health` | Liveness of SQL Server, MongoDB and Redis |

There is no authentication: every endpoint is public, which is deliberate for a sample.

## MiniProfiler for .NET

To access the page with the performance indicators and performance:

```
http://localhost:{port}/profiler/results-index
```
