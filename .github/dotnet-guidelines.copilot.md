# .NET/C# Coding Guidelines for GitHub Copilot

## General Principles

- Follow SOLID principles and clean code practices
- Prefer composition over inheritance
- Write testable, maintainable code
- Use meaningful names that express intent
- Keep methods small and focused on a single responsibility

## Naming Conventions

### Files and Namespaces
- Use PascalCase for namespaces: `MyCompany.MyProduct.MyFeature`
- One class per file, filename matches class name
- Organize files by feature/domain, not by technical concern

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
- Validate parameters with `ArgumentNullException.ThrowIfNull()`
- Use null-coalescing operators: `??`, `??=`

```csharp
public async Task<Secret> GetSecretAsync(Guid id, CancellationToken cancellationToken)
{
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
- Include relevant context in log messages
- Don't log sensitive information

```csharp
_logger.LogInformation(
    "Processing secret encryption for SecretId: {SecretId}, UserId: {UserId}",
    secretId,
    userId);
```

## Testing Guidelines

### Unit Tests
- Use xUnit as the testing framework
- Follow AAA pattern: Arrange, Act, Assert
- One assertion per test (generally)
- Use meaningful test names: `MethodName_Scenario_ExpectedResult`

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

- Use `StringBuilder` for string concatenation in loops
- Use `async` methods for I/O operations
- Consider pagination for large data sets
- Use `AsNoTracking()` for read-only EF Core queries
- Implement caching for frequently accessed data
- Use `Span<T>` and `Memory<T>` for high-performance scenarios

## Documentation

- Add XML documentation comments for public APIs
- Use `<summary>`, `<param>`, `<returns>`, `<exception>` tags
- Document complex business logic

```csharp
/// <summary>
/// Creates a new secret and publishes a domain event.
/// </summary>
/// <param name="value">The secret value to encrypt.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>The ID of the created secret.</returns>
/// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
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
- Use pattern matching and switch expressions
- Leverage init-only properties for immutability
- Use global using directives for commonly used namespaces
- Enable all nullable warnings and treat warnings as errors
