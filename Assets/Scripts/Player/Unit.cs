using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float maxHealth;
    public float health;
    
    Body body;
    
    public float baseMass { get{ return body.baseMass; } }
    public Modifier massModifier{ get{ return body.massModifier; } }
    public float mass { get{ return body.mass; } }
    
    public float baseHardness { get{ return body.hardness; } }
    public Modifier hardnessModifier{ get{ return body.hardnessModifier; } }
    public float hardness { get{ return body.hardness; } }
    
    public float baseMOI { get{ return body.baseMOI; } }
    public Modifier MOIModifier { get{ return body.MOIModifier; } }
    public float MOI { get{ return body.MOI; } }
    
    public float baseDrag { get{ return body.baseDrag; } }
    public Modifier dragModifier { get{ return body.MOIModifier; } }
    public float drag { get{ return body.drag; } }
    
    public float baseAngularDrag{ get{ return body.baseAngularDrag; } }
    public Modifier angularDragModifier { get{ return body.angularDragModifier; } }
    public float angularDrag{ get{ return body.angularDrag; } }
    
    public float baseSideDrag{ get{ return body.sideDrag; } }
    public Modifier sideDragModifier { get{ return body.sideDragModifier; } }
    public float sideDrag { get{ return body.sideDrag; } }
    
    void Start()
    {
        if(body == null) body = this.gameObject.GetComponent<Body>();
        if(body == null) 
        {
            Debug.Log("WARNING: A player script should have a BOdy component.");
            Destroy(this);
            return;
        }
        
        body.collisionCallbacks += OnCollision;
    }
    
    void OnCollision(Body other, Vector2 impulse)
    {
        // do nothing here...
    }
    
}
