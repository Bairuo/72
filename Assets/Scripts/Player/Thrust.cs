// Created by DK 2017/9/27

using UnityEngine;
using System.Collections.Generic;

/// Provides a sustained thrust.
public class Thrust : MonoBehaviour
{
    public Body body;
    
    public float baseThrust;
    public Modifier thrustModifier = new Modifier();
    public float thrust { get{ return thrustModifier.GetValue(baseThrust); } }
    
    ForceToken token;
    
    void Start()
    {
        if(body == null) body = this.gameObject.GetComponent<Body>();
        if(body == null)
        {
            Debug.Log("WARNING: cannot find body component required by Thrust.");
            Destroy(this);
            return;
        }
        
        token = body.AddForce(Vector2.up * thrust, Vector2.zero, 1e8f);
    }
    
    void OnDestroy()
    {
        body.RemoveForce(token);
    }
    
    void FixedUpdate()
    {
        token.forceToken.Value.value = Vector2.up * thrust;
    }
}