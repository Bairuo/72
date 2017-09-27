
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.CompilerServices;

/// 计算几何. 按需选用.


public class Calc
{

    public static float eps = 1e-7f; // 线性误差范围; long double : 1e-16;
    public static float eps2 = 1e-4f; // 平方级误差范围; long double: 1e-8;
    public static bool eq(float a, float b) { return Mathf.Abs(a-b) < eps; }

    // ============================ 点和向量 =============================
    
    public static float CrossMultiply(Vector2 a, Vector2 b) { return a.x * b.y - a.y * b.x; }
    public static float DotMultiply(Vector2 a, Vector2 b) { return Vector2.Dot(a, b); }

    public static bool OnLeftSide(Vector2 a, Vector2 b) { return CrossMultiply(a, b) > eps; }
    public static bool OnRightSide(Vector2 a, Vector2 b) { return CrossMultiply(a, b) < -eps; }
    public static bool Parallel(Vector2 a, Vector2 b) { return eq(CrossMultiply(a, b), 1); }
    
    /// Z轴转角. 假设四元数直接代表了z轴的旋转(z = sin(theta/2))
    public static float RotationAngleZ(Quaternion q) { return Mathf.Acos(q.w) * Mathf.Sign(q.z) * 2.0f; }
    
    /// 对一个二维向量应用Z轴旋转.
    public static Vector2 ApplyRotationAngle(float angle, Vector2 v)
    {
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }
    
    /// 对一个二维向量进行旋转, cos和sin由参数给出.
    public static Vector2 ApplyRotationAngle(float cos, float sin, Vector2 v)
    {
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }
    
    
    public static Quaternion quat90 = Quaternion.Euler(0f, 0f, 90f);
    public static Vector2 Rot90(Vector2 a) { return new Vector2(-a.y, a.x); }
    
    public static Vector2 FromTo(Vector2 a, Vector2 b) { return b - a; }
    
    public static float RangeCut(float a, float bottom, float top) { return a > top ? top : a < bottom ? bottom : a; }
    public static float RelativeCut(float curv, float topv) { return Mathf.Min(1.0f, Mathf.Max(0f, (curv - topv) / topv)); }
    
    public static bool Parallel(Segment a, Segment b)
    {
        return Parallel(a.dir, b.dir);
    }
    
    // 左正右负, 弧度.
    public static float Angle(Vector2 from, Vector2 to)
    {
        return Mathf.Acos(DotMultiply(from, to) / from.magnitude / to.magnitude) * Mathf.Sign(CrossMultiply(from, to));
    }
    
    // 左正右负, 弧度.
    public static float Angle(float from, float to)
    {
        float delta = to - from;
        if(-Mathf.PI < delta && delta <= Mathf.PI) return delta;
        else if(delta <= -Mathf.PI) return 2f * Mathf.PI + delta;
        else return delta - 2f * Mathf.PI;
    }
    
    public static Vector2 Reflection(Vector2 dir, Vector2 normal)
    {
        Vector2 h = DotMultiply(dir, normal) / normal.magnitude * normal.normalized;
        Vector2 d = h - dir;
        return dir + 2.0f * d;
    }
    
    // 相交. 考虑端点. 不计线段端点则删掉 eq(..., 0) 的所有判断.
    public static bool Intersect(Segment A, Segment B)
    {
        Vector2 dia = FromTo(A.from, A.to);
        Vector2 dib = FromTo(B.from, B.to);
        float a = CrossMultiply(dia, FromTo(A.from, B.from));
        float b = CrossMultiply(dia, FromTo(A.from, B.to));
        float c = CrossMultiply(dib, FromTo(B.from, A.from));
        float d = CrossMultiply(dib, FromTo(B.from, A.to));
        bool la = eq(a, 0f) || eq(b, 0f) || ((a < 0f) != (b < 0f));
        if(!la) return false;
        bool lb = eq(c, 0f) || eq(d, 0f) || ((c < 0f) != (d < 0f));
        return lb;
    }
    
    public static Vector2 Projection(Vector2 x, Vector2 dir)
    {
        return dir.normalized * DotMultiply(x, dir.normalized);
    }
    
    public static Vector2 IntersectionPoint(Segment A, Segment B)
    {
        if(Parallel(A.dir, B.dir))
        {
            if(A.Overlap(B.from)) return B.from;
            else if(A.Overlap(B.to)) return B.to;
            else if(B.Overlap(A.from)) return A.from;
            else return A.to;
        }
        Vector2 d1 = A.dir;
        Vector2 d2 = B.dir;
        Vector2 g = new Vector2(B.from.x - A.from.x, B.from.y - A.from.y);
        float K = CrossMultiply(d1, d2);
        return CrossMultiply(g, d2) / K * A.dir + A.from;
    }
    
    public static int GetIntersectionPoints(Vector2[] p1, Transform trs1, Vector2[] p2, Transform trs2, ref Vector2[] intersections)
    {
        Vector2 ct1 = trs1.position;
        Vector2 ct2 = trs2.position;
        float rt1 = RotationAngleZ(trs1.rotation);
        float rt2 = RotationAngleZ(trs2.rotation);
        int intc = 0;
        var cc1 = p1.Length;
        var cc2 = p2.Length;
        
        float cos1 = Mathf.Cos(rt1);
        float sin1 = Mathf.Sin(rt1);
        float cos2 = Mathf.Cos(rt2);
        float sin2 = Mathf.Sin(rt2);
        
        for(int i=0; i<cc1; i++)
        {
            Vector2 g1 = ApplyRotationAngle(cos1, sin1, p1[i]) + ct1;
            Vector2 g2 = ApplyRotationAngle(cos1, sin1, p1[i+1 == cc1 ? 0 : i+1]) + ct1;
            Segment seg1 = new Segment(g1, g2);
            float l1 = Mathf.Min(g1.x, g2.x);
            float r1 = Mathf.Max(g1.x, g2.x);
            float b1 = Mathf.Min(g1.y, g2.y);
            float t1 = Mathf.Max(g1.y, g2.y);
            
            for(int j=0; j<cc2; j++)
            {
                // AABB 快速排除.
                Vector2 k1 = ApplyRotationAngle(cos2, sin2, p2[j]) + ct2;
                Vector2 k2 = ApplyRotationAngle(cos2, sin2, p2[j+1 == cc2 ? 0 : j+1]) + ct2;
                Segment seg2 = new Segment(k1, k2);
                if(r1 < Mathf.Min(k1.x, k2.x) ||
                    Mathf.Max(k1.x, k2.x) < l1 ||
                    t1 < Mathf.Min(k1.y, k2.y) ||
                    Mathf.Max(k1.y, k2.y) < b1)
                    continue;
                
                // 相交判定.
                if(Intersect(seg1, seg2))
                {
                    intersections[intc++] = IntersectionPoint(seg1, seg2);
                }
            }
        }
        return intc;
    }
}

// ============================== 线段 ===============================
public struct Segment
{
    public Vector2 from;
    public Vector2 to;
    
    public Segment(Vector2 x, Vector2 y) { from = x; to = y; }
    
    public Vector2 dir { get { return to - from; } } // 方向向量,未标准化.
    public float len { get{ return (to - from).magnitude; } } // 长度.
    public float slope { get{ return (from.y - to.y) / (from.x - to.x); } }
    public float antislope { get{ return (from.x - to.x) / (from.y - to.y); } }
    public bool hasSlope { get{ return Calc.eq(from.x, to.x); } }
    public bool hasAntislope{ get{ return Calc.eq(from.y, to.y); } }
    
    public Vector2 Interpolate(float x) { return x * dir + from; }
    
    // 点在线段上.
    public bool Overlap(Vector2 v)
    { return Calc.eq(Calc.FromTo(from, to).magnitude, Calc.FromTo(v, from).magnitude + Calc.FromTo(v, to).magnitude); }
    
    
    public Vector2 Projection(Vector2 p) // 点到直线上的投影.
    {
        float h = Mathf.Abs(Calc.CrossMultiply(dir, Calc.FromTo(from, p))) / len;
        float r = Mathf.Sqrt(Calc.FromTo(from, p).sqrMagnitude - h*h);
        if(Calc.eq(r, 0)) return from;
        if(Calc.DotMultiply(dir, Calc.FromTo(from, p)) < 0) return dir.normalized * (-r);
        else return dir.normalized * r;
    }
    
    public Vector2 Nearest(Vector2 p) // 点到线段的最近点.
    {
        Vector2 g = Projection(p);
        if(Overlap(g)) return g;
        if(Calc.FromTo(g, from).magnitude < Calc.FromTo(g, to).magnitude) return from;
        return to;
    }
    
    public float ProjectionHeight(Vector2 p) // 点到直线的距离.
    {
        return Mathf.Abs(Calc.CrossMultiply(Calc.FromTo(p, from), Calc.FromTo(p, to)) / len / 2f);
    }
};

