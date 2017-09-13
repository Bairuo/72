using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysWorld : MonoBehaviour
{
    public int stepCount = 4;
    
    /// A reference pool.
    public static List<Body> bodies = new List<Body>();
    
    void StepPhysicsWorld(float timestep)
    {
        float t = timestep / stepCount;
        for(int h=0; h<stepCount; h++)
        {
            for(int i=0; i<bodies.Count; i++)
            {
                var x = bodies[i];
                
                for(int j=i+1; j<bodies.Count; j++)
                {
                    var y = bodies[j];
                
                    if(x.farDist + y.farDist >=
                        Vector2.Distance(x.gameObject.transform.position, y.gameObject.transform.position))
                        {
                            y.CollisionCall(x, t);
                        }
                }
            }
            
            foreach(var i in bodies)
            {
                i.StepMovement(t);
            }
        }
    }
    
    void FixedUpdate()
    {
        StepPhysicsWorld(Time.fixedDeltaTime);
    }
}