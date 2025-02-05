﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************
 * 作者：Bairuo
 * 创建时间：2017.10.1
 * 
 * 
 * ************************/
public class SynSystem
{     // 此类原本应设计为静态类由于历史原因，但由于历史原因，只能放在PosManager里面构造
    static Dictionary<string, NetObject> NetObjects = new Dictionary<string, NetObject>();
    public static string SystemFlag = "CallSyn";
    public static string SystemUserFlag = "CallSynUser";
    public static string VarFlag = "SynVar";
    public static bool IsUse;

    // 原应设计为静态构造
    public SynSystem()
    {
        if (Client.instance != null)
        {
            IsUse = true;   //
            Client.instance.AddListener(SystemFlag, CallSyn);
            Client.instance.AddListener(SystemUserFlag, CallSynUser);
        }
    }

    // Flag:CallSyn 协议数量 + 物体ID| 协议1 | + 物体ID| 协议2 |
    // 协议：变量ID.变量1 + 变量ID.变量2 + 变量ID.变量3
    public static void CallSyn(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;

        proto.GetString(start, ref start);
        int n = proto.GetInt(start, ref start);
        proto.GetString(start, ref start);

        proto.GetRestProtocol(start);

        while (n > 0)
        {
            n--;
        }

    }

    // Flag:CallSynUser 物体ID + | 协议 |
    // 协议：变量1 + 变量2 + 变量3
    public static void CallSynUser(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;

        proto.GetString(start, ref start);
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
    
    public static void Remove(string id)
    {
        lock (NetObjects) NetObjects.Remove(id);
    }
}
