
# Encrypt Stream Project


This project is a practical study of Software Architecture and Engineering patterns, implementing a distributed solution with microservices using .NET 9, Kafka, and PostgreSQL.

## 🎯 Objective


Demonstrates the implementation and integration of various architectural and design patterns in a real scenario, using Kafka messaging for communication between services. The project explores modern .NET practices, CQRS, DDD, and event-driven microservices.

## 🏗️ Architecture


The solution is composed of three main services that communicate asynchronously via Kafka topics, implementing an Event-Driven Architecture:

- **WebApi**: ASP.NET Core Minimal API – Entry point for creating secrets.
- **Encryptor**: Background worker service – Encrypts secrets and publishes events.
- **Validator**: Background worker service – Validates encrypted secrets.

All services share a common domain, application, and infrastructure layer, following clean architecture principles.


## 🧩 Implemented Patterns


### Domain Patterns
- **Service Layer**: Orchestrates domain operations
- **Domain Model**: Rich domain modeling with behaviors and business rules


### Data Access Patterns
- **Data Mapper**: Maps between domain objects and the database
- **Repository**: Abstraction for data access (DDD)
- **Unit of Work**: Transaction and change tracking for objects


### Structural Patterns
- **Identity Field**: Identification of entities and aggregates (DDD)
- **Value Object**: Immutable objects representing domain concepts
- **DTO**: Objects for data transfer between layers


### Integration Patterns
- **Gateway**: Abstraction for external services
- **Mapper**: Conversion between different data representations
- **Separated Interface**: Interfaces for decoupling between layers


## 🚀 Getting Started


### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)


### Environment Setup

1. **Start Kafka**
   ```bash
   cd docker
   docker-compose -f docker-compose.kafka.yml up -d
   ```
   This command starts the Kafka platform and creates the required topics.

2. **Start Dependencies**
   ```bash
   docker-compose up -d
   ```
   This command starts PostgreSQL and other required services defined in `docker-compose.yml`.


### Running the Project

1. **Build the Solution**
   ```bash
   dotnet build EncryptSecretProject.sln
   ```

2. **Start the Services**
   ```bash
   # In separate terminals, run:
   dotnet run --project ./src/WebApi/WebApi.csproj
   dotnet run --project ./src/Encryptor/Encryptor.csproj
   dotnet run --project ./src/Validator/Validator.csproj
   ```

3. **Access the API**
   - Open your browser at [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - Use the Swagger UI to test the endpoints

4. **Monitor Execution**
   - Watch the logs in the console to see the message flow
   - Check the integration between services through Kafka events


## 📚 Additional Documentation

For more details about the implemented patterns and project architecture, see:
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
- [Enterprise Integration Patterns](https://www.enterpriseintegrationpatterns.com/)
- [Microservices Patterns](https://microservices.io/patterns/index.html)


## 🤝 Contributing

Contributions are welcome! Feel free to submit improvements, bug fixes, or new pattern implementations. Open an issue or pull request.


## 📝 License

This project is intended for study and demonstration purposes only.
