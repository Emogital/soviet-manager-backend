# Soviet Manager Backend

## Core Technologies
[![.NET Core](https://img.shields.io/badge/.NET_Core-8.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0)

## Database
[![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-8.0-green)](https://docs.microsoft.com/en-us/ef/core/)
[![Npgsql](https://img.shields.io/badge/Npgsql-8.0.4-blue)](https://www.npgsql.org/)

## Real-Time Communication
[![SignalR](https://img.shields.io/badge/SignalR-8.0-lightgrey)](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-8.0)
[![MessagePack for SignalR](https://img.shields.io/badge/MessagePack_for_SignalR-8.0-orange)](https://docs.microsoft.com/en-us/aspnet/core/signalr/messagepackhubprotocol?view=aspnetcore-8.0)

## Serialization
[![Newtonsoft.Json](https://img.shields.io/badge/Newtonsoft.Json-13.0.3-yellowgreen)](https://www.newtonsoft.com/json)
[![JsonWebTokens](https://img.shields.io/badge/JsonWebTokens-7.6.2-yellow)](https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/master/docs/json-web-tokens.md)

## Testing
[![InMemory](https://img.shields.io/badge/EFCore.InMemory-8.0.6-lightblue)](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=dotnet-core-cli)

## Deployment
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue)](https://www.docker.com/)


Welcome to the Soviet Manager Backend repository. This project is the server-side application for the Soviet Manager game, designed to handle game logic, player interactions, and data management.

## Table of Contents

- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Development Guidelines](#development-guidelines)
  - [Code Style Convention](#code-style-convention)
  - [Branch Naming Convention](#branch-naming-convention)
- [Docker Support](#docker-support)
  - [Docker Compose](#docker-compose)
- [Contribution Guidelines](#contribution-guidelines)
- [License](#license)

## Project Structure

This project is currently under active development. As a result, the project structure is subject to significant changes. The final structure will be documented once the project reaches a more stable state. For now, please note that the organization of files and folders is evolving to best suit the needs of the development process.

We appreciate your understanding and patience as we work towards building a robust and well-organized system.

## Setup Instructions

To set up the project locally, follow these steps:

1. **Clone the repository**:
    ```bash
    git clone https://github.com/emogital/soviet-manager-backend.git
    cd soviet-manager-backend
    ```

2. **Environment configuration and application run**:
    - Create and configure .env file, run docker compose (see [Docker Support](#docker-support)).

## Development Guidelines

### Code Style Convention

To maintain consistency across the codebase, adhere to the code style guidelines outlined in [CodeStyleConvention.md](docs/CodeStyleConvention.md).

### Branch Naming Convention

Follow the branch naming conventions specified in [BranchNamingConvention.md](docs/BranchNamingConvention.md) for creating and managing branches.

## Docker Support

### Docker Compose

Instructions for building, running, and managing all backend services with Docker Compose are provided in [DockerCompose.md](docs/DockerCompose.md).

## License

    The MIT License (MIT)

    Copyright (c) 2024 Emogital

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.