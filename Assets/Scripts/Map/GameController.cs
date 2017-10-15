using UnityEngine;
using System.Collections.Generic;

public class GameController : NetworkBehaviour
{
    protected override string networkName{ get{ return "GameController"; } }
    
	protected override void Start()
    {
        Client.instance.posmanager.Init(Client.instance.playerid);
        base.Start();
        
        // ====================================================================
        // start script defined here...
        // ====================================================================
        
        /// Clients Send create requirement to server.
        if(!Client.IsRoomServer())
        {
            Send("CreateRequirement", Client.instance.playerid);
        }
        
        /// Server directly generate its own player "player0".
        CreateObject("Player/XPlayer", "player" + Client.instance.playerid, RandPos(), Quaternion.identity);
    }
    
    Vector2 RandPos(){ return new Vector2(Random.Range(-4f, 4f), Random.Range(-4f, 4f)); }
    
    /// Server knows that I should create an other object.
    void CreateReqCallback()
    {
        string id = GetString();
        CreateObject("Player/XPlayer", "player" + id, RandPos(), Quaternion.identity);
    }
    
    /// Create an object on each computer.
    void CreateObject(string prefabName, string name, Vector2 loc, Quaternion rot)
    {
        if(!Client.IsRoomServer()) return;
        Send("Create", prefabName, name, loc, rot);
        Debug.Log("Create on server: " + name);
        GameObject x = Instantiate(Resources.Load(prefabName) as GameObject, loc, rot);
        x.GetComponent<NetObject>().ObjectRegister(name);
        x.name = name;
    }
    void CreateObjectCallback()
    {
        string prefabname = GetString();
        string name = GetString();
        Debug.Log("Create on client: " + name);
        Vector2 loc = GetVec2();
        Quaternion rot = GetQuat();
        GameObject x = Instantiate(Resources.Load(prefabname) as GameObject, loc, rot);
        x.GetComponent<NetObject>().ObjectRegister(name);
        x.name = name;
    }
    
    public override void NetworkCallback()
    {
        string operation = GetString();
        Debug.Log("msg rcv. " + operation);
        if(operation == "Create") CreateObjectCallback();
        if(operation == "CreateRequirement") CreateReqCallback();
    }
    
    
    void RemoveObject(NetObject o)
    {
        
    }
	
	void Update()
    {
        Client.instance.Update();
	}
}