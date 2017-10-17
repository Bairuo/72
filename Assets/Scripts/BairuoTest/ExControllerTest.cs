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
        AddProtocol("Data", DataToClient, DataToServer, ServerReceive, ClientReceive,
            typeof(string), typeof(Vector2), typeof(Vector3), typeof(Quaternion), typeof(Color));
	}

    void Update()
    {
        if (Time.time - lastTime > 1f)
        {
            lastTime = Time.time;
            
            Send("Tex"); // will pull info from Senders.
            
            Send("Data");
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
    
    object[] DataToClient()
    {
        return new object[]{
            "Data to client: ",
            new Vector2(1f, 2f),
            new Vector3(-3f, -4f, -5f),
            Quaternion.identity,
            new Color(1f, 1f, 1f, 1f)};
    }
    
    void ClientReceive(object[] info)
    {
        Debug.Log(info[0] as string);
        Debug.Log((Vector2)info[1]);
        Debug.Log((Vector3)info[2]);
        Debug.Log((Quaternion)info[3]);
        Debug.Log((Color)info[4]);
    }
    
    
    object[] DataToServer()
    {
        return new object[]{
            "Data to server: ",
            new Vector2(-1f, -2f),
            new Vector3(-3f, -4f, -5f),
            Quaternion.Inverse(Quaternion.identity),
            new Color(0f, 0f, 0f, 1f)};
    }
    
    void ServerReceive(object[] info)
    {
        Debug.Log(info[0] as string);
        Debug.Log((Vector2)info[1]);
        Debug.Log((Vector3)info[2]);
        Debug.Log((Quaternion)info[3]);
        Debug.Log((Color)info[4]);
    }
    
}
