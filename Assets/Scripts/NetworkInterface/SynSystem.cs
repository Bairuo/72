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


    public static void CallSyn(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;

        string name = proto.GetString(start, ref start);
        int n = proto.GetInt(start, ref start);

        while (n > 0)
        {


            n--;
        }

    }

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
        ProtocolBytes protocol = new ProtocolBytes();
        foreach (NetObject obj in NetObjects.Values)
        {
            // 整合每帧所有物体所有变量要同步的消息一起发送

            obj.Send();
        }
    }

    public static void Register(string id, NetObject obj)
    {
        lock (NetObjects) NetObjects.Add(id, obj);
    }
}
