# Deployment Guide

## Workflow

1. Development: Work locally with `deploy/local/`
2. CI/CD: Build and push images to `emogital/*` registry
3. Production: Deploy with `deploy/remote/`

## Development Deployment

```bash
cd deploy/local
docker-compose up -d --build
```

Changes require rebuilding affected services.

## Production Deployment

### Initial Setup

```bash
# Server prerequisites
curl -fsSL https://get.docker.com | sh
usermod -aG docker $USER

# Create deployment directory
mkdir -p /opt/soviet-manager
cd /opt/soviet-manager

# Copy remote deployment files
# (scp deploy/remote/* to server)

# Configure environment
cat > .env << EOF
JWT_SECRET_KEY=your-production-secret
JWT_ISSUER=SovietManagerProd
JWT_AUDIENCE=SovietManagerUsers
IMAGE_TAG=latest
EOF

# Create network
docker network create soviet-manager-internal
```

### Deploy/Update

```bash
cd /opt/soviet-manager
docker-compose pull
docker-compose up -d
```

### Health Check

```bash
curl http://localhost/nginx-health
docker-compose ps
```

## Image Management

**Registry:** `emogital/soviet-manager-{auth,data,game}`

**Deploy specific version:**
```bash
IMAGE_TAG=v1.2.0 docker-compose up -d
```

**Rollback:**
```bash
IMAGE_TAG=previous-version docker-compose up -d
```

## Monitoring

**Logs:**
```bash
docker-compose logs -f [service]
```

**Status:**
```bash
docker-compose ps
docker stats
```

## Automation

**Deployment script:**
```bash
#!/bin/bash
cd /opt/soviet-manager
docker-compose pull
docker-compose up -d
sleep 30
curl -f http://localhost/nginx-health || exit 1
docker image prune -f
```

**Health check cron:**
```bash
*/5 * * * * curl -f http://localhost/nginx-health || systemctl restart docker
```

## Troubleshooting

**Service won't start:** Check logs with `docker-compose logs [service]`
**Health check fails:** Verify service responding on internal port 80
**Image pull fails:** Check registry access and network
**Port conflicts:** Ensure port 80 available
**Memory issues:** Monitor with `docker stats`

## Emergency Procedures

**Restart services:**
```bash
docker-compose restart
```

**Complete reset:**
```bash
docker-compose down
docker system prune -f
docker-compose up -d
```

**Rollback:**
```bash
IMAGE_TAG=last-known-good docker-compose up -d
```