using MessagePack;
using System.Numerics;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public readonly struct DieThrowData
    {
        [Key(0)]
        public readonly Vector3 Position;

        [Key(1)]
        public readonly Vector4 Rotation;

        [Key(2)]
        public readonly int Result;

        public DieThrowData(Vector3 position, Vector4 rotation, int result)
        {
            Position = position;
            Rotation = rotation;
            Result = result;
        }
    }
}
