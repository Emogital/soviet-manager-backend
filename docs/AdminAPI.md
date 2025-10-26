# Admin API Documentation

## Overview

The Admin API provides endpoints for monitoring server status and managing the Soviet Manager game server. This API is designed for operational use, allowing administrators to check active rooms and players before performing maintenance operations.

## Security

The Admin API is secured using API key authentication. All requests must include a valid API key in the `X-Admin-Key` header.

### API Key Configuration

- **Environment Variable**: `ADMIN_API_KEY`
- **Header Name**: `X-Admin-Key`
- **Required**: Yes for all admin endpoints

### Security Best Practices

1. **Never commit API keys to version control**
2. **Use strong, randomly generated keys** (minimum 32 characters)
3. **Rotate keys periodically** (recommended: every 90 days)
4. **Use environment variables** for production deployments
5. **Consider IP whitelisting** at network level for additional security
6. **Monitor access logs** for suspicious activity

## Endpoints

### GET /api/admin/server-status

Returns comprehensive server status including active rooms and connected players.

#### Request

```http
GET /api/admin/server-status
X-Admin-Key: your-api-key-here
```

**Note:** The API is accessible through nginx at `http://localhost/api/admin/server-status` in local development, or `https://your-domain.com/api/admin/server-status` in production.

#### Response

```json
{
  "timestamp": "2025-01-19T12:34:56.789Z",
  "activeRoomsCount": 3,
  "totalPlayersCount": 8,
  "connectedPlayersCount": 6,
  "rooms": [
    {
      "name": "Room1",
      "gameMode": 5,
      "status": 2,
      "playerCount": 3,
      "connectedPlayerCount": 3,
      "players": [
        {
          "name": "Player1",
          "status": 2,
          "id": 0,
          "teamId": 1
        }
      ]
    }
  ]
}
```

#### Response Fields

**ServerStatusDto:**
- `timestamp`: UTC timestamp when status was retrieved
- `activeRoomsCount`: Total number of active rooms
- `totalPlayersCount`: Total number of players across all rooms
- `connectedPlayersCount`: Number of currently connected players
- `rooms`: Array of room status objects

**RoomStatusDto:**
- `name`: Room name
- `gameMode`: Game mode enum value (see GameMode enum)
- `status`: Room status enum value (see RoomStatus enum)
- `playerCount`: Total players in room
- `connectedPlayerCount`: Connected players in room
- `players`: Array of player status objects

**PlayerStatusDto:**
- `name`: Player display name
- `status`: Player status enum value (see PlayerStatus enum)
- `id`: Player ID within the room
- `teamId`: Team assignment

#### Status Enums

**RoomStatus:**
- `0`: None
- `1`: Awaiting
- `2`: Playing
- `3`: Removing

**PlayerStatus:**
- `0`: None
- `1`: Disconnected
- `2`: Connected
- `3`: Removed
- `4`: AutoControlled

**GameMode:**
- `5`: ClassicMatch
- `6`: CoopForTwoMatch
- `7`: TwoByTwoMatch
- `13`: CoopHardcoreMatch
- `14`: ConfrontationMatch
- `15`: RatingMatch

#### Error Responses

**401 Unauthorized:**
```json
{
  "error": "Unauthorized: Missing API key"
}
```

**401 Unauthorized:**
```json
{
  "error": "Unauthorized: Invalid API key"
}
```

**500 Internal Server Error:**
```json
{
  "error": "Internal server error"
}
```

## Usage Examples

### cURL

```bash
# Check server status
curl -H "X-Admin-Key: your-api-key-here" \
     http://localhost/api/admin/server-status

# Check with verbose output
curl -v -H "X-Admin-Key: your-api-key-here" \
     http://localhost/api/admin/server-status
```

### PowerShell

```powershell
$headers = @{
    "X-Admin-Key" = "dev-admin-key-12345"
}
$response = Invoke-RestMethod -Uri "http://localhost/api/admin/server-status" -Headers $headers
$response | ConvertTo-Json -Depth 10
```

### Python

```python
import requests

headers = {
    "X-Admin-Key": "your-api-key-here"
}

response = requests.get("http://localhost/api/admin/server-status", headers=headers)
print(response.json())
```

## Deployment

### Environment Variables

The `ADMIN_API_KEY` environment variable must be set in your environment. It's not included in configuration files for security reasons.

**Local Development:**
Create a `.env` file in `deploy/local/` directory:
```bash
# Copy from .env.example and set your values
ADMIN_API_KEY=dev-admin-key-12345
```

**Production:**
Set the environment variable in your deployment environment:
```bash
export ADMIN_API_KEY="your-secure-random-key-here"
```

### Docker

The environment variable is already configured in both local and remote docker-compose files:

**Local (deploy/local/docker-compose.yml):**
```yaml
services:
  gameserver:
    environment:
      - ADMIN_API_KEY=${ADMIN_API_KEY}
```

**Remote (deploy/remote/docker-compose.yml):**
```yaml
services:
  game:
    environment:
      - ADMIN_API_KEY
```

### GitHub Secrets (Production)

For production deployments using GitHub Actions, set the `ADMIN_API_KEY` as a GitHub secret:

1. Go to your repository Settings → Secrets and variables → Actions
2. Add a new repository secret named `ADMIN_API_KEY`
3. Set the value to your secure random key
4. The GitHub Action will automatically inject this into the deployment environment

## Monitoring and Alerting

### Log Monitoring

The API logs all access attempts. Monitor for:
- Failed authentication attempts
- Unusual access patterns
- High request volumes

### Health Checks

Use this endpoint in your monitoring systems to:
- Verify server is responsive
- Check if maintenance can be performed safely
- Monitor active player counts
- Detect server issues

### Example Health Check Script

```bash
#!/bin/bash
API_KEY="your-api-key-here"
SERVER_URL="https://your-server.com"

response=$(curl -s -H "X-Admin-Key: $API_KEY" "$SERVER_URL/api/admin/server-status")
connected_players=$(echo $response | jq '.connectedPlayersCount')

if [ "$connected_players" -eq 0 ]; then
    echo "No players connected - safe to restart"
    # Add your restart logic here
else
    echo "Warning: $connected_players players still connected"
fi
```

## Rate Limiting

Currently, no rate limiting is implemented. Consider adding rate limiting for production use:

- **Recommended**: 60 requests per minute per IP
- **Burst allowance**: 10 requests per minute
- **Implementation**: Use ASP.NET Core rate limiting middleware

## Troubleshooting

### Common Issues

1. **401 Unauthorized**: Check API key configuration and header name
2. **500 Internal Server Error**: Check server logs for detailed error information
3. **Empty response**: Verify server is running and rooms exist

### Debug Mode

Enable debug logging to troubleshoot issues:

```json
{
  "Logging": {
    "LogLevel": {
      "GameServer.Middleware.ApiKeyAuthenticationMiddleware": "Debug"
    }
  }
}
```

## Future Enhancements

Potential future additions to the Admin API:

- Player management (kick, ban)
- Room management (create, destroy)
- Server metrics (performance, memory usage)
- Configuration management
- Log retrieval
- Health check endpoints
