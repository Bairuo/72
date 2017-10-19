using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenView : ExNetworkBehaviour
{
    public float time;
    void OnTriggerEnter2D(Collider2D x)
    {
        if(!Client.IsRoomServer()) return;
        
        var netx = x.gameObject.GetComponent<ExNetworkBehaviour>();
        if(netx == null) return;
        string id = netx.netObject.NetID;
        if(id == null || !ExPlayerController.IsPlayer(netx)) return;
        
        if(ExPlayerController.IsMyPlayer(netx))
        {
            Component.FindObjectOfType<FogController>().MakeVisible();
        }
        string pid = netx.netObject.NetID.Substring("Player-".Length).Substring(0, 1);
        Component.FindObjectOfType<FogController>().SendMakeVisible(pid);
        
        Destroy(this.gameObject);
    }
    
}
