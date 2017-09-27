// Created by DK 2017/9/27

using UnityEngine;
using System.Collections;

/// Provides a sustained thrust.
public class Thrust : MonoBehaviour
{
    public Body body;
    public float thrust;
    
    void Start()
    {
        if(body == null) body = this.gameObject.GetComponent<Body>();
        if(body == null)
        {
            Debug.Log("WARNING: cannot find body component required by Thrust.");
            Destroy(this);
            return;
        }
        
        body.AddForce(Vector2.up * thrust, Vector2.zero, 1e8f); // !!!
    }
}