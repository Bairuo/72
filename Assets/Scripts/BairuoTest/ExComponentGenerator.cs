using UnityEngine;
using System;
using System.Collections.Generic;

/// Global item generator.
/// Defines the rules of item generation.
public class ExComponentGenerator : ExNetworkBehaviour
{
    public static ExComponentGenerator instance;
    
    public static Dictionary<string, Type> componentTypes = null;
    
    protected override void Start()
    {
        base.Start();
        
        /// Multiple ItemGenerator may exist, this shall be inited only once.
        if(instance == null)
        {
            instance = this;
            componentTypes = new Dictionary<string, Type>();
            componentTypes.Add("BuffMassUp", typeof(BuffMassUp));
            componentTypes.Add("BuffSpeedUp", typeof(BuffSpeedUp));
            componentTypes.Add("BuffBurndrive", typeof(Burndrive));
        }
        else
        {
            Destroy(this);
            return;
        }
        
        AddProtocol("AddComponent", ComponentSend, null, null, ComponentReceive,
            typeof(string), typeof(string), typeof(string));
    }
    
    // ==============================================================
    //                     Game Logic Section
    // ==============================================================
    
    void Update()
    {
        // do nothing...
    }
    
    // ==============================================================
    //                 Compoennet Addition Section
    // ==============================================================
    
    string curTypeName;
    string curNetID;
    string curCompID;
    
    public ExNetworkBehaviour CreateComponentAt(string typeName, GameObject o, string compID)
    {
        return CreateComponentAt(typeName, o.GetComponent<ExNetworkBehaviour>().netObject.NetID, compID);
    }
    
    public ExNetworkBehaviour CreateComponentAt(string typeName, string netid, string compID)
    {
        if(!Client.IsRoomServer()) return null; // Only server can create things...
        
        curTypeName = typeName;
        curNetID = netid;
        curCompID = netid + "-" + compID;
        if(!componentTypes.ContainsKey(curTypeName))
        {
            Debug.LogError("Trying to create an unsupported component.");
            return null;
        }
        
        // Create component in clients.
        Send("AddComponent");
        
        // Local create.
        var x = GetNetworkBehaviour(netid).gameObject.AddComponent(componentTypes[typeName]) as ExNetworkBehaviour;
        x.netObject.NetID = netid + "-" + compID;
        
        return x;
    }
    
    object[] ComponentSend()
    {
        return new object[]{
            curTypeName,
            curNetID,
            curCompID};
    }
    
    void ComponentReceive(object[] info)
    {
        string type = info[0] as string;
        string netid = info[1] as string;
        string curid = info[2] as string;
        
        // wiil run Awake().
        var x = GetNetworkBehaviour(netid).gameObject.AddComponent(componentTypes[type]) as ExNetworkComponent;
        
        // Before the Start().
        x.netObject.NetID = curid;
    }
    
}