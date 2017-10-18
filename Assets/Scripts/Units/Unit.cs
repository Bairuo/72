// Created by DK 2017/10/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float baseDefence;
    public float defenceRatio; // per 1.0 baseDefence, the formula is shown in property *health*.
    public readonly Modifier defenceModifier = new Modifier();
    public float defence{ get{ return defenceModifier.GetValue(baseDefence); } }
    public float damageRatio{ get{ return 1f - Mathf.Exp(-1f / defenceRatio / defence); } }
    
    public float maxHealth;
    public float currentHealth;
    public float health
    {
        get{ return currentHealth; }
        set
        {
            float delta = value - currentHealth;
            if(delta >= 0f)
            {
                currentHealth += delta;
                if(currentHealth > maxHealth) currentHealth = maxHealth;
            }
            else
            {
                currentHealth += delta * damageRatio;
                if(currentHealth < 0f) currentHealth = 0f;
            }
        }
    }
    
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
    
    protected virtual void Start()
    {
        if(body == null) body = this.gameObject.GetComponent<Body>();
        if(body == null) 
        {
            Debug.Log("WARNING: A player script should have a BOdy component.");
            Destroy(this);
            return;
        }
        
        body.collisionCallbacks += OnCollision;
        
        Client.instance.posmanager.PlayerRegister(this.gameObject);
    }
    
    protected virtual void OnDestroy()
    {
        // [!] TODO.
    }
    
    void OnCollision(Body other, Vector2 impulse)
    {
        // do nothing here...
    }
    
}
