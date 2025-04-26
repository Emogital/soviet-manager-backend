# Docker Compose Setup for Soviet Manager Backend

## Overview

This document explains how to build and run the Soviet Manager Backend project using Docker Compose.

## Prerequisites

- Docker installed
- Docker Compose installed (comes with Docker Desktop)

## Services

- **DataService** — Port: 8082
- **AuthService** — Port: 8086
- **GameServer** — Port: 8084

## Environment Variables

Required environment variables:

- `JWT_SECRET_KEY`
- `JWT_ISSUER`
- `JWT_AUDIENCE`

Set them in the `.env` file located at the project root (copy from `.env.example` if needed).

## Usage

### Starting Services

```bash
docker-compose up -d
```

- Builds images if not already built.
- Starts containers in detached mode.

### Stopping Services

```bash
docker-compose down
```

- Stops and removes containers, network, etc.

### Viewing Logs

All services:

```bash
docker-compose logs
```

Specific service:

```bash
docker-compose logs dataservice
docker-compose logs authservice
docker-compose logs gameserver
```

## Troubleshooting

- **Ports already in use**: Free the ports or update port mappings in `docker-compose.yml`.
- **Environment variables issues**: Ensure `.env` file exists and is correctly filled.
- **Service connection problems**: All services must be on the `soviet-network`.