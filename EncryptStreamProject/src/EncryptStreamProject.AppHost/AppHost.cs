var builder = DistributedApplication.CreateBuilder(args);

// Add Database Resource
var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithImage("postgres:latest")
    .WithDataVolume("")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_DB", "encrypt_db");

var postgresdb = postgres.AddDatabase("postgresdb");

// Add Kafka Resource
var kafka = builder.AddKafka("kafka")
    .WithKafkaUI(kafkaUI => kafkaUI.WithHostPort(19000));

// Add Kafka topic initialization
var initKafka = builder.AddContainer("init-kafka", "confluentinc/cp-kafka", "6.1.1")
    .WithArgs("/bin/sh", "-c",
        "kafka-topics --bootstrap-server kafka:s9093 --list && " +
        "echo -e 'Creating kafka topics' && " +
        "kafka-topics --create --topic encryptor-notifications --partitions 1 --replication-factor 1 --bootstrap-server kafka:s9093 --if-not-exists && " +
        "kafka-topics --create --topic encryptor-events --partitions 1 --replication-factor 1 --bootstrap-server kafka:s9093 --if-not-exists && " +
        "echo -e 'Successfully created the following topics:' && " +
        "kafka-topics --bootstrap-server kafka:s9093 --list")
    .WithReference(kafka)
    .WaitFor(kafka);

builder.AddProject<Projects.Validator>("validator")
    .WithReference(kafka)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitFor(initKafka);

builder.AddProject<Projects.Encryptor>("encryptor")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(kafka)
    .WaitFor(initKafka);

builder.AddProject<Projects.WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(kafka)
    .WaitFor(initKafka);

builder.Build().Run();
