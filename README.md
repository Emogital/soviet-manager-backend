# Soviet Manager Backend

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)  
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue)](https://dotnet.microsoft.com/apps/aspnet)
[![C#](https://img.shields.io/badge/C%23-8.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)  
[![SignalR](https://img.shields.io/badge/SignalR-2.4.1-blue)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![MessagePack](https://img.shields.io/badge/MessagePack-2.1.80-blue)](https://github.com/neuecc/MessagePack-CSharp)  
[![Postgres](https://img.shields.io/badge/Postgres-13-blue)](https://www.postgresql.org/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-6.4.4-blue)](https://docs.microsoft.com/en-us/ef/)  
[![Docker](https://img.shields.io/badge/Docker-19.03-blue)](https://www.docker.com/)

Welcome to the Soviet Manager Backend repository. This project is the server-side application for the Soviet Manager game, designed to handle game logic, player interactions, and data management.

## Table of Contents

- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Development Guidelines](#development-guidelines)
  - [Code Style Convention](#code-style-convention)
  - [Branch Naming Convention](#branch-naming-convention)
- [Docker Support](#docker-support)
- [Contribution Guidelines](#contribution-guidelines)
- [License](#license)

## Project Structure

The repository is structured as follows:

```plaintext
soviet-manager-backend/
├── docs/
│   ├── BranchNamingConvention.md
│   ├── CodeStyleConvention.md
├── src/
│   └── GameServer/
│       ├── Controllers/
│       ├── Properties/
│       ├── appsettings.json
│       ├── Dockerfile
│       ├── GameServer.http
│       ├── Program.cs
│       ├── WeatherForecast.cs
├── .dockerignore
├── .gitattributes
├── .gitignore
├── SovietManagerBackend.sln
```

## Setup Instructions

To set up the project locally, follow these steps:

1. **Clone the repository**:
    ```bash
    git clone https://github.com/your-username/soviet-manager-backend.git
    cd soviet-manager-backend
    ```

2. **Open the solution in Visual Studio**:
    - Open `SovietManagerBackend.sln` in Visual Studio.

3. **Build the solution**:
    - Build the solution to restore the dependencies and compile the code.

4. **Run the project**:
    - Set `GameServer` as the startup project.
    - Run the project using Visual Studio.

## Development Guidelines

### Code Style Convention

To maintain consistency across the codebase, adhere to the code style guidelines outlined in [CodeStyleConvention.md](docs/CodeStyleConvention.md).

### Branch Naming Convention

Follow the branch naming conventions specified in [BranchNamingConvention.md](docs/BranchNamingConvention.md) for creating and managing branches.

## Docker Support

The project includes a Dockerfile for containerization. To build and run the Docker container, use the following commands:

1. **Build the Docker image**:
    ```bash
    docker build -t soviet-manager-backend .
    ```

2. **Run the Docker container**:
    ```bash
    docker run -p 8080:80 soviet-manager-backend
    ```

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
    