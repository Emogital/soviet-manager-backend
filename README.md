# Soviet Manager Backend

Microservices backend for Soviet Manager game built with .NET 8.

## Architecture

- **AuthService** - JWT authentication and user management
- **DataService** - Game data persistence and business logic  
- **GameServer** - Real-time gameplay via SignalR
- **Nginx** - Reverse proxy

## Project Structure

```
src/
├── AuthService/      # Port 8086
├── DataService/      # Port 8082
└── GameServer/       # Port 8084
deploy/
├── local/           # Development environment
└── remote/          # Production deployment
```

## Quick Start

1. Clone repository
2. Copy `.env.example` to `.env` and configure JWT settings
3. Run development environment:
   ```bash
   cd deploy/local
   docker-compose up -d
   ```
4. API available at http://localhost

## API Endpoints

- `/auth/` - Authentication service
- `/data/` - Data management service  
- `/game/` - Game server with WebSocket support

## Deployment

Two deployment configurations:

- **Local** (`deploy/local/`) - Builds from source, development/testing
- **Production** (`deploy/remote/`) - Pre-built images from registry

See [docs/DockerCompose.md](docs/DockerCompose.md) for detailed setup instructions.

## Development

- Follow [docs/BranchNamingConvention.md](docs/BranchNamingConvention.md)
- Code style: [docs/CodeStyleConvention.md](docs/CodeStyleConvention.md)
- Docker deployment: [docs/DockerCompose.md](docs/DockerCompose.md)

## Technologies

- .NET 8, ASP.NET Core
- Entity Framework Core, Npgsql
- SignalR, JWT
- Docker, Nginx

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