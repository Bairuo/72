using System;
using UnityEngine;
using System.Collections.Generic;

public class ExGameObjectCreator : ExNetworkBehaviour
{
    bool prepared = false;
    
    protected override void Start()
    {
        base.Start();
        
        // Protocol GameObjectCreate ...
        // string : Prefab path.
        // string : playerid.
        AddProtocol("GameObjectCreate", SendInfo, null, null, CreateProcess,
            typeof(string), typeof(string), typeof(Vector2), typeof(float), typeof(string));
        
        prepared = true;
    }
    
    bool inited = false;
    void Update()
    {
        if(!inited)
        {
            inited = true;
        }
    }
    
    GameObject LocalCreateGameObject(GameObject prefab, Vector2 loc, float dir, string netid)
    {
        GameObject x = Instantiate(prefab, loc, Quaternion.Euler(0f, 0f, dir));
        NetObject nx = x.GetComponent<NetObject>();
        nx.ObjectRegister(netid);
        return x;
    }
    
    /// Will only be used by clients.
    void CreateProcess(object[] info)
    {
        string path = info[0] as string;
        string id = info[1] as string;
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