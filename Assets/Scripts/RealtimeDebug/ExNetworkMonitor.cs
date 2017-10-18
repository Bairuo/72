using UnityEngine;

public class ExNetworkMonitor : MonoBehaviour
{
    public ExNetworkBehaviour target;
    
    public string localClientID;
    
    public string netID;
    public bool prepared;
    public string gameObjectName;
    
    
    void Update()
    {
        localClientID = Client.instance.playerid;
        
        if(target == null) return;
        
        netID = target.netObject.NetID;
        prepared = target.prepared;
        gameObjectName = target.gameObject.name;
    }
    
    
}