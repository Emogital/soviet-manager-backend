# Soviet Manager Backend Deployment Guide

## Prerequisites
- Docker & Docker Compose installed (see Docker docs)
- For local development: `.env` file in project root with all required secrets (`JWT_SECRET_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`)
- For server deployment: GitHub secrets configured (JWT_SECRET_KEY, JWT_ISSUER, JWT_AUDIENCE, SSH keys, Docker Hub credentials)

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
docker-compose logs authservice-container  # Individual service
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

**Note:**
- Do not edit Dockerfiles or compose files in server deployment; they are for reference only.
- Production deployment assumes images are published by the CI pipeline. See README for CI/CD/cleanup process.
- Secrets are managed via GitHub repository settings, not local files on the server.
- Healthchecks are defined in the remote compose file but may not be fully implemented in the services yet.

For anything else (branching, code style rules), see referenced documents in the [README](../README.md).