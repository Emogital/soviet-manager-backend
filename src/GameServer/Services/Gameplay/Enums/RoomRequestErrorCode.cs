namespace GameServer.Services.Gameplay
{
    public enum RoomRequestErrorCode
    {
        None = 0,
        RoomNameMissing = 1,
        PlayerNameMissing = 2,
        RoomNotFound = 3,
        RoomNotAwaiting = 4,
        PlayerNameAlreadyTaken = 5,
        RoomIsFull = 6,
        PlayerRegistrationFailed = 7,
        GameModeNotAllowed = 8,
        InvalidRoomCapacity = 9,
        RoomAlreadyExists = 10,
        RoomCreationOrPlayerRegistrationFailed = 11,
    }
}