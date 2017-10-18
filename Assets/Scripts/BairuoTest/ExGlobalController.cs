using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExGlobalController : ExNetworkBehaviour
{
    protected override void Awake()
    {
        Client.instance.posmanager.Init(Client.instance.playerid);
        base.Awake();
        AddProtocol("CreatePlayerRequest", null, SendCreatePlayerRequest, ReceiveCreatePlayerRequest, null, typeof(string));
    }
	
    bool playerInited = false;
    
    void Update()
    {
        // ========================== Players creation =========================
        if(!playerInited)
        {
            if(Client.IsRoomServer())
            {
                Component.FindObjectOfType<ExGameObjectCreator>().GlobalGameObjectCreate(
                    "Player/XPlayer",
                    "Player-" + Client.instance.playerid,
                    Util.RandPos(Vector2.zero, 12f),
                    Util.RandAngle(),
                    "Player-" + Client.instance.playerid);
            }
            else
            {
                Send("CreatePlayerRequest");
            }
            playerInited = true;
        }
        
        
        // ========================== Local scene scripts =============================
        PhysWorld pw = Component.FindObjectOfType<PhysWorld>();
        if(Client.IsRoomServer())
        {
            pw.applyCollision = true;
            pw.applyPhysStep = true;
        }
        else
        {
            pw.applyCollision = false;
            pw.applyPhysStep = true;
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
            Util.RandPos(Vector2.zero, 12f),
            Util.RandAngle(),
            "Player-" + id);
    }
}
