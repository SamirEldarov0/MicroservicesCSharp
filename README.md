# Microservices Architecture with .NET, Kubernetes, and RabbitMQ

This project demonstrates a **microservices-based architecture** built using **ASP.NET Core**, implementing both **synchronous and asynchronous communication patterns**.

The system consists of two independent services that communicate via **REST, gRPC, and an Event Bus**, and are deployed to a **Kubernetes cluster** with supporting infrastructure.

---

# Architecture Overview

The application is built using the **Microservices Architecture Pattern**, where each service is independently deployable and maintains its own data store.

Key architectural patterns implemented:

* Microservices Architecture
* API Gateway Pattern
* REST API Communication
* gRPC Service-to-Service Communication
* Event-driven Architecture using RabbitMQ
* Containerization with Docker
* Container Orchestration with Kubernetes

---

# Services

## Platform Service

Responsible for managing platform data and exposing REST endpoints for platform operations.

Responsibilities:

* Manage platform resources
* Provide REST API endpoints
* Publish events to RabbitMQ when platforms are created
* Store platform data in its own database

Technologies:

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Docker

---

## Command Service

Responsible for managing commands associated with platforms.

Responsibilities:

* Receive platform events from RabbitMQ
* Maintain its own independent database
* Provide gRPC endpoint for synchronous communication
* Handle command creation and retrieval

Technologies:

* ASP.NET Core
* gRPC
* Entity Framework Core
* SQL Server

---

# Communication Patterns

## Synchronous Communication

Two synchronous communication mechanisms are implemented:

### REST API

Used for client-to-service communication.

Example:

Client → Platform Service

### gRPC

Used for **service-to-service communication** for improved performance and efficiency.

Example:

Platform Service → Command Service

Benefits:

* High performance
* Binary serialization
* Strongly typed contracts

---

## Asynchronous Communication

Asynchronous messaging is implemented using **RabbitMQ** as an **Event Bus**.

Workflow:

1. Platform Service creates a new platform
2. Platform Service publishes an event
3. RabbitMQ distributes the event
4. Command Service consumes the event
5. Command Service updates its own database

Benefits:

* Loose coupling
* Scalability
* Fault tolerance

---

# Infrastructure

The system is containerized and deployed to **Kubernetes**.

Infrastructure components include:

* Kubernetes Deployments
* Kubernetes Services
* Ingress Controller
* Persistent Volume Claims
* RabbitMQ Message Broker
* SQL Server Databases

Kubernetes configuration files can be found in the repository under the deployment YAML files.

---

# Technology Stack

Backend:

* ASP.NET Core
* .NET
* Entity Framework Core

Communication:

* REST API
* gRPC
* RabbitMQ

Infrastructure:

* Docker
* Kubernetes

Database:

* SQL Server

---

# Project Structure

```
MicroservicesCSharp
│
├── PlatformService
│   ├── Controllers
│   ├── Data
│   ├── Models
│   └── Services
│
├── CommandService
│   ├── Controllers
│   ├── Data
│   ├── Models
│   └── Services
│
├── Kubernetes
│   ├── platform-dep.yaml
│   ├── commands-dep.yaml
│   ├── rabbitmq-depl.yaml
│   └── ingress-srv.yaml
```

---

# Running the Project

## Prerequisites

Install the following tools:

* .NET SDK
* Docker
* Kubernetes (Docker Desktop / Minikube)
* kubectl

---

## Build and Run with Docker

Build images:

```
docker build -t platformservice .
docker build -t commandservice .
```

---

## Deploy to Kubernetes

Apply Kubernetes configurations:

```
kubectl apply -f .
```

Check deployments:

```
kubectl get pods
kubectl get services
```

---

# Key Learning Objectives

This project demonstrates practical experience with:

* Designing Microservices Architecture
* Implementing API Gateway Pattern
* Building RESTful APIs
* Implementing gRPC communication
* Event-driven architecture using RabbitMQ
* Containerizing services with Docker
* Deploying applications to Kubernetes

---

# Future Improvements

Possible enhancements:

* Add authentication using Identity Server / JWT
* Implement centralized logging
* Add distributed tracing (Jaeger / OpenTelemetry)
* Introduce API Gateway such as YARP or Ocelot
* Implement CI/CD pipeline

---

# Author

Samir Eldarov

Software Developer | .NET Backend | Microservices Architecture
