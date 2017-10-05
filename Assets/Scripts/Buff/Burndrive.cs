using System;
using UnityEngine;
using System.Collections.Generic;


public class Burndrive : Buff
{
 
    LinkedListNode<SubModifier> massToken;   
    public float massAdd;
    public float massMult;
    
    LinkedListNode<SubModifier> thrustToken;
    public float thrustAdd;
    public float thrustMult;
    
    public float time;
    
    protected override void Begin()
    {
        massToken = this.gameObject.GetComponent<Body>().massModifier.Add(massAdd, massMult, 0f);
        thrustToken = this.gameObject.GetComponent<Thrust>().thrustModifier.Add(thrustAdd, thrustMult, 0f);
    }
    
    protected override void End()
    {
        this.gameObject.GetComponent<Body>().massModifier.Remove(massToken);
        this.gameObject.GetComponent<Thrust>().thrustModifier.Remove(thrustToken);
    }
    
    protected override float timeLimit{ get{ return time; } }
}