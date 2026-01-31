# Custom instructions for Copilot

## Repository Overview
A repository consolidating .NET study projects exploring Architecture, Design Patterns, Microservices, and Messaging patterns.

## EncryptStreamProject - Event-Driven Microservices Study

### Architecture Philosophy
This project demonstrates **Event-Driven Architecture (EDA)** with three loosely-coupled microservices communicating via Kafka:
1. **WebApi** (ASP.NET Minimal API) → publishes events to Kafka
2. **Encryptor** (BackgroundService worker) → consumes events, encrypts secrets, publishes to Kafka
3. **Validator** (BackgroundService worker) → consumes events, validates encrypted secrets

**Key Design Principle**: Services share Domain/Application/Infrastructure layers but run independently. Communication is async-only through Kafka topics (`encryptor-events`, `encryptor-notifications`).

**Technology Stack**: .NET 9, Kafka, PostgreSQL, EF Core, MediatR, .NET Aspire

### Critical Project Structure Patterns

#### Layer Responsibilities (Clean Architecture)
- **Domain** (`src/Domain/`): Aggregates, Entities, Value Objects, Domain Services
  - Example: `Secret` aggregate in [Domain/Model/Secret.cs](EncryptStreamProject/src/Domain/Model/Secret.cs#L8-L27) uses private setters + domain methods (`UpdateTextEncrypted`, `UpdateValidStatus`)
  - Aggregates validate invariants in constructors and throw domain exceptions
  - Rich domain models encapsulate business logic, not anemic DTOs
- **Application** (`src/Application/`): CQRS handlers, Events, Notifications
  - Commands/Queries via MediatR (e.g., `CreateSecretCommandHandler`)
  - Events use `IEventPublisher` → Kafka (e.g., `SecretCreated` event)
  - All handlers registered automatically via `AddMediatR()` in [ConfigureInfrastructure.cs](EncryptStreamProject/src/Infrastructure/Configuration/ConfigureInfrastructure.cs#L33)
- **Infrastructure** (`src/Infrastructure/`): EF Core repositories, Kafka integration, DB migrations
  - All DI configuration in `Configuration/ConfigureInfrastructure.cs` using extension methods
  - Two DbContexts: `SecretDbContext` (domain), `MessageDbContext` (outbox pattern)
  - Repositories implement interfaces from Domain layer

#### CQRS + Kafka Message Flow
```
HTTP POST /send → CreateSecret Command → SecretService.PersistSecret() 
→ SecretCreated Event published to Kafka → Encryptor consumes 
→ EncryptorWorker processes → SecretEncrypted Event → Validator consumes
```
- **Commands** trigger domain operations + publish events to Kafka via `IEventPublisher`
- **Workers** use `IConsumer<string, Message>` with `MessageHandler` to deserialize and route to MediatR
- See [EncryptorWorker.cs](EncryptStreamProject/src/Encryptor/EncryptorWorker.cs#L35-L48) for consumer pattern: `consumer.Consume()` → `messageHandler.GetMessageTypeByName()` → `mediator.Send()`
- **Message Subscription**: Workers register event types after host build:
  ```csharp
  var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
  messageHandler.AddSubscription<SecretCreated>();  // Register event types to handle
  ```

### Essential Developer Workflows

#### Local Development Setup
1. **Start Kafka first** (REQUIRED before running services):
   ```bash
   cd EncryptStreamProject/docker
   docker-compose up -d
   ```
   Creates topics: `encryptor-notifications`, `encryptor-events`

2. **Run services** (choose one method):
   - **VS Code Tasks**: Use pre-configured tasks `build-encryptor`, `build-validator`, `build-webapi`, or `build-all`
   - **Manual**: 
     ```bash
     dotnet run --project src/WebApi/WebApi.csproj
     dotnet run --project src/Encryptor/Encryptor.csproj  
     dotnet run --project src/Validator/Validator.csproj
     ```
   - **.NET Aspire**: Run AppHost project for orchestrated startup with observability dashboard
     ```bash
     dotnet run --project src/EncryptStreamProject.AppHost/EncryptStreamProject.AppHost.csproj
     ```
     Access Aspire dashboard at http://localhost:15888 for service health, logs, traces, and metrics

3. **Database migrations**: Auto-run if `ApplicationSettings.UseMigration=true` in [appsettings.json](EncryptStreamProject/src/WebApi/appsettings.json). See [WebApi/Program.cs](EncryptStreamProject/src/WebApi/Program.cs#L44-L49) for migration logic.

#### Testing Commands
- Run all tests: `dotnet test EncryptStreamProject/EncryptSecretProject.sln`
- Run specific test category: `dotnet test --filter "Layer=Application"`
- Tests use **NSubstitute** for mocking, **AutoFixture** for test data generation, **xUnit** framework
- Example test structure: [CreateSecretCommandHandlerTests.cs](EncryptStreamProject/tests/EncryptSecretProject.UnitTests/Application/CommandHandlers/CreateSecretCommandHandlerTests.cs)
- Test naming: `MethodName__Scenario__ExpectedResult` (e.g., `Handle__WhenPersistsSecretSuccessfully__ShouldSendEvent`)

#### Debugging Tips
- **Kafka messages**: Check topics with `docker exec -it <kafka-container> kafka-console-consumer --topic encryptor-events --from-beginning --bootstrap-server localhost:9092`
- **Database**: Connection string in appsettings.json → `ConnectionStrings:DefaultConnection`
- **Aspire Dashboard**: View distributed traces, logs, and metrics at http://localhost:15888 when running via AppHost

### Project-Specific Conventions

#### Naming & Organization
- **Commands**: `CreateSecret`, `UpdateTextSecret` (records in `Application/Cqrs/Commands/`)
- **Handlers**: `CreateSecretCommandHandler` (ends with `Handler`)
- **Events**: `SecretCreated`, `SecretEncrypted` (inherit from `IEvent` in `Application/Events/`)
- **Repositories**: `ISecretRepository` interface + `SecretRepository` implementation
- **Workers**: `EncryptorWorker`, `ValidatorWorker` (inherit `BackgroundService`)
- **Tests**: `MethodName__Scenario__ExpectedResult` pattern with `[Trait]` attributes for categorization

#### DI Registration Pattern
All services configured via extension methods in `Infrastructure/Configuration/ConfigureInfrastructure.cs`:
- `AddMessageBus()`: Kafka producer/consumer, MediatR, publishers
- `AddDatabase()`: PostgreSQL DbContexts via .NET Aspire (`AddNpgsqlDbContext`)
- `ConfigureDomainServices()`: Domain services registration

**Example DI Extension Pattern**:
```csharp
public static void AddMessageBus(this IHostApplicationBuilder builder, IConfiguration config)
{
    // Register Kafka, MediatR, publishers
    builder.Services.AddSingleton<IEventPublisher, MessagePublisher>();
}
```

#### Message Subscription Pattern (Workers Only)
Workers subscribe to Kafka topics after host build:
```csharp
var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
messageHandler.AddSubscription<SecretCreated>();  // Register event types
messageHandler.AddSubscription<SecretEncrypted>();
```
**Critical**: Events must be registered via `AddSubscription<T>()` before workers can process them.

### Integration & Dependencies

#### Kafka Topics Configuration
- Defined in [docker/docker-compose.yml](EncryptStreamProject/docker/docker-compose.yml)
- Topics: `encryptor-notifications` (notifications), `encryptor-events` (domain events)
- Consumer groups configured in `appsettings.json` → `MessageBusSettings.MessageBusChannels`
- **Bootstrap server**: `localhost:9092` (local) or `kafka:9093` (Aspire/Docker network)

#### Database Access
- EF Core with PostgreSQL (connection string `postgresdb`)
- Two DbContexts: `SecretDbContext` (domain entities), `MessageDbContext` (outbox pattern)
- Migrations in `Infrastructure/Migrations/`
- Auto-migration enabled via `ApplicationSettings.UseMigration` flag in appsettings

#### .NET Aspire Integration
- Service defaults in [EncryptStreamProject.ServiceDefaults](EncryptStreamProject/src/EncryptStreamProject.ServiceDefaults/)
- Observability, health checks via `builder.AddServiceDefaults()` in each service
- AppHost orchestrates service startup order: Postgres → Kafka + topics → Workers/WebApi
- Dashboard at http://localhost:15888 provides distributed tracing, metrics, logs

### Code Guidelines
- **Read first**: `.github/instructions/dotnet-architecture-guidelines.instructions.md` for DDD/SOLID principles
- **C# conventions**: `.github/instructions/csharp-guidelines.instructions.md` for naming, async patterns, modern C# features
- **Testing**: Follow AAA pattern (Arrange-Act-Assert), use `[Trait]` attributes for categorization
- **Domain behavior**: Always encapsulate state changes in domain methods (no public setters)
- **Async naming**: Suffix async methods with `Async` (e.g., `GetByIdAsync`)
- **Records for DTOs**: Use C# records for commands/events/DTOs with `init` properties
- **Immutability**: Prefer `init` properties and readonly collections in domain entities
