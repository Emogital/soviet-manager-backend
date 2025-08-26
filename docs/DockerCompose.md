# Docker Compose Setup

## Overview

Two deployment configurations:
- `deploy/local/` - Development (builds from source)
- `deploy/remote/` - Production (pre-built images)

## Local Development

**Prerequisites:** Docker, Docker Compose

**Setup:**
```bash
# Configure environment
cp .env.example .env
# Edit .env with JWT settings

# Start services
cd deploy/local
docker-compose up -d

# Test
curl http://localhost/health
```

**Services:**
- AuthService: Build from `src/AuthService`, internal port 8086
- DataService: Build from `src/DataService`, internal port 8082  
- GameServer: Build from `src/GameServer`, internal port 8084
- Nginx: Proxy on port 80

**Commands:**
```bash
# View logs
docker-compose logs -f [service]

# Rebuild after code changes
docker-compose up -d --build [service]

# Stop
docker-compose down
```

## Production Deployment

**Prerequisites:** Docker, access to `emogital/*` images

**Setup:**
```bash
# Environment variables
JWT_SECRET_KEY=production-secret
JWT_ISSUER=SovietManagerProd  
JWT_AUDIENCE=SovietManagerUsers
IMAGE_TAG=latest

# Create network
docker network create soviet-manager-internal

# Deploy
cd deploy/remote
docker-compose up -d
```

**Services:**
- Uses pre-built images: `emogital/soviet-manager-{auth,data,game}`
- All services on internal port 80
- Health checks enabled
- Auto-restart policies
- External network isolation

**Commands:**
```bash
# Update deployment
docker-compose pull && docker-compose up -d

# Health check
curl http://server/nginx-health

# Rollback
IMAGE_TAG=previous-version docker-compose up -d
```

## Environment Variables

Required for both environments:
- `JWT_SECRET_KEY` - JWT signing key
- `JWT_ISSUER` - Token issuer
- `JWT_AUDIENCE` - Token audience

Production only:
- `IMAGE_TAG` - Docker image version (default: latest)

## Networks

**Local:** `backend-network` (bridge)
**Production:** `soviet-manager-internal` (external)

## Troubleshooting

**Port conflicts:** Change port mapping in docker-compose.yml
**Build failures:** Check Docker daemon, disk space
**Health check failures:** Review service logs
**Image pull issues:** Verify registry access, network connectivity