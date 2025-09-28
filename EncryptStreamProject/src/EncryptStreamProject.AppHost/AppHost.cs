var builder = DistributedApplication.CreateBuilder(args);

// Add Database Resource
var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithImage("postgres:latest")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_DB", "encrypt_db");
    
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.Validator>("validator")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.Encryptor>("encryptor")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.Build().Run();
