using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysWorld : MonoBehaviour
{
    /// Make the simulation step on anyway.
    public bool forceApply;
    
    public int stepCount;
    
    // [!]Depreated
    // Increase step count when velocity is higher.
    // public float testTimesPerSpeed;
    
    /// A reference pool.
    public static List<Body> bodies = new List<Body>();
    
    delegate void FuncDealCollision(Body x, Body y);
    
    void ForeachBody(FuncDealCollision f)
    {
        
        for(int i=0; i<bodies.Count; i++)
        {
            for(int j=i+1; j<bodies.Count; j++)
            {
                f(bodies[i], bodies[j]);
            }
        }
    }
    
    void FixedUpdate()
    {
        if(forceApply || Client.IsRoomServer())
        {
            for(int i=0; i<stepCount; i++)
                StepPhysicsWorld(Time.fixedDeltaTime / stepCount);
        }
    }
    
    void StepPhysicsWorld(float timestep)
    {
        ForeachBody((Body x, Body y) =>
        {
            if(x.freezed && y.freezed) return;
            
            // Fast exclusive.
            float rads = x.farDist + y.farDist;
            Vector2 relv = x.velocity - y.velocity;
            Segment l = new Segment(x.transform.position, (Vector2)x.transform.position + relv);
            Segment left = new Segment(l.from + Calc.Rot90(l.dir).normalized, l.to + Calc.Rot90(l.dir).normalized);
            Segment right = new Segment(l.from - Calc.Rot90(l.dir).normalized, l.to - Calc.Rot90(l.dir).normalized);
            Vector2 tar = y.transform.position;
            if(
                Vector2.Distance(l.from, tar) <= rads || 
                Vector2.Distance(l.to, tar) <= rads || 
                (   (left.Overlap(left.Projection(tar)) && left.ProjectionHeight(tar) <= y.farDist) ||
                    Vector2.Distance(left.from, tar) <= rads ||
                    Vector2.Distance(left.to, tar) <= rads) ||
                (   (right.Overlap(right.Projection(tar)) && right.ProjectionHeight(tar) <= y.farDist) ||
                    Vector2.Distance(right.from, tar) <= rads ||
                    Vector2.Distance(right.to, tar) <= rads))
            {
                CollisionCall(x, y, timestep);
            }
        });
        
        foreach(var i in bodies)
        {
            i.StepMovement(timestep);
        }
    }
    
    
    // intersection points location in world space.
    // not declared to be temporary variable for avoiding GC.
    static Vector2[] isc = new Vector2[15];
    static int colCount;
    
    /// A "collision call" is to deal with the interactivity of two colliding objects.
    /// Will change their velocity and angular velocity.
    public static bool CollisionCall(Body bodyA, Body bodyB, float timestep)
    {
        colCount = Calc.GetIntersectionPoints(bodyA.points, bodyA.gameObject.transform, bodyB.points, bodyB.gameObject.transform, ref isc);
        
        if(colCount == 0) return false;
        
        // ============ Collision React ==============
        // Consider forces applied to itself.
        // Not affect the other collider.
        
        // Coefficient of Restitution.
        // defined as - deltaV before / deltaV after.
        // determined by material properties (like hardness).
        float e = bodyB.hardness * bodyB.hardness;
        if(e < 0f) e = 0f;
        // intersection line.
        Segment L = new Segment(isc[0], isc[1]);
        // collision point. Center of intersection line.
        Vector2 mid = L.Interpolate(0.5f);
        // relative position of collision point to mass center.
        Vector2 rc = mid - (Vector2)bodyA.gameObject.transform.position;
        Vector2 rp = mid - (Vector2)bodyB.gameObject.transform.position;
        // relative velocity.
        Vector2 rv = bodyA.velocity - bodyB.velocity + Calc.Rot90(rc) * bodyA.angularVelocity - Calc.Rot90(rp) * bodyB.angularVelocity;
        // normal of collision. Whatever direction is.
        Vector2 f = Calc.Rot90(L.dir.normalized);
        if(Calc.DotMultiply(f, rc) < 0f) f = -f;
        // Impulse.
        Vector2 I = f;
        I *= - (1f + e) * Calc.DotMultiply(rv, f) / (
            (1f / bodyA.mass + 1f / bodyB.mass) +
            Calc.DotMultiply(f, Vector3.Cross(Vector3.Cross((Vector3)rc, f) / bodyA.MOI, rc)) +
            Calc.DotMultiply(f, Vector3.Cross((Vector3)(Vector3.Cross((Vector3)rp, f) / bodyB.MOI), rp)));
        
        if(I.magnitude < 100f || Calc.DotMultiply(rc, I) > 0f) // seperate two objects for no reason.
        {
            Vector2 dip = bodyB.gameObject.transform.position - bodyA.gameObject.transform.position;
            bodyA.velocity -= dip.normalized * Mathf.Pow(0.2f, timestep);
            bodyA.angularVelocity *= Mathf.Pow(0.1f, timestep);
            bodyB.velocity += dip.normalized * Mathf.Pow(0.2f, timestep);
            bodyB.angularVelocity *= Mathf.Pow(0.1f, timestep);
        }
        else
        {
            bodyA.velocity += I / bodyA.mass;
            bodyA.angularVelocity += Calc.CrossMultiply(rc, I) / bodyA.MOI;
            bodyA.angularVelocity = Calc.RangeCut(bodyA.angularVelocity, -2.0f * Mathf.PI, 2.0f * Mathf.PI);
            
            bodyB.velocity += -I / bodyB.mass;
            bodyB.angularVelocity += Calc.CrossMultiply(rp, -I) / bodyB.MOI;
            bodyB.angularVelocity = Calc.RangeCut((float)bodyB.angularVelocity, -2.0f * Mathf.PI, 2.0f * Mathf.PI);
        }
        
        return true;
    }
    
}