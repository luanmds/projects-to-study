# Encrypt Secret Project - Study

This project is a consolidation of studies about Architecture and Engineering Software topics and patterns. 

There are microservices patterns implemented with integrated three apps using messages in a Kafka Broker.
There are Enterprise Architecture, Event-Driven and Domain-Driven Design patterns too, above are some of these patterns:
- Domain Logic Patterns
    - Service Layer
    - Domain Model
- Data Source Patterns
    - Data Mapper
- Object-Relational Behavioral Patterns
    - Unit of Work 
- Object-Relational Structural Patterns
    - Identity Field (in Aggregate and Entities from DDD)
- Object-Relational Metadata Mapping Patterns
    - Repository (from Domain-Driven Design)
- Distribution Patterns
    - DTO
- Base Patterns
    - Gateway
    - Mapper
    - Separated Interface (Interfaces between layers)
        - Value Object (with Domain-Driven Design)

## How to run this project

### Before Run the project

- Run **docker-compose.kafka.yml** in **./docker** a directory to start kafka platform and create topics necessary to this project;

- Execute the docker-compose.yaml file in docker folder located in this project root directory;

### To Run this project in your machine using DotNet

- Runs all *.csproj files;
- Open your favorite browser in [http://localhost:5000/swagger] and call root endpoint;
- Follow terminal console logs to see flow working.
