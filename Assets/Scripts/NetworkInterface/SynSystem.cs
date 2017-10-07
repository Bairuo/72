using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************
 * 作者：Bairuo
 * 创建时间：2017.10.1
 * 
 * 
 * ************************/
public class SynSystem{
    static Dictionary<string, NetObject> NetObjects = new Dictionary<string, NetObject>();
    public static string SystemFlag = "CallSyn";
    public static string SystemUserFlag = "CallSynUser";
    public static string VarFlag = "SynVar";

  
    //public static SynSystem()
    //{
    //    if (Client.instance != null)
    //    {
    //        Client.instance.AddListener(SystemFlag, CallSyn);
    //        Client.instance.AddListener(SystemUserFlag, CallSynUser);
    //    }
    //}

    // Flag:CallSyn 协议数量 + 物体ID| 协议1 | + 物体ID| 协议2 |
    public static void CallSyn(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;

        string name = proto.GetString(start, ref start);
        int n = proto.GetInt(start, ref start);
        string id = proto.GetString(start, ref start);

        ProtocolBytes restProto = proto.GetRestProtocol(start);

        while (n > 0)
        {
            

            n--;
        }

    }

    // Flag:CallSynUser 物体ID + | 协议 |
    public static void CallSynUser(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;

        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);

        ProtocolBytes objProtocol = proto.GetRestProtocol(start);

        if (NetObjects.ContainsKey(id))
        {
            NetObjects[id].DispatchMsgEvent(objProtocol);
        }
    }

    public void Update()
    {
        int length = NetObjects.Values.Count;
        if (length == 0) return;

        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString(SystemFlag);
        protocol.AddInt(length);

        foreach (NetObject obj in NetObjects.Values)
        {
            // 整合每帧所有物体所有变量要同步的消息一起发送

            //obj.Send();
        }
    }

    public static void Register(string id, NetObject obj)
    {
        lock (NetObjects) NetObjects.Add(id, obj);
    }
}
