namespace GameServer.Services.Gameplay.Rooms
{
    public sealed class RoomRequestResult
    {
        public RoomRequestErrorCode ErrorCode { get; }

        public bool IsSuccess => ErrorCode == RoomRequestErrorCode.None;

        private RoomRequestResult(RoomRequestErrorCode code)
        {
            ErrorCode = code;
        }

        public static RoomRequestResult Success()
        {
            return new RoomRequestResult(RoomRequestErrorCode.None);
        }

        public static RoomRequestResult Fail(RoomRequestErrorCode code)
        {
            return new RoomRequestResult(code);
        }
    }
}



