# Encrypt Stream Project

Este projeto é um estudo prático de padrões de Arquitetura e Engenharia de Software, implementando uma solução distribuída com microserviços.

## 🎯 Objetivo

Demonstrar a implementação e integração de diversos padrões arquiteturais e de design em um cenário real, utilizando mensageria com Kafka para comunicação entre serviços.

## 🏗️ Arquitetura

O projeto é composto por três aplicações que se comunicam através de mensagens em um broker Kafka, implementando uma arquitetura orientada a eventos (Event-Driven Architecture).

### Padrões Implementados

#### Padrões de Domínio
- **Service Layer**: Camada de serviços para orquestração de operações de domínio
- **Domain Model**: Modelagem rica de domínio com comportamentos e regras de negócio

#### Padrões de Acesso a Dados
- **Data Mapper**: Mapeamento entre objetos de domínio e banco de dados
- **Repository**: Abstração para acesso a dados (DDD)
- **Unit of Work**: Controle de transações e mudanças em objetos

#### Padrões Estruturais
- **Identity Field**: Identificação de entidades e agregados (DDD)
- **Value Object**: Objetos imutáveis que representam conceitos do domínio
- **DTO**: Objetos para transferência de dados entre camadas

#### Padrões de Integração
- **Gateway**: Abstração para serviços externos
- **Mapper**: Conversão entre diferentes representações de dados
- **Separated Interface**: Interfaces para desacoplamento entre camadas

## 🚀 Como Executar

### Pré-requisitos

- [.NET Core SDK](https://dotnet.microsoft.com/download) (versão mais recente)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Configuração do Ambiente

1. **Configurar o Kafka**
   ```bash
   cd docker
   docker-compose -f docker-compose.kafka.yml up -d
   ```
   Este comando iniciará a plataforma Kafka e criará os tópicos necessários.

2. **Configurar Dependências**
   ```bash
   docker-compose up -d
   ```
   Este comando iniciará os demais serviços necessários definidos no docker-compose.yaml.

### Executando o Projeto

1. **Iniciar os Serviços**
   ```bash
   # Em terminais separados, execute:
   dotnet run --project ./Service1/Service1.csproj
   dotnet run --project ./Service2/Service2.csproj
   dotnet run --project ./Service3/Service3.csproj
   ```

2. **Acessar a API**
   - Abra o navegador em [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - Use a interface Swagger para testar os endpoints

3. **Monitorar a Execução**
   - Acompanhe os logs no console para visualizar o fluxo de mensagens
   - Verifique a integração entre os serviços através dos eventos no Kafka

## 📚 Documentação Adicional

Para mais detalhes sobre os padrões implementados e a arquitetura do projeto, consulte:
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
- [Enterprise Integration Patterns](https://www.enterpriseintegrationpatterns.com/)
- [Microservices Patterns](https://microservices.io/patterns/index.html)

## 🤝 Contribuição

Sinta-se à vontade para contribuir com melhorias, correções ou novos padrões de implementação. Abra uma issue ou envie um pull request.

## 📝 Licença

Este projeto é destinado apenas para fins de estudo e demonstração.
