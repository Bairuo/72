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

        }

        foreach (var item in blocks)
        {
            /*
            if (item.Value.GetComponentInChildren<TransformController>() != null)
            {
                string netID = item.Value.GetComponentInChildren<TransformController>().controllerID;
                if (netID != playerID) continue;

                Block block = item.Value.GetComponent<Block>();

                int DataID = blocksinfo[item.Key].GetDataID();
                ProtocolBytes unitproto = blocksinfo[item.Key].GetUnitData(DataID, "UpdateUnitInfo", block.net_id, item.Value.transform.position);
                ProtocolBytes UDPunitproto = blocksinfo[item.Key].GetUDPUnitData(DataID, "U", block.net_id, item.Value.transform.position);

                Client.instance.UDPSend(UDPunitproto);

            }*/
        }

        if (playerID == "0") Client.instance.SendSafyAreaInfo(SaftyArea.instance.radius);
        
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

        //Debug.Log(protoName + " DataID:" + DataID);

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
        if (Time.time - lastSendTime > 0.085f)
        {
            SendPos();
            lastSendTime = Time.time;
        }

        //Debug.Log("??");
        //Debug.Log(players.Count);

        foreach (var item in players)
        {
            //Debug.Log(item.Key);
            if (item.Value == null || item.Value.active == false) continue;
            GameObject player = item.Value;
            string id = item.Key;
            Vector3 fpos = playersinfo[id].fpos;
            Vector3 pos = player.transform.position;

            
            if (player.GetComponent<PlayerController>().PlayerID == playerID) continue;
            //Debug.Log(id + " " + fpos);
            lock(player)player.transform.position = Vector3.Lerp(pos, fpos, playersinfo[id].delta);
        }

        foreach (var item in blocks)
        {/*
            if (item.Value.GetComponentInChildren<TransformController>() != null)
            {
                string netID = item.Value.GetComponentInChildren<TransformController>().controllerID;
                if (netID == playerID) continue;
                //Debug.Log(2);
                //Debug.Log("Update" + item.Key + " " + blocksinfo[item.Key].fpos);

                GameObject block = item.Value;
                string id = item.Key;
                Vector3 fpos = blocksinfo[id].fpos;
                Vector3 pos = block.transform.position;

                lock (block) block.transform.position = Vector3.Lerp(pos, fpos, blocksinfo[id].delta);
            }*/
        }
    }
    
}
