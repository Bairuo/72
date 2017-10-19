using System;
using UnityEngine;
using System.Collections.Generic;

public class ExGameObjectCreator : ExNetworkBehaviour
{
    public static ExGameObjectCreator instance;
    
    protected override void Start()
    {
        instance = this;
        
        base.Start();
        
        // Protocol GameObjectCreate ...
        // string : Prefab path.
        // string : playerid.
        AddProtocol("GameObjectCreate", SendInfo, null, null, CreateProcess,
            typeof(string), typeof(string), typeof(Vector2), typeof(float), typeof(string));
        
        prepared = true;
    }
    
    GameObject LocalCreateGameObject(GameObject prefab, Vector2 loc, float dir, string netid)
    {
        GameObject x = Instantiate(prefab, loc, Calc.GetQuaternion(dir));
        x.name = "[Net]" + netid;
        ExNetworkBehaviour[] nx = x.GetComponents<ExNetworkBehaviour>();
        int i = 0;
        foreach(var n in nx)
        {
            n.netObject.NetID = netid + (i += 1);
        }
        return x;
    }
    
    /// Will only be used by clients.
    void CreateProcess(object[] info)
    {
        string path = info[0] as string;
        Vector2 loc = (Vector2)info[2];
        float rot = (float)info[3];
        string netid = info[4] as string;
        
        Debug.LogFormat("Local create ({0}), resault ({1})", path, Resources.Load(path) as GameObject);
        
        LocalCreateGameObject(Resources.Load(path) as GameObject, loc, rot, netid);
    }
    
    object[] temp;
    object[] SendInfo() { return temp; }
    
    /// Create object, providing name and netid to locate the objects.
    public GameObject GlobalGameObjectCreate(string path, string name, Vector2 loc, float dir, string netid)
    {
        if(!Client.IsRoomServer()) return null; // clients cannot create game object.
        temp = new object[]{path, name, loc, dir, netid};
        Send("GameObjectCreate");
        return LocalCreateGameObject(Resources.Load(path) as GameObject, loc, dir, netid);
    }
    
}