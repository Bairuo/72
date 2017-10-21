using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

// 此类拟改名
public class PosManager
{
    public GameObject prefab;

    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> blocks = new Dictionary<string, GameObject>();

    Dictionary<string, NetUnitData> playersinfo = new Dictionary<string, NetUnitData>();
    Dictionary<string, NetUnitData> blocksinfo = new Dictionary<string, NetUnitData>();

    // Dictionary<string, float> LastReceiveTime = new Dictionary<string, float>();

    GameObject player;

    // string playerID = "";
    public bool isInit = false;
    public float lastSendTime = float.MinValue;

    public static PosManager instance;
    public PosManager()
    {
        instance = this;
    }
    
    public void PlayerRegister(GameObject player)
    {
        string netID = player.GetComponent<ExNetworkBehaviour>().netObject.NetID;
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
        Client.instance.DelListener("U", UpdateUnitInfo);   // UDP位置同步
        Client.instance.DelListener("SafyAreaInfo", SafyAreaInfo);
        Client.instance.DelListener("TaggerGenerate", TaggerGenerate);
        Client.instance.DelListener("PropGenerate", PropGenerate);

        players.Clear();

        blocks.Clear();

        lastSendTime = float.MinValue;
        isInit = false;
    }
    public void Init(string id)
    {
        if (isInit) return;
        isInit = true;

        // playerID = id;

        // 原应将SynSystem设计为静态构造
        if (!SynSystem.IsUse) new SynSystem();
        
        // 重要协议注册， 位置同步，玩家点击操作，安全区信息，物体创建
        Client.instance.AddListener("UpdateUnitInfo", UpdateUnitInfo);
        Client.instance.AddListener("U", UpdateUnitInfo);
        Client.instance.AddListener("SafyAreaInfo", SafyAreaInfo);
        Client.instance.AddListener("TaggerGenerate", TaggerGenerate);
        Client.instance.AddListener("PropGenerate", PropGenerate);
    }

    public void SendPos()
    {
        if (!Client.IsRoomServer()) return;

        foreach (var item in players)
        {
            if (item.Value != null && item.Value.activeInHierarchy == true)
            {
                int DataID = playersinfo[item.Key].GetDataID();
                //ProtocolBytes unitproto = playersinfo[item.Key].GetUnitData(DataID, "UpdateUnitInfo", item.Key, item.Value.transform.position);
                ProtocolBytes UDPunitproto = playersinfo[item.Key].GetUDPUnitData(DataID, "U", item.Key, item.Value.transform.position);

                //unitproto.AddFloat(item.Value.GetComponent<Body>().velocity.x);
                //unitproto.AddFloat(item.Value.GetComponent<Body>().velocity.y);
                
                var body = item.Value.GetComponent<Body>();
                if(body != null)
                {
                    UDPunitproto.AddFloat(body.velocity.x);
                    UDPunitproto.AddFloat(body.velocity.y);
                }
                else
                {
                    UDPunitproto.AddFloat(0f);
                    UDPunitproto.AddFloat(0f);
                }
                
                //Client.instance.Send(unitproto);
                Client.instance.UDPSend(UDPunitproto);
            }
        }

        if (SaftyArea.instance != null) Client.instance.SendSafyAreaInfo(SaftyArea.instance.radius);
        
    }

    public void SafyAreaInfo(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        proto.GetString(start, ref start);
        float radius = proto.GetFloat(start, ref start);

        if (SaftyArea.instance != null) SaftyArea.instance.radius = radius;
    }
    
    public void UpdateUnitInfo(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        proto.GetString(start, ref start);
        int DataID = proto.GetInt(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);
        float z = proto.GetFloat(start, ref start);

        float velocity_x = proto.GetFloat(start, ref start);
        float velocity_y = proto.GetFloat(start, ref start);

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

        if (playersinfo.ContainsKey(id) && players[id] != null)
        {
            if (Sys.IsOrderRight(playersinfo[id].LastReceiveID, DataID))
            {
                playersinfo[id].Update(pos);
                playersinfo[id].LastReceiveID = DataID;
                
                if(players[id].GetComponent<Body>() != null)
                    players[id].GetComponent<Body>().velocity = velocity;

                //players[id].GetComponent<PlayerController>().fict_velocity = velocity;
                //Debug.Log(playersinfo[id].fpos);
            }

        }
    }

    public void TaggerGenerate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        proto.GetString(start, ref start);
        string tag = proto.GetString(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);

        TaggerGenerate(tag, new UnityEngine.Vector2(x, y));
    }
    public void PropGenerate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        proto.GetString(start, ref start);
        string tag = proto.GetString(start, ref start);
        int id = proto.GetInt(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);

        PropGenerate(tag, id, new UnityEngine.Vector2(x, y));
    }
    
    Dictionary<string, ObjectGenerator> Generators = new Dictionary<string, ObjectGenerator>();
    public void TaggerGenerate(string tag, Vector2 loc)
    {
        ObjectGenerator Generator;
        if (Generators.ContainsKey(tag))
        {
            Generator = Generators[tag];
        }
        else
        {
            Generator = GameObject.FindGameObjectWithTag(tag).GetComponent<ObjectGenerator>();
            Generators.Add(tag, Generator);
        }

        if (Generator != null)
        {
            Generator.GenerateTagger(loc);
        }
    }
    public void PropGenerate(string tag, int id, Vector2 loc)
    {
        ObjectGenerator Generator;
        if (Generators.ContainsKey(tag))
        {
            Generator = Generators[tag];
        }
        else
        {
            Generator = GameObject.FindGameObjectWithTag(tag).GetComponent<ObjectGenerator>();
            Generators.Add(tag, Generator);
        }

        if (Generator != null)
        {
            Generator.Generate(id, loc);
        }
    }
    public void ChangePosition(string net_id, float x, float y, float z)
    {
        if (players.ContainsKey(net_id))
        {
            lock (playersinfo[net_id]) playersinfo[net_id].fpos = playersinfo[net_id].lpos = new Vector3(x, y, z);
            lock (players[net_id]) players[net_id].transform.position = new Vector3(x, y, z);
        }
    }

    public void Update()
    {
        // 0.085,0.075,0.1,0.15,0.19,0.2
        if (Time.time - lastSendTime > 0.075f)
        {
            SendPos();
            lastSendTime = Time.time;
        }

        // 预测式状态同步
        if (Client.IsRoomServer()) return;
        foreach (var item in players)
        {
            //Debug.Log(item.Key);
            if (item.Value == null || item.Value.activeInHierarchy == false) continue;
            GameObject player = item.Value;
            string id = item.Key;
            Vector3 fpos = playersinfo[id].fpos;
            Vector3 pos = player.transform.position;

            lock(player)player.transform.position = Vector3.Lerp(pos, fpos, playersinfo[id].delta);
        }

    }
    
}
