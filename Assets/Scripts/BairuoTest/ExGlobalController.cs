using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExGlobalController : ExNetworkBehaviour
{
    bool prepared = false;
    
    protected override void Awake()
    {
        base.Awake();
        Client.instance.posmanager.Init(Client.instance.playerid);
        
        AddProtocol("CreatePlayerRequest", null, SendCreatePlayerRequest, ReceiveCreatePlayerRequest, null, typeof(string));
        
        prepared = true;
    }
	
    bool playerInited = false;
    
    void Update()
    {
        // Create the players at the beginning.
        if(!playerInited)
        {
            if(Client.IsRoomServer())
            {
                (Component.FindObjectOfType(typeof(ExGameObjectCreator)) as ExGameObjectCreator).GlobalGameObjectCreate(
                    "Player/XPlayer",
                    "Player-" + Client.instance.playerid,
                    new Vector2(0f, 0f),
                    0f,
                    Client.instance.playerid);
            }
            else
            {
                Send("CreatePlayerRequest");
            }
            playerInited = true;
        }
        
        Client.instance.Update();
	}
    
    // =================================================================================
    //                             Player Creation Section
    // =================================================================================
    // protocol CreatePlayerRequest
    // string : player id. The created object will be set to this id.
    
    object[] SendCreatePlayerRequest()
    {
        Debug.LogError("creation request send.");
        return new object[]{Client.instance.playerid};
    }
    
    void ReceiveCreatePlayerRequest(object[] info)
    {
        Debug.LogError("creation request receive and applied.");
        
        string id = info[0] as string;
        
        (Component.FindObjectOfType(typeof(ExGameObjectCreator)) as ExGameObjectCreator).GlobalGameObjectCreate(
            "Player/XPlayer",
            "Player-" + id,
            new Vector2(0f, 0f),
            0f,
            id);
    }
}
