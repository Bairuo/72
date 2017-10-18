using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysWorld : MonoBehaviour
{
    public bool applyPhysStep;
    public bool applyCollision;
    
    public int stepCount;
    
    // [!]Depreated
    // Increase step count when velocity is higher.
    // public float testTimesPerSpeed;
    
    /// A reference pool.
    public static List<Body> bodies = new List<Body>();
    
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
        if(applyPhysStep)
        {
            for(int i=0; i<stepCount; i++)
                StepPhysicsWorld(Time.fixedDeltaTime / stepCount);
        }
    }
    
    delegate void FuncDealCollision(Body x, Body y);
    void StepPhysicsWorld(float timestep)
    {
        ForeachBody((Body x, Body y) =>
        {
            if(x.freezedAll && y.freezedAll) return;
            // Fast exclusive.
            if(x.farDist + y.farDist >= Vector2.Distance(x.gameObject.transform.position, y.gameObject.transform.position))
            {
                if(applyCollision) CollisionCall(x, y, timestep);
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
        
        // ====================== Collision React ========================
        // Consider forces applied to itself.
        // Not affect the other collider.
        
        // Coefficient of Restitution.
        // defined as - deltaV before / deltaV after.
        // determined by material properties (like hardness).
        float e = 1 - (1f - bodyA.hardness) * (1f - bodyB.hardness);
        
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
        
        if(Calc.DotMultiply(rc, I) > 0f) // seperate two objects for no reason.
        {
            I = -I * 0.001f; // Not so scientific.
        }
        
        bodyA.velocity += I / bodyA.mass;
        bodyA.angularVelocity += Calc.CrossMultiply(rc, I) / bodyA.MOI;
        
        bodyB.velocity += -I / bodyB.mass;
        bodyB.angularVelocity += Calc.CrossMultiply(rp, -I) / bodyB.MOI;
        
        // ======================= Collision Callback =========================
        // Callbacks are called AFTER the simulation of collision.
        // To disable the collision and take an other logic, reverse applying the logic before.
        bodyA.collisionCallbacks(bodyB, I);
        bodyB.collisionCallbacks(bodyA, -I);
        
        return true;
    }
    
}