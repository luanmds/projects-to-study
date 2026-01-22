# Custom instructions for Copilot

## Repository Context

- A repository to consolidate .Net projects to study many topics like Architecture, Design Patterns, Microservices, Messaging, and more.

## Projects

### EncryptStreamProject

A study project focused on Architecture and Engineering Software patterns, implementing microservices that communicate through Kafka messaging.

#### Key Features
- **Architecture Patterns**: Event-Driven Architecture (EDA), Microservices, CQRS
- **Domain Patterns**: Service Layer, Domain Model, Repository, Value Object, Aggregate
- **Data Patterns**: Data Mapper, Unit of Work, Identity Field
- **Distribution Patterns**: DTO, Gateway, Separated Interface
- **Messaging**: Kafka-based event streaming and notifications

#### Project Structure
The solution consists of:
- **WebApi**: ASP.NET Core Minimal API - Entry point for creating secrets
- **Encryptor**: Background worker service - Encrypts secrets and publishes events
- **Validator**: Background worker service - Validates encrypted secrets
- **Domain Layer**: Core business entities, value objects, and domain services
- **Application Layer**: CQRS commands/queries/events with MediatR
- **Infrastructure Layer**: EF Core repositories, Kafka messaging, PostgreSQL data access

#### Technology Stack
- **.NET 9.0**: Target framework for all projects
- **Entity Framework Core 9.0**: ORM with PostgreSQL provider
- **Kafka (Bitnami 3.5.1)**: Message broker for event streaming
- **MediatR 12.4**: In-process messaging for CQRS pattern
- **PostgreSQL**: Primary data store
- **Serilog**: Structured logging
- **Aspire**: For service defaults and observability
- **xUnit**: Unit testing framework
- **Docker & Docker Compose**: Container orchestration

#### Running the Project
Requires Docker for Kafka infrastructure. See the project's [README.md](../EncryptStreamProject/README.md) for detailed setup instructions.

## Guidelines using .Net/C#

- Follow the guidelines in `.github/instructions/dotnet-guidelines.instructions.md` for writing .Net/C# code.
- Use .NET 9.0 features and modern C# patterns (records, pattern matching, init-only properties)
- Implement CQRS pattern using MediatR for commands, queries, and events
- Use Repository pattern with Unit of Work for data access
- Apply Domain-Driven Design principles (Aggregates, Entities, Value Objects)
- Use Kafka for inter-service communication with proper event sourcing
- Implement proper error handling with custom domain exceptions
- Use structured logging with Serilog
- Enable nullable reference types in all projects
- Write unit tests using xUnit with AAA pattern and follow the instructions in `.github/instructions/unit-tests-guidelines.instructions.md`
