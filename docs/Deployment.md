# Soviet Manager Backend Deployment Guide

## Prerequisites
- Docker & Docker Compose installed (see Docker docs)
- For local development: `.env` file in project root with all required secrets (`JWT_SECRET_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`, `ADMIN_API_KEY`)
- For server deployment: GitHub secrets configured (JWT_SECRET_KEY, JWT_ISSUER, JWT_AUDIENCE, ADMIN_API_KEY, SSH keys, Docker Hub credentials)

## Compose Setups

### Development/Local Mode (`deploy/local/docker-compose.yml`)
- Builds fresh service images from local source for quick development.
- All service communication goes through Nginx proxy.
- Use from project root:

```bash
cd deploy/local
# Ensure your .env is ready
# Builds and starts all backend services
docker-compose up -d
```

To stop and remove all:
```bash
docker-compose down
```

Log example:
```bash
docker-compose logs
docker-compose logs authservice  # Individual service
```

Check if services are running:
```bash
docker-compose ps
```

### Production/Server/CD Mode (`deploy/remote/docker-compose.yml`)
- Uses latest Docker images from Docker Hub (`emogital/soviet-manager-*`): no build steps, just pull and run.
- Uses an external pre-created Docker network (`soviet-manager-internal`).
- Secrets are provided via GitHub Actions (CD pipeline) - no .env file needed on server.
- Supports `IMAGE_TAG` environment variable to specify image version (defaults to `latest`).

**Deployment via GitHub Actions:**
1. Go to Actions tab in GitHub repository
2. Select "CD Pipeline" workflow
3. Click "Run workflow" button
4. The workflow will automatically deploy to your server

**Manual deployment (if needed):**
```bash
cd /opt/games/soviet-manager
# Set image tag if needed (optional, defaults to latest)
export IMAGE_TAG=latest
# Secrets are provided by GitHub Actions, no manual export needed
docker compose pull
docker compose up -d --remove-orphans
```

To check logs:
```bash
docker compose logs soviet-manager-nginx
# or...
docker compose logs soviet-manager-auth-container
docker compose logs soviet-manager-data-container
docker compose logs soviet-manager-game-container
```

Check if services are running:
```bash
docker compose ps
```

## Configuration File Updates

**Important:** Application code is deployed automatically via Docker images, but configuration files require manual updates on the server.

### Files Requiring Manual Updates

When these files change in the repository, update them on the server:
- `docker-compose.yml` - Service configuration and environment variables
- `nginx.conf` - Routing and proxy configuration

### Update Process

1. Copy the updated file content from `deploy/remote/` to the server
2. Restart services to apply changes:
```bash
cd /opt/games/soviet-manager
docker compose down
docker compose up -d
```

**Note:**
- Production deployment uses images published by the CI pipeline. See README for CI/CD process.
- Secrets are managed via GitHub repository settings, not local files on the server.
- Healthchecks are defined in the compose file but may not be fully implemented yet.

For anything else (branching, code style rules), see referenced documents in the [README](../README.md).