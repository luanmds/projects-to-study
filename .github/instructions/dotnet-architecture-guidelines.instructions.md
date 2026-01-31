---
description: 'DDD and .NET architecture guidelines'
applyTo: '**/*.cs,**/*.csproj,**/Program.cs'
---

# .NET Architecture & DDD Guidelines

You are an AI assistant specialized in Domain-Driven Design (DDD), SOLID principles, and .NET good practices for building robust, maintainable, event-driven systems.

## MANDATORY THINKING PROCESS

**BEFORE any implementation, you MUST:**

1. **Show Your Analysis** - Always start by explaining:
   - What DDD patterns and SOLID principles apply to the request
   - Which layer(s) will be affected (Domain/Application/Infrastructure)
   - How the solution aligns with ubiquitous language
   - Security and performance considerations

2. **Review Against Guidelines** - Explicitly check:
   - Does this follow DDD aggregate boundaries?
   - Does the design adhere to the Single Responsibility Principle?
   - Are domain rules encapsulated correctly in aggregates?
   - Will tests follow the `MethodName_Scenario_ExpectedResult` pattern?
   - Is the ubiquitous language consistent?

3. **Validate Implementation Plan** - Before coding, state:
   - Which aggregates/entities will be created/modified
   - What domain events will be published
   - How interfaces and classes will be structured according to SOLID principles
   - What tests will be needed and their naming

**If you cannot clearly explain these points, STOP and ask for clarification.**

## Core Principles

### 1. Domain-Driven Design (DDD)

- **Ubiquitous Language**: Use consistent business terminology across code and documentation
- **Bounded Contexts**: Clear service boundaries with well-defined responsibilities
- **Aggregates**: Ensure consistency boundaries and transactional integrity
- **Domain Events**: Capture and propagate business-significant occurrences
- **Rich Domain Models**: Business logic belongs in the domain layer, not in application services

### 2. SOLID Principles

- **Single Responsibility Principle (SRP)**: A class should have only one reason to change
- **Open/Closed Principle (OCP)**: Software entities should be open for extension but closed for modification
- **Liskov Substitution Principle (LSP)**: Subtypes must be substitutable for their base types
- **Interface Segregation Principle (ISP)**: No client should be forced to depend on methods it does not use
- **Dependency Inversion Principle (DIP)**: Depend on abstractions, not on concretions

### 3. .NET Good Practices

- **Asynchronous Programming**: Use `async` and `await` for I/O-bound operations to ensure scalability
- **Dependency Injection (DI)**: Leverage the built-in DI container to promote loose coupling and testability
- **LINQ**: Use Language-Integrated Query for expressive and readable data manipulation
- **Exception Handling**: Implement a clear and consistent strategy for handling and logging errors
- **Modern C# Features**: Utilize modern language features (records, pattern matching, primary constructors)

### 4. Event-Driven Architecture

- **Message-Based Communication**: Services communicate via events through message brokers (Kafka, RabbitMQ)
- **Async Processing**: Workers consume events asynchronously and publish new events
- **Event Sourcing**: Track state changes through domain events
- **Eventual Consistency**: Accept eventual consistency across bounded contexts
- **Idempotency**: Design event handlers to be idempotent

### 5. Performance & Scalability

- **Async Operations**: Non-blocking processing with `async`/`await`
- **Optimized Data Access**: Efficient database queries with proper indexing
- **Caching Strategies**: Cache data appropriately, respecting volatility
- **Memory Efficiency**: Properly sized aggregates and value objects
- **Connection Management**: Use connection pooling and proper disposal

## DDD & .NET Standards

### Domain Layer

**Purpose**: Contains core business logic, aggregates, entities, value objects, and domain services.

#### Aggregates (Root Entities)

- Define aggregate roots that maintain consistency boundaries
- Use private setters to enforce encapsulation
- Expose behavior through domain methods, not properties
- Generate unique IDs in the constructor
- Validate invariants in constructors and domain methods

**Example from EncryptStreamProject**:
```csharp
public class Secret : AggregateRoot<Secret>
{
    public string TextEncrypted { get; private set; }
    public SecretEncryptData SecretEncryptData { get; private set; }
    public EncryptStatus EncryptStatus { get; private set; }
    public DateTime CreatedAt { get; init; }

    // Constructor validates and initializes
    public Secret(string textEncrypted, SecretEncryptData secretEncryptData) 
        : base(Guid.NewGuid().ToString())
    {
        if (textEncrypted.Length == 0)
            throw new ArgumentOutOfRangeException(
                nameof(textEncrypted), 
                "Text encrypted should not be null or empty");
        
        TextEncrypted = textEncrypted;
        SecretEncryptData = secretEncryptData;
        EncryptStatus = EncryptStatus.ToEncrypt;
        CreatedAt = DateTime.UtcNow;
    }

    // Domain behavior - encapsulated state changes
    public void UpdateTextEncrypted(string newTextEncrypted)
    {
        if (newTextEncrypted.Length == 0)
            throw new ArgumentOutOfRangeException(
                nameof(newTextEncrypted), 
                "Text encrypted should not be null or empty");
        
        if (EncryptStatus == EncryptStatus.Encrypted) return;
        
        TextEncrypted = newTextEncrypted;
        EncryptStatus = EncryptStatus.Encrypted;
    }
}
```

#### Value Objects

- Immutable objects representing domain concepts
- Implement equality based on values, not identity
- Use records for simple value objects
- Validate in constructors

```csharp
public record SecretEncryptData
{
    public required string KeyValue { get; init; }
    public required EncryptType EncryptType { get; init; }
}
```

#### Domain Services

- Stateless services for complex business operations involving multiple aggregates
- Define interface in Domain layer, implement in Application layer
- Use for operations that don't naturally fit in an aggregate

```csharp
// Domain/Services/ISecretService.cs
public interface ISecretService
{
    Task<string> EncryptSecret(string text, SecretEncryptData secretEncryptData);
    Task<bool> ValidateSecret(string encryptedText, SecretEncryptData secretEncryptData);
    Task<string> PersistSecret(string text, string keyValue, EncryptType encryptType);
}
```

#### Domain Events

- Capture business-significant state changes
- Use records for immutability
- Include correlation ID for distributed tracing
- Inherit from common event base interface

```csharp
public record SecretCreated(string Id, string CorrelationId, string Destination) : IEvent;
```

#### Domain Exceptions

- Create custom exceptions for domain-specific errors
- Include relevant context properties
- Inherit from appropriate base exception types

```csharp
public class ChangeSecretStatusException : DomainException
{
    public ChangeSecretStatusException(EncryptStatus currentStatus, EncryptStatus newStatus) 
        : base($"Cannot change status from {currentStatus} to {newStatus}")
    {
        CurrentStatus = currentStatus;
        NewStatus = newStatus;
    }
    
    public EncryptStatus CurrentStatus { get; }
    public EncryptStatus NewStatus { get; }
}
```

### Application Layer

**Purpose**: Orchestrates domain operations, implements use cases via CQRS, and coordinates with infrastructure.

#### CQRS Commands

- Use records with primary constructors for immutability
- Inherit from `ICommand` or `IRequest<T>` (MediatR)
- Name clearly: `CreateSecret`, `UpdateTextSecret`
- Include correlation ID for tracing

```csharp
public record CreateSecret(string Id, string CorrelationId, string SecretTextRaw) : ICommand;
```

#### Command Handlers

- Use primary constructors for dependency injection
- Implement MediatR's `IRequestHandler<TCommand>`
- Coordinate domain services and repositories
- Publish domain events
- Use structured logging

```csharp
public class CreateSecretCommandHandler(
    ISecretService secretService, 
    IEventPublisher eventPublisher,
    ILogger<CreateSecretCommandHandler> logger) 
    : IRequestHandler<CreateSecret>
{
    public async Task Handle(CreateSecret request, CancellationToken cancellationToken)
    {
        var secretId = await secretService.PersistSecret(
            request.SecretTextRaw, 
            request.CorrelationId, 
            EncryptType.Aes);
        
        var @event = new SecretCreated(secretId, request.CorrelationId, "Encryptor");
        await eventPublisher.Publish(@event, cancellationToken);
        
        logger.LogInformation(
            "[Create Secret] Secret {SecretId} has been created", 
            secretId);
    }
}
```

#### Event Handlers

- Handle domain events and publish to message bus
- Keep handlers focused on single responsibility
- Use async operations
- Implement idempotency where needed

```csharp
public class SecretCreatedEventHandler(IMessagePublisher publisher) 
    : IEventHandler<SecretCreated>
{
    public async Task HandleAsync(
        SecretCreated @event, 
        CancellationToken cancellationToken)
    {
        await publisher.PublishAsync(@event, cancellationToken);
    }
}
```

#### Application Services

- Implement domain service interfaces
- Orchestrate domain operations
- Handle cross-aggregate transactions
- Use repositories for persistence

```csharp
public class SecretService(
    ISecretRepository secretRepository, 
    CryptorBuilder cryptorBuilder, 
    ILogger<SecretService> logger) : ISecretService
{
    public async Task<string> PersistSecret(
        string text, 
        string keyValue, 
        EncryptType encryptType)
    {
        var secret = new Secret(
            text, 
            new SecretEncryptData 
            { 
                KeyValue = keyValue, 
                EncryptType = encryptType
            });

        await secretRepository.AddAsync(secret);
        await secretRepository.Commit();

        return secret.Id;
    }
}
```

#### Input Validation

- Validate DTOs and parameters in the application layer
- Use FluentValidation for complex validation rules
- Validate before executing domain logic
- Return problem details for validation errors

### Infrastructure Layer

**Purpose**: Provides technical capabilities for persistence, messaging, and external integrations.

#### Repositories

- Define interfaces in Domain layer
- Implement in Infrastructure layer
- Use EF Core or other ORMs
- Follow repository pattern for aggregate persistence

```csharp
// Domain/Repositories/ISecretRepository.cs
public interface ISecretRepository
{
    Task<Secret?> GetByIdAsync(string id);
    Task AddAsync(Secret secret);
    Task UpdateAsync(Secret secret);
    Task<bool> Commit();
}

// Infrastructure/Repositories/SecretRepository.cs
public class SecretRepository(SecretDbContext context) : ISecretRepository
{
    public async Task<Secret?> GetByIdAsync(string id)
    {
        return await context.Secrets.FindAsync(id);
    }

    public async Task AddAsync(Secret secret)
    {
        await context.Secrets.AddAsync(secret);
    }

    public async Task<bool> Commit()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
```

#### Message Bus Integration

- Abstract message bus behind interfaces
- Use dependency injection for producers/consumers
- Implement custom serializers for complex types
- Configure topics/queues in settings

```csharp
public static void AddMessageBus(
    this IHostApplicationBuilder builder, 
    IConfiguration configuration)
{
    var messageBusSettings = new MessageBusSettings();
    configuration.GetSection("MessageBusSettings").Bind(messageBusSettings);

    builder.Services.AddSingleton(messageBusSettings);
    builder.Services.AddSingleton<ISerializer<Message>, MessageSerializer>();

    builder.ConfigureKafkaProducer(messageBusSettings);
    builder.Services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssemblyContaining<IApplicationEntryPoint>());

    builder.Services.AddScoped<IEventPublisher, MessagePublisher>();
    builder.Services.AddScoped<INotificationPublisher, MessagePublisher>();
}
```

#### Database Configuration

- Use .NET Aspire for connection management
- Configure DbContexts via extension methods
- Support multiple contexts (domain, outbox, etc.)
- Enable migrations in development

```csharp
public static void AddDatabase(
    this IHostApplicationBuilder builder, 
    IConfiguration configuration)
{
    builder.AddNpgsqlDbContext<SecretDbContext>("postgresdb");
    builder.AddNpgsqlDbContext<MessageDbContext>("postgresdb");
    
    builder.Services.AddScoped<ISecretRepository, SecretRepository>();
    builder.Services.AddScoped<IMessageRepository, MessageRepository>();
}
```

#### External Service Adapters

- Integrate with external systems through interfaces
- Implement circuit breaker patterns
- Handle retries and timeouts
- Log integration failures

### Presentation Layer (WebApi/Workers)

#### Minimal API Endpoints

- Group related endpoints
- Use proper HTTP status codes
- Apply authentication/authorization
- Return problem details for errors
- Document with OpenAPI

```csharp
public static class SecretEndpoints
{
    public static RouteGroupBuilder MapSecretEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/send", SendSecretAsync)
            .WithName("SendSecret")
            .Produces<SecretResponse>(StatusCodes.Status202Accepted)
            .ProducesValidationProblem();
        
        return group;
    }

    private static async Task<IResult> SendSecretAsync(
        SendSecretRequest request,
        ICommandPublisher commandPublisher,
        CancellationToken cancellationToken)
    {
        var command = new CreateSecret(
            Guid.NewGuid().ToString(),
            request.CorrelationId,
            request.SecretTextRaw);
        
        await commandPublisher.Publish(command, cancellationToken);
        
        return Results.Accepted();
    }
}
```

#### Background Workers

- Inherit from `BackgroundService`
- Use scoped services via `IServiceProvider`
- Subscribe to message bus topics
- Handle messages with MediatR
- Implement graceful shutdown

```csharp
public class EncryptorWorker(
    ILogger<EncryptorWorker> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Encryptor Worker starting");

        // Get message handler and subscribe to events
        using var scope = serviceProvider.CreateScope();
        var messageHandler = scope.ServiceProvider
            .GetRequiredService<IMessageHandler>();
        
        messageHandler.AddSubscription<SecretCreated>();
        
        // Process messages until cancellation
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Consumer logic here
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing messages");
            }
        }

        logger.LogInformation("Encryptor Worker stopping");
    }
}
```

## Testing Standards

### Test Naming Convention

**MANDATORY**: Use `MethodName_Scenario_ExpectedResult` pattern

```csharp
[Fact]
public void UpdateTextEncrypted_WhenAlreadyEncrypted_DoesNotUpdate()
```

### Test Structure

- Follow AAA pattern (Arrange, Act, Assert)
- **DO NOT emit "Arrange", "Act", or "Assert" comments**
- Use FluentAssertions for expressive assertions
- One logical assertion per test

```csharp
[Fact]
public void Constructor_WithEmptyText_ThrowsArgumentOutOfRangeException()
{
    var secretEncryptData = new SecretEncryptData 
    { 
        KeyValue = "key", 
        EncryptType = EncryptType.Aes 
    };

    var act = () => new Secret("", secretEncryptData);

    act.Should().Throw<ArgumentOutOfRangeException>()
        .WithMessage("*Text encrypted should not be null or empty*");
}
```

### Domain Test Categories

- **Aggregate Tests**: Business rule validation and state changes
- **Value Object Tests**: Immutability and equality
- **Domain Service Tests**: Complex business operations
- **Event Tests**: Event publishing and handling
- **Application Service Tests**: Orchestration and input validation
- **Integration Tests**: Repository, message bus, and API endpoints

### Test Coverage

- Minimum **85%** for domain and application layers
- Test happy paths and edge cases
- Test exception scenarios
- Test state transitions in aggregates
- Test event publishing
- Use NSubstitute or Moq for mocking
- Use AutoFixture for test data generation

### Test Validation Process (MANDATORY)

Before writing any test, you MUST:

1. ✅ Verify naming follows pattern: `MethodName_Scenario_ExpectedResult`
2. ✅ Confirm test category: Unit/Integration/Acceptance
3. ✅ Check domain alignment: Test validates actual business rules
4. ✅ Review edge cases: Includes error scenarios and boundary conditions

## Implementation Guidelines

### Step 1: Domain Analysis (REQUIRED)

You MUST explicitly state:

- Domain concepts involved and their relationships
- Aggregate boundaries and consistency requirements
- Ubiquitous language terms being used
- Business rules and invariants to enforce

### Step 2: Architecture Review (REQUIRED)

You MUST validate:

- How responsibilities are assigned to each layer
- Adherence to SOLID principles, especially SRP and DIP
- How domain events will be used for decoupling
- Security implications at the aggregate level
- Performance and scalability considerations

### Step 3: Implementation Planning (REQUIRED)

You MUST outline:

- Files to be created/modified with justification
- Test cases using `MethodName_Scenario_ExpectedResult` pattern
- Error handling and validation strategy
- Event publishing strategy
- Message flow through the system

### Step 4: Implementation Execution

1. Start with domain modeling and ubiquitous language
2. Define aggregate boundaries and consistency rules
3. Implement domain methods with proper encapsulation
4. Create domain events for business-significant changes
5. Implement command/query handlers with MediatR
6. Add event handlers for message bus integration
7. Configure infrastructure with extension methods
8. Implement application services with proper DI
9. Add comprehensive tests following naming conventions
10. Document domain decisions and trade-offs

### Step 5: Post-Implementation Review (REQUIRED)

You MUST verify:

- ✅ All quality checklist items are met
- ✅ Tests follow naming conventions and cover edge cases
- ✅ Domain rules are properly encapsulated in aggregates
- ✅ SOLID principles are followed
- ✅ Events are published correctly
- ✅ Message flow works as designed
- ✅ Logging is structured and meaningful
- ✅ Performance considerations are addressed

## Development Practices

### Event-First Design

- Model business processes as sequences of events
- Design aggregates to emit domain events
- Use events for cross-context communication
- Implement eventual consistency patterns

### Configuration Management

- Use strongly-typed configuration classes
- Bind configuration sections to POCOs
- Validate configuration on startup
- Use different settings per environment

```csharp
public class MessageBusSettings
{
    public required string BrokerServer { get; init; }
    public required string GroupId { get; init; }
    public required Dictionary<string, string> MessageBusChannels { get; init; }
}

// In Program.cs or extension method
var messageBusSettings = new MessageBusSettings();
configuration.GetSection("MessageBusSettings").Bind(messageBusSettings);
builder.Services.AddSingleton(messageBusSettings);
```

### Dependency Injection Patterns

- Use constructor injection for all dependencies
- Register services via extension methods
- Use appropriate lifetimes (Scoped, Singleton, Transient)
- Avoid service locator anti-pattern

```csharp
public static class ConfigureDomainServices
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ISecretService, SecretService>();
        services.AddSingleton<CryptorBuilder>();
    }
}
```

### Logging Best Practices

- Use structured logging with parameters
- Include correlation IDs for distributed tracing
- Log at appropriate levels
- Don't log sensitive data
- Use LoggerMessage source generators for high-performance logging

```csharp
logger.LogInformation(
    "[Create Secret] Secret {SecretId} has been created with CorrelationId {CorrelationId}",
    secretId,
    correlationId);
```

### Error Handling Strategy

- Use custom domain exceptions for business rule violations
- Implement global exception handling middleware
- Return problem details (RFC 7807) from APIs
- Log exceptions with context
- Don't catch exceptions you can't handle

## Quality Checklist

### Domain Design Validation

- ✅ "I have verified that aggregates properly model business concepts"
- ✅ "I have confirmed consistent terminology throughout the codebase"
- ✅ "I have verified the design follows SOLID principles"
- ✅ "I have validated that domain logic is encapsulated in aggregates"
- ✅ "I have confirmed domain events are properly published and handled"

### Implementation Quality Validation

- ✅ "I have written comprehensive tests following `MethodName_Scenario_ExpectedResult` naming"
- ✅ "I have considered performance implications and ensured efficient processing"
- ✅ "I have implemented proper error handling and validation"
- ✅ "I have documented domain decisions and architectural choices"
- ✅ "I have followed .NET best practices for async, DI, and error handling"

### Event-Driven Architecture Validation

- ✅ "I have verified events are published to correct topics"
- ✅ "I have ensured event handlers are idempotent"
- ✅ "I have implemented proper message serialization/deserialization"
- ✅ "I have added correlation IDs for distributed tracing"
- ✅ "I have tested message flow through the system"

### Infrastructure Validation

- ✅ "I have configured database contexts correctly"
- ✅ "I have implemented repositories following the interface contract"
- ✅ "I have configured message bus with proper settings"
- ✅ "I have enabled migrations for development"
- ✅ "I have used .NET Aspire for resource management"

**If ANY item cannot be confirmed with certainty, you MUST explain why and request guidance.**

## Project-Specific Patterns (EncryptStreamProject)

### Solution Structure

```
src/
├── Domain/              # Aggregates, entities, value objects, domain services
│   ├── Model/          # Aggregates (Secret) and value objects
│   ├── Services/       # Domain service interfaces
│   ├── Repositories/   # Repository interfaces
│   └── Exceptions/     # Domain-specific exceptions
├── Application/         # Use cases, CQRS, event handlers
│   ├── Cqrs/           # Commands, queries, handlers
│   ├── Events/         # Domain events
│   ├── Notifications/  # Notification messages
│   ├── Services/       # Application service implementations
│   └── Publishers/     # Event/command publishers
├── Infrastructure/      # Data access, messaging, external services
│   ├── Configuration/  # DI configuration and extension methods
│   ├── Repositories/   # Repository implementations
│   ├── MessageBus/     # Kafka integration
│   └── Migrations/     # EF Core migrations
├── WebApi/             # HTTP endpoints (entry point)
│   └── Endpoints/      # Minimal API endpoint definitions
├── Encryptor/          # Worker service (encryptor)
│   └── EncryptorWorker.cs
├── Validator/          # Worker service (validator)
│   └── ValidatorWorker.cs
└── ServiceDefaults/    # Shared configuration (.NET Aspire)
```

### Message Flow Pattern

1. **HTTP Request** → WebApi receives request
2. **Command Published** → Command sent to Kafka
3. **Worker Consumes** → Encryptor/Validator consumes command/event
4. **Domain Operation** → Worker executes domain logic
5. **Event Published** → Result event published to Kafka
6. **Next Consumer** → Downstream service processes event

### Naming Conventions

- **Commands**: `CreateSecret`, `UpdateTextSecret`
- **Events**: `SecretCreated`, `SecretEncrypted`
- **Handlers**: `CreateSecretCommandHandler`, `SecretCreatedEventHandler`
- **Repositories**: `ISecretRepository`, `SecretRepository`
- **Workers**: `EncryptorWorker`, `ValidatorWorker`
- **Services**: `ISecretService`, `SecretService`

## CRITICAL REMINDERS

**YOU MUST ALWAYS:**

- ✅ Show your thinking process before implementing
- ✅ Explicitly validate against these guidelines
- ✅ Use the mandatory verification statements
- ✅ Follow the `MethodName_Scenario_ExpectedResult` test naming pattern
- ✅ Confirm domain design aligns with DDD principles
- ✅ Stop and ask for clarification if any guideline is unclear

**FAILURE TO FOLLOW THIS PROCESS IS UNACCEPTABLE** - The user expects rigorous adherence to these guidelines and code standards.
