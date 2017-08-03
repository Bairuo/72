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
    /*
    public void PlayerRegister(GameObject player)
    {
        string netID = player.GetComponent<PlayerController>().netID;
        NetUnitData playerinfo = new NetUnitData(netID, player);

        //Debug.Log("Register: " + netID);
        lock(players)players.Add(netID, player);
        lock(playersinfo)playersinfo.Add(netID, playerinfo);
    }*/

    public void Close()
    {
        Client.instance.DelListener("UpdateUnitInfo", UpdateUnitInfo);
        Client.instance.DelListener("U", UpdateUnitInfo);

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
    }
    public void StartFight() //发送初始化信息 
    {
        //发送自己的信息
        CharactAttribute info = new CharactAttribute(player);
        ProtocolBytes proto = info.GetInfoProto("AddPlayer");
        Client.instance.Send(proto);

        //房主或服务器
        /*
        if (playerID == "0") 
        {
            foreach (var item in GameController.instance.enemies)
            {
                item.Value.GetComponent<EnemyController>().Target = player;
                info = new CharactAttribute(item.Value);
                Client.instance.Send(info.GetInfoProto("AddEnemy"));
            }
            foreach (var item in GameController.instance.enemies2)
            {
                item.Value.GetComponent<EnemyController>().Target = player;
                info = new CharactAttribute(item.Value);
                Client.instance.Send(info.GetInfoProto("AddEnemy"));
            }
        }
        else
        {
            GameController.instance.ClearEnemy();
            Client.instance.AddListener("UpdateUnitInfo", UpdateUnitInfo);
        }
         * */

    }

    public void SendPos()
    {
        if (players.ContainsKey(playerID)) {
            if (players[playerID].active == true)
            {
                int DataID = playersinfo[playerID].GetDataID();
                ProtocolBytes unitproto = playersinfo[playerID].GetUnitData(DataID, "UpdateUnitInfo", playerID, players[playerID].transform.position);
                ProtocolBytes UDPunitproto = playersinfo[playerID].GetUDPUnitData(DataID, "U", playerID, players[playerID].transform.position);

                Client.instance.UDPSend(UDPunitproto);
                //Client.instance.UDPP2PBroadcast(UDPunitproto);
                //Client.instance.Send(unitproto);
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
        Vector3 pos = new Vector3(x, y, z);

        //Debug.Log(protoName + " DataID:" + DataID);

        UpdateUnitInfo(id, DataID, pos);
        
    }
    public void UpdateUnitInfo(string id, int DataID, Vector3 pos)
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
                //Debug.Log(playersinfo[id].fpos);
            }

        }
    }
    /*
    public void EnterBlock(string playerID, string blockID)
    {
        if (players.ContainsKey(playerID) && blocks.ContainsKey(blockID))
        {
            players[playerID].GetComponent<PlayerController>().PlayerEnter(players[playerID], blocks[blockID]);
        }
    }*/
    /*
    public void LeaveBlock(string blockID)
    {
        if (blocks.ContainsKey(blockID))
        {
            lock (blocks[blockID])
            {
                TransformController t = blocks[blockID].GetComponentInChildren<TransformController>();
                if (t != null)
                {
                    t.PlayerLeave();
                }
            }
        }
    }*/
    public void Hit(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int isPlayer = proto.GetInt(start, ref start);
        int damage = proto.GetInt(start, ref start);

        GameObject temp;
        if (isPlayer == 1)
        {
            if (id == playerID || !players.ContainsKey(id)) return;
            temp = players[id];
        }
        else
        {
            if (playerID == "0" || !blocks.ContainsKey(id)) return;
            temp = blocks[id];
        }

    }

    // UDP speed up
    string id_last = "-1";
    int forward_last = -1;
    bool stand_last = false;
    float x_last = 0, y_last = 0, z_last = 0;

  
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
            if (item.Value.active == false) continue;
            GameObject player = item.Value;
            string id = item.Key;
            Vector3 fpos = playersinfo[id].fpos;
            Vector3 pos = player.transform.position;

            
            //if (player.GetComponent<PlayerController>().netID == playerID) continue;
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
