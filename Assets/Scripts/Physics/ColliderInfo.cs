using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// This is as a simple sample on how to use the callback of Physworld.
/// This module should not be directly used in game logic.
/// It's better to implement the same logic in somewhere about PlayerController and so on.
public class ColliderInfo : MonoBehaviour
{
    /// Count seconds from the last collision of this object.
    /// Special rules of collision taken into account is described in function TriggerCollision below.
    /// This has a special use in the specific game design.
    /// This design which should be not taken into a part of extension.
    public float timeAfterLastCollision = 0f;
    
    Body body;
    
    void Start()
    {
        body = this.gameObject.GetComponent<Body>();
        if(body == null)
        {
            Debug.Log("WARNING: collider info component must be attached to a Body component.");
            Destroy(this);
        }
        
        if(PhysWorld.collisionCallback.ContainsKey(body))
        {
            Debug.Log("WARNING: override callback for collider info. There must be at most one callback function for each Body.");
            PhysWorld.collisionCallback[body] = CollisionCallback;
        }
        else
        {
            PhysWorld.collisionCallback.Add(body, CollisionCallback);
        }
    }
    
    void OnDestroy()
    {
        if(PhysWorld.collisionCallback.ContainsKey(body))
            PhysWorld.collisionCallback.Remove(body);
    }
    
    void FixedUpdate()
    {
        timeAfterLastCollision += Time.fixedDeltaTime;
    }
    
    
    void CollisionCallback(Body op, CollisionInfo info)
    {
        timeAfterLastCollision = 0f;
    }
    
    
}   