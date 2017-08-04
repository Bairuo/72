using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


public class PosManager
{
    public GameObject prefab;

    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> blocks = new Dictionary<string, GameObject>();

    Dictionary<string, NetUnitData> playersinfo = new Dictionary<string, NetUnitData>();
    Dictionary<string, NetUnitData> blocksinfo = new Dictionary<string, NetUnitData>();

    Dictionary<string, float> LastReceiveTime = new Dictionary<string, float>();

    GameObject player;

    string playerID = "";
    public bool isInit = false;
    public float lastSendTime = float.MinValue;

    public static PosManager instance;
    public PosManager()
    {
        instance = this;
    }
    
    public void PlayerRegister(GameObject player)
    {
        string netID = player.GetComponent<PlayerController>().PlayerID;
        NetUnitData playerinfo = new NetUnitData(netID, player);

        lock(players)players.Add(netID, player);
        lock(playersinfo)playersinfo.Add(netID, playerinfo);
    }
    public void PlayerLogoff(string netID)
    {
        
    }

    public void Close()
    {
        Client.instance.DelListener("UpdateUnitInfo", UpdateUnitInfo);
        Client.instance.DelListener("U", UpdateUnitInfo);
        Client.instance.DelListener("SafyAreaInfo", SafyAreaInfo);

        players.Clear();

        blocks.Clear();

        lastSendTime = float.MinValue;
        isInit = false;
    }
    public void Init(string id) 
    {
        if (isInit) return;
        isInit = true;

        playerID = id;
        Client.instance.AddListener("UpdateUnitInfo", UpdateUnitInfo);
        Client.instance.AddListener("U", UpdateUnitInfo);
        Client.instance.AddListener("SafyAreaInfo", SafyAreaInfo);

    }

    public void SendPos()
    {
        if (playerID != "0") return;

        /*
        if (players.ContainsKey(playerID)) {
            if (players[playerID].active == true)
            {
                int DataID = playersinfo[playerID].GetDataID();
                ProtocolBytes unitproto = playersinfo[playerID].GetUnitData(DataID, "UpdateUnitInfo", playerID, players[playerID].transform.position);
                ProtocolBytes UDPunitproto = playersinfo[playerID].GetUDPUnitData(DataID, "U", playerID, players[playerID].transform.position);

                unitproto.AddFloat(players[playerID].GetComponent<Rigidbody2D>().velocity.x);
                unitproto.AddFloat(players[playerID].GetComponent<Rigidbody2D>().velocity.y);

                //Client.instance.UDPSend(UDPunitproto);
                //Client.instance.UDPP2PBroadcast(UDPunitproto);
                Client.instance.Send(unitproto);
            }

        }*/

        foreach (var item in players)
        {
            if (item.Value.active == true)
            {
                int DataID = playersinfo[item.Key].GetDataID();
                ProtocolBytes unitproto = playersinfo[item.Key].GetUnitData(DataID, "UpdateUnitInfo", item.Key, item.Value.transform.position);
                ProtocolBytes UDPunitproto = playersinfo[item.Key].GetUDPUnitData(DataID, "U", item.Key, item.Value.transform.position);

                unitproto.AddFloat(item.Value.GetComponent<Rigidbody2D>().velocity.x);
                unitproto.AddFloat(item.Value.GetComponent<Rigidbody2D>().velocity.y);
                UDPunitproto.AddFloat(item.Value.GetComponent<Rigidbody2D>().velocity.x);
                UDPunitproto.AddFloat(item.Value.GetComponent<Rigidbody2D>().velocity.y);

                //Client.instance.Send(unitproto);
                Client.instance.UDPSend(UDPunitproto);
            }
        }

        Client.instance.SendSafyAreaInfo(SaftyArea.instance.radius);
        
    }

    public void SafyAreaInfo(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        float radius = proto.Getfloat(start, ref start);

        SaftyArea.instance.radius = radius;
    }

    public void UpdateUnitInfo(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int DataID = proto.GetInt(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);
        float z = proto.Getfloat(start, ref start);

        float velocity_x = proto.Getfloat(start, ref start);
        float velocity_y = proto.Getfloat(start, ref start);

        Vector3 pos = new Vector3(x, y, z);
        Vector2 velocity = new Vector2(velocity_x, velocity_y);

        //Debug.Log(protoName + " DataID:" + DataID + " velocity " + velocity);

        UpdateUnitInfo(id, DataID, pos, velocity);
        
    }
    public void UpdateUnitInfo(string id, int DataID, Vector3 pos, Vector2 velocity)
    {
        //Debug.Log(id);
        if (blocksinfo.ContainsKey(id))
        {
            if (Sys.IsOrderRight(blocksinfo[id].LastReceiveID, DataID))
            {
                blocksinfo[id].Update(pos);
                blocksinfo[id].LastReceiveID = DataID;
                //Debug.Log(blocksinfo[id].fpos);
            }

        }

        if (playersinfo.ContainsKey(id))
        {
            if (Sys.IsOrderRight(playersinfo[id].LastReceiveID, DataID))
            {
                playersinfo[id].Update(pos);
                playersinfo[id].LastReceiveID = DataID;
                //players[id].GetComponent<Rigidbody2D>().velocity = velocity;
                players[id].GetComponent<PlayerController>().fict_velocity = velocity;
                //Debug.Log(playersinfo[id].fpos);
            }

        }
    }

    public void PlayerDestroy(string net_id)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().PlayerDestroy();
        }
    }
    public void PlayerClick(string net_id, float x, float y, float z)
    {
        if (playerID != "0") return;
        Vector3 ClickPos = new Vector3(x, y, z);
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().AddForce(ClickPos);
        }
    }
    public void ChangePosition(string net_id, float x, float y, float z)
    {
        if (players.ContainsKey(net_id))
        {
            lock (playersinfo[net_id]) playersinfo[net_id].fpos = playersinfo[net_id].lpos = new Vector3(x, y, z);
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangePosition(x, y, z);
        }
    }

    public void ChangeMassLevel(string net_id, int masslevel)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangeMassLevel(masslevel);
        }
    }
    public void ChangeSpeedLevel(string net_id, int speedlevel)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangeSpeedLevel(speedlevel);
        }
    }
    public void ChangeMass(string net_id, float mass)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangeMass(mass);
        }
    }
    public void ChangeBrake(string net_id, float brake)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangeBrake(brake);
        }
    }
    public void ChangeSpeed(string net_id, float speed)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangeSpeed(speed);
        }
    }
    public void ChangeHealth(string net_id, float health)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangeHealth(health);
        }
    }
    public void ChangeStatus(string net_id, int status)
    {
        if (players.ContainsKey(net_id))
        {
            lock (players[net_id]) players[net_id].GetComponent<PlayerController>().RealChangeStatus(status);
        }
    }
  
    public void Update()
    {
        //0.085,0.075,0.1,0.15,0.19,0.2
        if (Time.time - lastSendTime > 0.075f)
        {
            SendPos();
            lastSendTime = Time.time;
        }

        if (playerID == "0") return;
        foreach (var item in players)
        {
            //Debug.Log(item.Key);
            if (item.Value == null || item.Value.active == false) continue;
            GameObject player = item.Value;
            string id = item.Key;
            Vector3 fpos = playersinfo[id].fpos;
            Vector3 pos = player.transform.position;

            
            //if (player.GetComponent<PlayerController>().PlayerID == playerID) continue;
            //Debug.Log(id + " " + fpos);
            lock(player)player.transform.position = Vector3.Lerp(pos, fpos, playersinfo[id].delta);
        }

    }
    
}
