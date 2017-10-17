using System;
using System.Collections.Generic;
using UnityEngine;

public class ExNetworkBehaviour : MonoBehaviour
{
    NetObject netObject;
    
    public delegate void ServerReceiver(params object[] info);
    public delegate void ClientReceiver(params object[] info);
    public delegate object[] ServerSender();
    public delegate object[] ClientSender();
    
    Dictionary<string, ProtocolEntry> protocols = new Dictionary<string, ProtocolEntry>();
    
    struct ProtocolEntry
    {
        public string protocolName;
        public ServerReceiver serverReceiver;
        public ClientReceiver clientReceiver;
        public ServerSender serverSender;
        public ClientSender clientSender;
        public TypeCode[] types;
        public ProtocolEntry(
            string n = "DEFAULT",
            ServerReceiver sr = null,
            ClientReceiver cr = null,
            ServerSender ss = null,
            ClientSender cs = null,
            TypeCode[] tp = null)
        {
            protocolName = n;
            serverReceiver = sr;
            clientReceiver = cr;
            serverSender = ss;
            clientSender = cs;
            types = tp;
        }
    }
    
    void ProtoSend(string protocolName, params object[] info)
    {
        ProtocolBytes proto = netObject.GetObjectProtocol();
        proto.AddName(protocolName);
        
        // This is a hidden info.
        // used to preventing client receive info from client.
        proto.AddString(Client.instance.playerid);
        
        if(info.Length != protocols[protocolName].types.Length)
        {
            Debug.LogError("The info array and protocol don't have the same length.");
        }
        for(int i=0; i<info.Length; i++)
        {
            if(Type.GetTypeCode(info[i].GetType()) != protocols[protocolName].types[i])
            {
                Debug.LogError("The info array and protocol are not corresponding.");
                return;
            }
        }
        
        foreach(var i in info)
        {
            if(i is string) proto.AddString(i as string);
            else if(i is byte[]) proto.AddByte(i as byte[]);
            else if(i is bool) proto.AddBool((bool)i);
            else if(i is int) proto.AddInt((int)i);
            else if(i is float) proto.AddFloat((float)i);
            else if(i is Vector2)
            {
                Vector2 v = (Vector2)i;
                proto.AddFloat(v.x);
                proto.AddFloat(v.y);
            }
            else if(i is Vector3)
            {
                Vector3 v = (Vector2)i;
                proto.AddFloat(v.x);
                proto.AddFloat(v.y);
                proto.AddFloat(v.z);
            }
            else if(i is Quaternion)
            {
                Quaternion v = (Quaternion)i;
                proto.AddFloat(v.x);
                proto.AddFloat(v.y);
                proto.AddFloat(v.z);
                proto.AddFloat(v.w);
            }
            else Debug.LogError("Try to send a variable that is not supported.");
        }
        
        netObject.Send(proto);
    }
    
    protected virtual void Start()
    {
        netObject = GetComponent<NetObject>();
    }
    
    public void AddProtocol(
        ServerSender serverSender = null, ClientSender clientSender = null,
        ServerReceiver serverReceiver = null, ClientReceiver clientReceiver = null)
    {
        AddProtocol("DEFAULT", serverSender, clientSender, serverReceiver, clientReceiver);
    }
    
    public void AddProtocol(
        string name,
        ServerSender serverSender = null, ClientSender clientSender = null,
        ServerReceiver serverReceiver = null, ClientReceiver clientReceiver = null,
        params object[] tp)
    {
        var types = new TypeCode[tp.Length];
        for(int i=0; i<types.Length; i++)
        {
            TypeCode tpc = Type.GetTypeCode(tp[i].GetType());
            if(tpc != TypeCode.String && tpc != TypeCode.Int32  && tpc != TypeCode.Boolean && tpc != TypeCode.Single)
            {
                Debug.LogError("Type not supported! " + "Adding protocol " + name);
                return;
            }
            types[i] = tpc;
        }
        protocols.Add(name, new ProtocolEntry(name, serverReceiver, clientReceiver, serverSender, clientSender, types));
        netObject.AddListener(name, Callback);
    }
    
    public void Callback(ProtocolBase protocol)
    {
        ProtocolBytes v = protocol as ProtocolBytes;
        int start = 0;
        string protoName = v.GetString(start, ref start);
        string oppositePlayerID = v.GetString(start, ref start);
        TypeCode[] types = protocols[protoName].types;
        object[] info = new object[types.Length];
        for(int i=0; i<types.Length; i++)
        {
            switch(types[i])
            {
                case TypeCode.Int32 : info[i] = v.GetInt(start, ref start); break;
                case TypeCode.String : info[i] = v.GetString(start, ref start); break;
                case TypeCode.Single : info[i] = v.GetFloat(start, ref start); break;
                case TypeCode.Boolean : info[i] = v.GetBool(start, ref start); break;
                default:
                    Debug.LogError("Unknown type in protocol data!");
                break;
            }
        }
        if(Client.IsRoomServer()) protocols[protoName].serverReceiver(info);
        if(!Client.IsRoomServer() && Client.IsNamedServer(oppositePlayerID)) protocols[protoName].clientReceiver(info);
    }
    
    public void Send(string protoName)
    {
        bool invoked = false;
        foreach(var i in protocols)
        {
            if(i.Key == protoName)
            {
                if(Client.IsRoomServer()) ProtoSend(protoName, i.Value.serverSender());
                else ProtoSend(protoName, i.Value.clientSender());
                invoked = true;
            }
        }
        if(!invoked)
        {
            Debug.LogError("Protocol not found! " + protoName + " in " + this.gameObject.name);
        }
    }
    
}