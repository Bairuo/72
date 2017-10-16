using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExControllerTest : ExNetworkBehaviour
{
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
            SendToServer("Client " + Client.instance.playerid + " Send To server.");
            SendToClient("Server " + Client.instance.playerid + " Send to server.");
        }
    }
    
    protected override void ServerReceive(string from)
    {
        Debug.LogError(GetString() + " :: from " + from);
    }
    
    protected override void ClientReceive(string from)
    {
        Debug.LogError(GetString() + " :: from " + from);
    }
    
}
