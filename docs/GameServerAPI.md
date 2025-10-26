# GameServer API (Connection)

## Overview

Player-facing endpoint for creating or joining rooms.

Note: Gameplay endpoints are under active development; endpoints and payloads may change frequently.

### Authentication

Requires user authentication (Bearer JWT).

## POST /api/connection/joinroom

Creates a room (when `gameMode != None`) or joins an existing one (when `gameMode == None`).

### Request

Body matches `RoomRequestDto`:

```json
{
  "lobbySettings": {
    "roomName": "Room1",
    "playerName": "Alice"
  },
  "gameMode": 5,
  "roomCapacity": 2
}
```

### Responses

- 200 OK: Room created or joined
- 204 NoContent: Missing or empty request body
- 400 BadRequest: Validation/domain error. Uses RFC 7807 ProblemDetails with an integer `errorCode` extension (from `RoomRequestErrorCode`).

Example 400 response:

```json
{
  "title": "Room request failed",
  "detail": "RoomNotFound",
  "status": 400,
  "errorCode": 3
}
```

Clients should branch logic on `errorCode` and treat `title`/`detail` as human-readable.


