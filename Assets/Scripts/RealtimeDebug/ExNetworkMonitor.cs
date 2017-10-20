using UnityEngine;

public class ExNetworkMonitor : MonoBehaviour
{
    public ExNetworkBehaviour target;
    
    public string localClientID;
    
    public string[] netID;
    public bool prepared;
    public string gameObjectName;
    
    
    void Update()
    {
        localClientID = Client.instance.playerid;
        
        if(target == null) return;
        
        var x = target.GetComponents<ExNetworkBehaviour>();
        netID = new string[x.Length];
        for(int i=0; i<x.Length; i++) netID[i] = x[i].netObject.NetID;
        prepared = target.prepared;
        gameObjectName = target.gameObject.name;
    }
    
    
}