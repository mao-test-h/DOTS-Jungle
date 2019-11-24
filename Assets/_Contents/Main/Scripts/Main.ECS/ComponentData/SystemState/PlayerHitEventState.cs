using Unity.Entities;
using Unity.Mathematics;
using Main.Enums;

namespace Main.ECS
{
    // プレイヤーヒットイベント
    public struct PlayerHitEventState : ISystemStateComponentData
    {
        public float3 HitPosition;
        public Bullet BulletType;
    }
}
