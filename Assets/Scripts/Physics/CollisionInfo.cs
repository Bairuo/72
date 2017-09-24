using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollisionInfo
{
    public Vector2 impulse;
    
    bool overlap;
    public bool overlapping { get { return overlap; } }
    public bool colliding { get { return !overlap; } }
    
    public CollisionInfo(Vector2 imp, bool overl)
    {
        impulse = imp;
        overlap =  overl;
    }
}