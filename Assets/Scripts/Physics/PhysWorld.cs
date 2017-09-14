using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysWorld : MonoBehaviour
{
    public int standardStepCount;
    
    // Increase step count when velocity is higher.
    public float testTimesPerSpeed;
    
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
    
    void StepPhysicsWorld(float timestep)
    {
        ForeachBody((Body x, Body y) =>
        {
            if(x.farDist + y.farDist + (x.velocity - y.velocity).magnitude * timestep >=
                Vector2.Distance(x.gameObject.transform.position, y.gameObject.transform.position))
            {
                int times = Mathf.FloorToInt((x.velocity - y.velocity).magnitude / testTimesPerSpeed) + standardStepCount;
                for(int h=0; h<times; h++)
                    y.CollisionCall(x, timestep / times);
            }
            
            // Emmmmmmm this looks better? But not so fast.
            // if(x.farDist + y.farDist >= Vector2.Distance(x.gameObject.transform.position, y.gameObject.transform.position))
            // {
            //     for(int i=0; i<standardStepCount; i++)
            //         y.CollisionCall(x, timestep / standardStepCount);
            // }
            
        });
        
        
        foreach(var i in bodies)
        {
            i.StepMovement(timestep);
        }
    }
    
    void FixedUpdate()
    {
        StepPhysicsWorld(Time.fixedDeltaTime);
    }
}