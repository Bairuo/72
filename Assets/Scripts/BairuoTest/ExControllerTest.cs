using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExControllerTest : ExNetworkBehaviour
{
    float lastTime = 0;
    
	protected override void Awake()
    {
        base.Awake();
        AddProtocol("Tex", SendToClient, SendToServer, Receive, Receive, typeof(string));
	}

    void Update()
    {
        if (Time.time - lastTime > 1f)
        {
            lastTime = Time.time;
            
            Send("Tex"); // will pull info from Senders.
        }
    }
    
    object[] SendToServer()
    {
        return new object[]{"Client sended info, id : " + Client.instance.playerid};
    }
    
    object[] SendToClient()
    {
        return new object[]{"Server sended info, id : " + Client.instance.playerid};
    }
    
    void Receive(object[] info)
    {
        Debug.LogError("Receive info : " + (info[0] as string));
    }
    
    
    
    
}
