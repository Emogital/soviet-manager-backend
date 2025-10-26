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


Welcome to the Soviet Manager Backend repository. This project is the server-side application for the Soviet Manager game, designed to handle game logic, player interactions, and data management.

## Table of Contents

- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Development Guidelines](#development-guidelines)
  - [Code Style Convention](#code-style-convention)
  - [Branch Naming Convention](#branch-naming-convention)
- [Deployment](#deployment)
- [License](#license)

## Project Structure

This project is currently under active development. As a result, the project structure is subject to significant changes. The final structure will be documented once the project reaches a more stable state. For now, please note that the organization of files and folders is evolving to best suit the needs of the development process.

We appreciate your understanding and patience as we work towards building a robust and well-organized system.

## Setup Instructions

You can run the project in two main ways:

- **Locally (development):** Build and run everything from source code using Docker Compose. Fast and easy for making code changes.
- **On the server (production-like):** Use prebuilt images from Docker Hub (automatically built by the CI/CD pipeline). Just copy `deploy/remote` folder to your server.

For step-by-step commands, see [Deployment Guide](docs/Deployment.md).

## Github Actions Workflows

- **CI Pipeline:** On every PR/merge to `dev`, Docker images are built and pushed to Docker Hub (`emogital` org) with each commit/tag.
- **CD Pipeline:** Deploy is done via the Github Action (manual trigger via Actions), which pulls the latest images and runs the app on your server, all containers managed via Docker Compose.
- **Cleanup:** Old Docker images are cleaned up (keeping latest 3) by a separate workflow (also manual trigger).

## Development Guidelines

- See [Code Style Convention](docs/CodeStyleConvention.md) for style rules.
- See [Branch Naming Convention](docs/BranchNamingConvention.md) for branching strategy.

## Admin API

The GameServer includes an Admin API for monitoring server status and managing operations. This API is secured with API key authentication and provides endpoints to check active rooms and players.

### Quick Start

1. Set the `ADMIN_API_KEY` environment variable (see deployment guide)
2. Make requests to `/api/admin/server-status` with the `X-Admin-Key` header

### Example Usage

```bash
# Check server status
curl -H "X-Admin-Key: your-api-key" \
     http://localhost/api/admin/server-status
```

For detailed documentation, see [Admin API Guide](docs/AdminAPI.md).

## Deployment

Instructions for building, running, and managing all backend services are provided in [Deployment Guide](docs/Deployment.md).

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