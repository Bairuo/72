using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExControllerTest : ExNetworkBehaviour
{
    public float Health = 10;
    public int Damage = 10;
    float lastTime = 0;
    
    public override string protocolName{ get{ return "Test!"; } }
    
	protected override void Start()
    {
        base.Start();
	}

    void Update()
    {
        if (Time.time - lastTime > 0.65f)
        {
            lastTime = Time.time;
            if (!Client.IsRoomServer())
            {
                foo();
            }
        }

    }


    public void foo()
    {
        Damage++;
        
        Send(Damage, Health);
    }
    
    
    protected override void Recieve()
    {
        Damage = GetInt();
        Health = GetFloat();
    }
    
}
