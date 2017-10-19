using System;
using System.Threading;
using UnityEngine;

public class ExSynchronizedState : ExNetworkBehaviour
{
    public float UpdateDeltaTime;
    
    
    protected override void Start()
    {
        base.Start();
        AddProtocol("Sync", State, null, null, ReceiveState,
            typeof(Vector2), typeof(float));
    }
    
    void FixedUpdate()
    {
        Send("Sync");
    }
    
    object[] State()
    {
        return new object[]{
            (Vector2)this.gameObject.transform.position,
            Calc.RotationAngleZ(this.gameObject.transform.rotation)};
    }
    
    void ReceiveState(object[] info)
    {
        this.gameObject.transform.position = (Vector2)info[0];
        this.gameObject.transform.rotation = Calc.GetQuaternion((float)info[1]);
    }
}