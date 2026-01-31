---
description: 'Guidelines for building C# applications'
applyTo: '**/*.cs'
---

# C# Coding Guidelines

## Version and Language Features

- Always use the latest C# version features (currently C# 13/.NET 9)
- Leverage pattern matching and switch expressions wherever possible
- Use file-scoped namespace declarations
- Use init-only properties and records for immutability
- Apply primary constructors where appropriate
- Use collection expressions for cleaner collection initialization

## General Principles

- Follow SOLID principles and clean code practices
- Prefer composition over inheritance
- Write testable, maintainable code
- Use meaningful names that express intent
- Keep methods small and focused on a single responsibility
- Write clear and concise comments for complex logic, explaining **why** not **what**
- Make only high-confidence suggestions when reviewing code changes
- Handle edge cases and write clear exception handling
- Document usage and purpose of external libraries and dependencies

## Naming Conventions

### Files and Namespaces
- Use PascalCase for namespaces: `MyCompany.MyProduct.MyFeature`
- One class per file, filename matches class name
- Organize files by feature/domain, not by technical concern
- Use file-scoped namespace declarations

### Classes and Interfaces
- **Classes**: PascalCase - `SecretService`, `UserRepository`
- **Interfaces**: PascalCase with `I` prefix - `IRepository`, `IMessageHandler`
- **Abstract classes**: PascalCase, consider `Base` suffix - `EntityBase`
- **Records**: PascalCase - `SecretCreatedEvent`, `UserDto`

### Methods and Properties
- **Methods**: PascalCase, use verbs - `CreateSecret()`, `ValidateInput()`
- **Properties**: PascalCase - `UserId`, `CreatedAt`
- **Private fields**: camelCase with `_` prefix - `_secretRepository`, `_logger`
- **Constants**: PascalCase - `MaxRetryAttempts`, `DefaultTimeout`
- **Local variables**: camelCase - `userId`, `secretValue`

### Async Methods
- Suffix with `Async` - `CreateSecretAsync()`, `GetUserByIdAsync()`
- Always return `Task` or `Task<T>`

## Code Formatting

- Apply code-formatting style defined in `.editorconfig`
- Use file-scoped namespace declarations for cleaner code
- Insert a newline before the opening curly brace of code blocks
- Ensure the final return statement of a method is on its own line
- Use pattern matching and switch expressions wherever possible
- Use `nameof` instead of string literals when referring to member names
- Single-line using directives preferred
- Ensure XML doc comments are created for any public APIs
- Include `<example>` and `<code>` blocks in XML documentation when applicable

## Code Structure

### Domain-Driven Design (DDD) Patterns

#### Entities
```csharp
public class Secret : Entity<Guid>
{
    public string Value { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // Private constructor for EF Core
    private Secret() { }
    
    // Factory method for creation
    public static Secret Create(string value)
    {
        // Validate and create
        return new Secret { Value = value, CreatedAt = DateTime.UtcNow };
    }
    
    // Behavior methods
    public void Encrypt(string encryptedValue)
    {
        // Domain logic here
    }
}
```

#### Value Objects
```csharp
public record SecretValue
{
    public string Value { get; init; }
    
    public SecretValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));
        
        Value = value;
    }
}
```

#### Repositories
```csharp
public interface ISecretRepository
{
    Task<Secret?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Secret secret, CancellationToken cancellationToken = default);
    Task UpdateAsync(Secret secret, CancellationToken cancellationToken = default);
}
```

### Application Layer

#### CQRS Commands
```csharp
public record CreateSecretCommand(string Value, Guid UserId) : ICommand<Guid>;

public class CreateSecretCommandHandler : ICommandHandler<CreateSecretCommand, Guid>
{
    private readonly ISecretRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSecretCommandHandler> _logger;
    
    public CreateSecretCommandHandler(
        ISecretRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSecretCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Guid> HandleAsync(
        CreateSecretCommand command, 
        CancellationToken cancellationToken)
    {
        var secret = Secret.Create(command.Value);
        await _repository.AddAsync(secret, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        _logger.LogInformation("Secret created with ID: {SecretId}", secret.Id);
        
        return secret.Id;
    }
}
```

#### Event Handlers
```csharp
public class SecretCreatedEventHandler : IEventHandler<SecretCreatedEvent>
{
    private readonly IMessagePublisher _publisher;
    
    public SecretCreatedEventHandler(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }
    
    public async Task HandleAsync(
        SecretCreatedEvent @event, 
        CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(@event, cancellationToken);
    }
}
```

### Infrastructure Layer

#### Entity Framework Configuration
```csharp
public class SecretConfiguration : IEntityTypeConfiguration<Secret>
{
    public void Configure(EntityTypeBuilder<Secret> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
        
        builder.HasIndex(s => s.CreatedAt);
    }
}
```

#### Repository Implementation
```csharp
public class SecretRepository : ISecretRepository
{
    private readonly ApplicationDbContext _context;
    
    public SecretRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Secret?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Secrets
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
    
    public async Task AddAsync(
        Secret secret, 
        CancellationToken cancellationToken = default)
    {
        await _context.Secrets.AddAsync(secret, cancellationToken);
    }
}
```

## Best Practices

### Dependency Injection
- Register services in `Program.cs` or extension methods
- Use constructor injection
- Prefer interface abstractions over concrete types
- Use scoped lifetime for DbContext and repositories

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<ISecretRepository, SecretRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICommandHandler<CreateSecretCommand, Guid>, CreateSecretCommandHandler>();
        
        return services;
    }
}
```

### Error Handling
- Use custom exceptions for domain errors
- Use `Result<T>` pattern for operation results when appropriate
- Don't catch exceptions you can't handle
- Log exceptions with context

```csharp
public class SecretNotFoundException : DomainException
{
    public SecretNotFoundException(Guid secretId) 
        : base($"Secret with ID {secretId} was not found")
    {
        SecretId = secretId;
    }
    
    public Guid SecretId { get; }
}
```

### Async/Await
- Always use `async`/`await` for I/O operations
- Pass `CancellationToken` to all async methods
- Don't use `.Result` or `.Wait()` - causes deadlocks
- Use `ConfigureAwait(false)` in library code (not in ASP.NET Core)

### Null Safety
- Use nullable reference types (`<Nullable>enable</Nullable>`)
- Use `?` for nullable types: `string?`, `Secret?`
- Declare variables non-nullable, and check for `null` at entry points
- **Always use `is null` or `is not null`** instead of `== null` or `!= null`
- Trust the C# null annotations - don't add null checks when the type system says a value cannot be null
- Validate parameters with `ArgumentNullException.ThrowIfNull()`
- Use null-coalescing operators: `??`, `??=`

```csharp
public async Task<Secret> GetSecretAsync(Guid id, CancellationToken cancellationToken)
{
    ArgumentNullException.ThrowIfNull(id);
    
    var secret = await _repository.GetByIdAsync(id, cancellationToken);
    
    if (secret is null)
        throw new SecretNotFoundException(id);
    
    return secret;
}
```

### Configuration
- Use strongly-typed configuration classes
- Validate configuration on startup
- Store secrets in user secrets (development) or Azure Key Vault (production)

```csharp
public class KafkaSettings
{
    public required string BootstrapServers { get; init; }
    public required string GroupId { get; init; }
    public required Dictionary<string, string> Topics { get; init; }
}

// In Program.cs
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));
```

### Logging
- Use structured logging with `ILogger<T>`
- Use log levels appropriately (Debug, Information, Warning, Error, Critical)
- Include relevant context in log messages using structured logging parameters
- Don't log sensitive information
- Implement correlation IDs for request tracking in distributed systems

```csharp
_logger.LogInformation(
    "Processing secret encryption for SecretId: {SecretId}, UserId: {UserId}",
    secretId,
    userId);

// For production systems, consider Serilog or other structured logging providers
```

## Validation and Error Handling

### Custom Exceptions
- Use custom exceptions for domain-specific errors
- Inherit from appropriate base exception types
- Include relevant context properties

```csharp
public class SecretNotFoundException : DomainException
{
    public SecretNotFoundException(Guid secretId) 
        : base($"Secret with ID {secretId} was not found")
    {
        SecretId = secretId;
    }
    
    public Guid SecretId { get; }
}
```

### Model Validation
- Use data annotations for simple validation rules
- Implement FluentValidation for complex validation logic
- Customize validation responses for consistent error handling
- Implement global exception handling using middleware
- Use problem details (RFC 7807) for standardized API error responses

### Exception Handling Strategy
- Don't catch exceptions you can't handle
- Log exceptions with sufficient context
- Use specific exception types rather than catching generic `Exception`
- Consider the Result pattern for expected error cases

## Testing Guidelines

### Unit Tests
- Use xUnit as the testing framework
- Follow AAA pattern: Arrange, Act, Assert
- **Do NOT emit "Arrange", "Act", or "Assert" comments** in test code
- Copy existing style in nearby files for test method names and capitalization
- One assertion per test (generally)
- Use meaningful test names: `MethodName_Scenario_ExpectedResult`
- Always include test cases for critical paths of the application
- Use FluentAssertions for expressive assertions

```csharp
public class SecretServiceTests
{
    [Fact]
    public async Task CreateSecret_WithValidValue_ReturnsSecretId()
    {
        // Arrange
        var repository = new Mock<ISecretRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new SecretService(repository.Object, unitOfWork.Object);
        
        // Act
        var result = await service.CreateSecretAsync("test-value");
        
        // Assert
        Assert.NotEqual(Guid.Empty, result);
        repository.Verify(r => r.AddAsync(It.IsAny<Secret>(), default), Times.Once);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task CreateSecret_WithInvalidValue_ThrowsException(string value)
    {
        // Arrange
        var repository = new Mock<ISecretRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new SecretService(repository.Object, unitOfWork.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateSecretAsync(value));
    }
}
```

### Integration Tests
- Use WebApplicationFactory for API tests
- Use test containers for database and message broker
- Clean up test data after each test
- Explain integration testing approaches for API endpoints
- Demonstrate how to test authentication and authorization logic
- Apply test-driven development principles where appropriate

## Data Access Patterns

### Entity Framework Core
- Implement repository pattern when beneficial for abstraction
- Use different providers (SQL Server, PostgreSQL, SQLite, In-Memory) appropriately
- Implement database migrations for schema changes
- Use data seeding for initial or test data
- Follow efficient query patterns to avoid N+1 problems
- Use `AsNoTracking()` for read-only queries to improve performance

### Query Optimization
- Use projection (`Select`) to retrieve only needed columns
- Implement pagination for large data sets
- Use compiled queries for frequently executed queries
- Avoid lazy loading in production code - use explicit or eager loading

## Authentication and Authorization

- Implement authentication using JWT Bearer tokens
- Understand OAuth 2.0 and OpenID Connect concepts
- Implement role-based and policy-based authorization
- Integrate with Microsoft Entra ID (formerly Azure AD) when appropriate
- Secure both controller-based and Minimal APIs consistently
- Use `[Authorize]` attributes or RequireAuthorization() appropriately

## API Development

### API Versioning and Documentation
- Implement API versioning strategies (URL, header, or query string)
- Use Swagger/OpenAPI for API documentation
- Document endpoints, parameters, responses, and authentication requirements
- Apply versioning consistently across controller-based and Minimal APIs
- Create meaningful API documentation for consumers

### Minimal APIs
- Use route groups for organizing related endpoints
- Apply proper HTTP status codes (201 Created, 404 Not Found, etc.)
- Implement OpenAPI metadata with `.WithName()` and `.WithOpenApi()`
- Use proper result types: `Results.Ok()`, `Results.Created()`, etc.

## Minimal APIs (ASP.NET Core)

### Endpoint Definition
```csharp
public static class SecretEndpoints
{
    public static RouteGroupBuilder MapSecretEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateSecretAsync)
            .WithName("CreateSecret")
            .WithOpenApi()
            .Produces<SecretResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        
        group.MapGet("/{id:guid}", GetSecretAsync)
            .WithName("GetSecret")
            .WithOpenApi()
            .Produces<SecretResponse>()
            .Produces(StatusCodes.Status404NotFound);
        
        return group;
    }
    
    private static async Task<IResult> CreateSecretAsync(
        CreateSecretRequest request,
        ICommandHandler<CreateSecretCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateSecretCommand(request.Value, request.UserId);
        var secretId = await handler.HandleAsync(command, cancellationToken);
        
        return Results.CreatedAtRoute(
            "GetSecret",
            new { id = secretId },
            new SecretResponse(secretId));
    }
}
```

## Worker Services (Background Jobs)

```csharp
public class EncryptorWorker : BackgroundService
{
    private readonly ILogger<EncryptorWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public EncryptorWorker(
        ILogger<EncryptorWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Encryptor Worker starting");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider
                    .GetRequiredService<IMessageHandler<SecretCreatedEvent>>();
                
                // Process messages
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing messages");
            }
        }
        
        _logger.LogInformation("Encryptor Worker stopping");
    }
}
```

## Performance Considerations

### Caching Strategies
- Implement in-memory caching for frequently accessed data
- Use distributed caching (Redis) for multi-instance scenarios
- Implement response caching for HTTP responses
- Set appropriate cache expiration policies

### Asynchronous Programming
- Use `async`/`await` for I/O operations
- Understand why asynchronous programming matters for API performance
- Avoid blocking calls (`.Result`, `.Wait()`)
- Pass `CancellationToken` to all async methods

### General Performance
- Use `StringBuilder` for string concatenation in loops
- Use `Span<T>` and `Memory<T>` for high-performance scenarios
- Consider pagination, filtering, and sorting for large data sets
- Use `AsNoTracking()` for read-only EF Core queries
- Implement compression for API responses
- Measure and benchmark API performance regularly

## Deployment and DevOps

### Containerization
- Use .NET's built-in container support: `dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer`
- Understand when to use built-in publishing vs. manual Dockerfile creation
- Optimize container image size using multi-stage builds

### CI/CD and Hosting
- Implement CI/CD pipelines for .NET applications
- Deploy to Azure App Service, Azure Container Apps, or Kubernetes
- Implement health checks and readiness probes
- Use environment-specific configurations for deployment stages
- Implement proper logging and monitoring in production

### Monitoring and Observability
- Integrate with Application Insights for telemetry collection
- Implement custom telemetry and metrics
- Use correlation IDs for distributed tracing
- Monitor API performance, errors, and usage patterns
- Set up alerts for critical issues

## Project Setup and Structure

### Solution Organization
- Guide users through creating .NET projects with appropriate templates
- Explain the purpose of generated files and folders
- Organize code using feature folders or domain-driven design principles
- Demonstrate proper separation of concerns

### Configuration
- Use strongly-typed configuration classes
- Validate configuration on startup
- Store secrets in user secrets (development) or Azure Key Vault (production)
- Understand Program.cs and configuration system in modern ASP.NET Core
- Implement environment-specific settings (Development, Staging, Production)

### Solution Structure

## Documentation

- Add XML documentation comments for public APIs
- Use `<summary>`, `<param>`, `<returns>`, `<exception>` tags
- Include `<example>` and `<code>` blocks for complex APIs
- Document complex business logic with clear explanations of **why**
- Explain design decisions in comments for maintainability

```csharp
/// <summary>
/// Creates a new secret and publishes a domain event.
/// </summary>
/// <param name="value">The secret value to encrypt.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>The ID of the created secret.</returns>
/// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
/// <example>
/// <code>
/// var secretId = await service.CreateSecretAsync("my-secret", cancellationToken);
/// </code>
/// </example>
public async Task<Guid> CreateSecretAsync(
    string value, 
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

## Code Organization

### Solution Structure
```
src/
├── Domain/           # Domain entities, value objects, domain services
├── Application/      # Use cases, commands, queries, handlers
├── Infrastructure/   # Data access, external services, messaging
├── WebApi/          # API endpoints, controllers
├── Worker/          # Background services
└── ServiceDefaults/ # Shared configuration and extensions

tests/
├── UnitTests/       # Unit tests for domain and application logic
├── IntegrationTests/# Integration tests for infrastructure
└── EndToEndTests/   # End-to-end API tests
```

## Additional Tips

- Use records for DTOs and immutable data
- Prefer `sealed` classes when inheritance is not needed
- Use pattern matching and switch expressions for cleaner code
- Leverage init-only properties for immutability
- Use global using directives for commonly used namespaces
- Enable all nullable warnings and treat warnings as errors
- Use primary constructors in C# 12+ for concise class definitions
- Apply collection expressions for cleaner initialization
- Use file-scoped types when appropriate for better organization
- Leverage `nameof` operator to avoid magic strings
- Consider source generators for repetitive boilerplate code

## Best Practices Summary

1. **Always use latest C# features** - Stay current with language improvements
2. **Nullable reference types everywhere** - Use `is null`/`is not null` consistently
3. **No AAA comments in tests** - Structure should be self-evident
4. **XML docs for public APIs** - Include examples and code samples
5. **Structured logging** - Use parameters, not string concatenation
6. **Async all the way** - Never block with `.Result` or `.Wait()`
7. **Problem details for errors** - Standardize API error responses
8. **Performance-first mindset** - Cache, paginate, use AsNoTracking()
9. **Security by default** - Validate, authorize, sanitize
10. **Test critical paths** - Unit tests, integration tests, and E2E tests
