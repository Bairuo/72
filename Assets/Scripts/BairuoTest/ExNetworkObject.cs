using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExNetworkObject : NetworkBehaviour
{
    public float Health = 10;
    public int Damage = 10;
    float lastTime = 0;
    public bool Test;
    
    protected override string networkName{ get{ return "exp"; } }
    
    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastTime > 0.65f)
        {
            lastTime = Time.time;
            // Sender : Server.
            if(!Client.IsRoomServer())
            {
                Debug.Log("Send! " + Client.instance.playerid);
                foo();
            }
            
        }

    }


    public void foo()
    {
        Damage++;
        Send(Damage, Health);
    }
    
    // Receiver : Clients.
    public override void NetworkCallback()
    {
        Debug.LogError("rec : " + Client.instance.playerid);
        Debug.Log(GetInt());
        Debug.Log(GetFloat());
    }
}
