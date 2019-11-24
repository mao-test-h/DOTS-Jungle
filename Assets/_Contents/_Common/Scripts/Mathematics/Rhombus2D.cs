// refered to:
//     https://takachan.hatenablog.com/entry/2018/07/24/225933

using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace MathematicsExtensions
{
    public struct Rhombus2D
    {
        public float2 P1;
        public float2 P2;
        public float2 P3;
        public float2 P4;

        /// <summary>
        /// 平面上の菱形の定義
        /// </summary>
        /// <param name="p1">p2の始点, p4の終点</param>
        /// <param name="p2">p3の始点, p1の終点</param>
        /// <param name="p3">p4の始点, p2の終点</param>
        /// <param name="p4">p1の始点, p3の終点</param>
        /// <remarks>引数は(p1→p2, p2→p3..)と菱形を整形する形で結ばないと正しい結果が返らない</remarks>
        public Rhombus2D(float2 p1, float2 p2, float2 p3, float2 p4)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }

        public bool Contains(float x, float y) => Contains(new float2(x, y));

        public bool Contains(float2 point)
        {
            var a = Calc(P1, P2, point);
            var b = Calc(P2, P3, point);
            var c = Calc(P3, P4, point);
            var d = Calc(P4, P1, point);
            return a > 0 && b > 0 && c > 0 && d > 0;
        }

        // 指定した2点と1点の外積の計算
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float Calc(in float2 p1, in float2 p2, in float2 p)
        {
            var vec12 = p1 - p2; // p1とp2のベクトル
            var vec1p = p1 - p; // p1と判定位置(p)のベクトル
            return vec12.x * vec1p.y - vec1p.x * vec12.y; // 外積
        }
    }
}
