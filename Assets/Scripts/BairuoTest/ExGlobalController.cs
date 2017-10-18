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
    
    bool bossCreated = false;
    
    void Update()
    {
        // ========================== Players creation =========================
        if(!playerInited)
        {
            if(Client.IsRoomServer())
            {
                // Create player object for server.
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
        // =============================== Boss Create ================================
        if(!bossCreated)
        {
            bossCreated = true;
            Component.FindObjectOfType<ExGameObjectCreator>().GlobalGameObjectCreate(
                "Map/boss",
                "Boss",
                Vector2.zero,
                0f,
                "Boss"
            );
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
        Debug.Log("creation request send.");
        return new object[]{Client.instance.playerid};
    }
    
    void ReceiveCreatePlayerRequest(object[] info)
    {
        Debug.Log("creation request receive and applied.");
        
        string id = info[0] as string;
        
        /// Create player object for clients.
        Component.FindObjectOfType<ExGameObjectCreator>().GlobalGameObjectCreate(
            "Player/XPlayer",
            "Player-" + id,
            Util.RandPos(Vector2.zero, 12f),
            Util.RandAngle(),
            "Player-" + id);
    }
}
