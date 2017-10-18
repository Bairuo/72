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
        }
        else
        {
            Destroy(this);
            return;
        }
        
        AddProtocol("AddComponent", ComponentSend, null, null, ComponentReceive,
            typeof(string), typeof(string));
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
    
    public void CreateComponentAt(string typeName, GameObject o)
    {
        CreateComponentAt(typeName, o.GetComponent<ExNetworkBehaviour>().netObject.NetID);
    }
    
    public void CreateComponentAt(string typeName, string netid)
    {
        if(!Client.IsRoomServer()) return; // Only server can create things...
        
        curTypeName = typeName;
        curNetID = netid;
        if(!componentTypes.ContainsKey(curTypeName))
        {
            Debug.LogError("Trying to create an unsupported component.");
            return;
        }
        
        /// Create component in clients.
        Send("AddComponent");
        
        /// Local create.
        GetNetworkBehaviour(netid).gameObject.AddComponent(componentTypes[typeName]);
    }
    
    object[] ComponentSend()
    {
        return new object[]{
            curTypeName,
            curNetID};
    }
    
    void ComponentReceive(object[] info)
    {
        string type = info[0] as string;
        string netid = info[1] as string;
        
        GetNetworkBehaviour(netid).gameObject.AddComponent(componentTypes[type]);
    }
    
}