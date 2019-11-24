using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace MathematicsExtensions
{
    public struct PolarCoordinates
    {
        public float Radius;
        public float Theta;
    }

    public struct SphericalCoordinates
    {
        public float Radius;
        public float Theta;
        public float Phi;
    }

    public static class MathEx
    {
        static readonly float3 Up = new float3(0f, 1f, 0f);
        static readonly float3 Right = new float3(1f, 0f, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates PosToPolarCoordinates(in float2 pos)
        {
            var rad = math.sqrt(pos.x * pos.x + pos.y * pos.y);
            return new PolarCoordinates
            {
                Radius = rad,
                Theta = math.atan2(pos.y, pos.x),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 PolarCoordinatesToPos(in float radius, in float theta)
            => PolarCoordinatesToPos(new PolarCoordinates() {Radius = radius, Theta = theta});

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 PolarCoordinatesToPos(in PolarCoordinates coord)
        {
            return new float2
            {
                x = coord.Radius * math.sin(coord.Theta),
                y = coord.Radius * math.cos(coord.Theta),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SphericalCoordinates PosToSphericalCoord(in float3 pos)
        {
            var rad = math.sqrt(pos.x * pos.x + pos.y * pos.y + pos.z * pos.z);
            return new SphericalCoordinates
            {
                Radius = rad,
                Theta = math.acos(pos.z / rad),
                Phi = math.atan2(pos.y, pos.x),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 SphericalCoordToPos(in float radius, in float theta, in float phi)
            => SphericalCoordToPos(new SphericalCoordinates() {Radius = radius, Theta = theta, Phi = phi});

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 SphericalCoordToPos(in SphericalCoordinates coord)
        {
            return new float3
            {
                x = coord.Radius * math.sin(coord.Theta) * math.cos(coord.Phi),
                y = coord.Radius * math.sin(coord.Theta) * math.sin(coord.Phi),
                z = coord.Radius * math.cos(coord.Theta),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 AddRandomPosition(in float3 pos, ref Random randomRef, in float range)
        {
            var x = pos.x + randomRef.NextFloat(-range, range);
            randomRef.state += 1;
            var y = pos.y + randomRef.NextFloat(-range, range);
            randomRef.state += 1;
            var z = pos.z + randomRef.NextFloat(-range, range);
            randomRef.state += 1;
            return new float3(x, y, z);
        }
        
        /// <summary>
        /// 円錐上でのベクトルの算出
        /// </summary>
        /// <param name="centerAxis">円錐の中心軸</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">角度(radius)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 CalcConeVector(in float3 centerAxis, in float radius, in float angle)
        {
            var bottom = Right * radius;
            bottom = math.mul(quaternion.AxisAngle(Up, angle), bottom);
            return math.normalize(bottom + centerAxis);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
        }
    }
}
