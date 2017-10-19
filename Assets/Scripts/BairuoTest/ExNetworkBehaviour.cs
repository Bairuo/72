using System;
using System.Collections.Generic;
using UnityEngine;


/// Create as a single message receive unit.
/// Use netObject.NetID to identify.
public class ExNetworkBehaviour : MonoBehaviour
{
    public string initialID;
    
    public NetObject netObject = new NetObject();
    
    public bool prepared = false;
    
    static Dictionary<string, ExNetworkBehaviour> networkScripts = new Dictionary<string, ExNetworkBehaviour>();
    
    protected virtual void Awake()
    {
        if(initialID != null && initialID != "") netObject.NetID = initialID.ToString();
    }
    
    protected virtual void Start()
    {    
        Debug.LogFormat("ExNetworkBehaviour execute Start\nobject {0} component {1} netID {2}.", this.gameObject.name, this, netObject.NetID);
        
        /// This protocol is to destroy the object in *clients*.
        /// The Destroy() *should* be called at the server.
        AddProtocol("DefaultDestroying", DestroyingSend, null, null, DestroyingReceive);
        
        networkScripts.Add(netObject.NetID, this);
        prepared = true;
    }
    
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
        public Type[] types;
        public ProtocolEntry(
            string n = "DEFAULT",
            ServerReceiver sr = null,
            ClientReceiver cr = null,
            ServerSender ss = null,
            ClientSender cs = null,
            Type[] tp = null)
        {
            protocolName = n;
            serverReceiver = sr;
            clientReceiver = cr;
            serverSender = ss;
            clientSender = cs;
            types = tp;
        }
        public override string ToString()
        {
            return "" + protocolName + " | " + serverReceiver + " | " + clientReceiver + " | " + serverSender + " | " + clientSender;
        }
    }
    
    
    // ======================================================================
    //                             Send Section
    // ======================================================================
    
    public void Send(string protoName)
    {
        bool invoked = false;
        foreach(var i in protocols)
        {
            if(i.Key == protoName)
            {
                if(Client.IsRoomServer())
                {
                    if(i.Value.serverSender != null) ProtoSend(protoName, i.Value.serverSender());
                }
                else
                {
                    if(i.Value.clientSender != null) ProtoSend(protoName, i.Value.clientSender());
                }
                invoked = true;
            }
        }
        if(!invoked)
        {
            Debug.LogError("Protocol not found! " + protoName + " in " + this.gameObject.name);
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
            if(info[i].GetType() != protocols[protocolName].types[i])
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
                Vector3 v = (Vector3)i;
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
            else if(i is Color)
            {
                Color v = (Color)i;
                proto.AddFloat(v.r);
                proto.AddFloat(v.g);
                proto.AddFloat(v.b);
                proto.AddFloat(v.a);
            }
            else Debug.LogError("Try to send a variable that is not supported.");
        }
        
        netObject.Send(proto);
    }
    
    // ======================================================================
    //                           Callback Section
    // ======================================================================
    
    public void Callback(ProtocolBase protocol)
    {
        ProtocolBytes v = protocol as ProtocolBytes;
        int start = 0;
        string protoName = v.GetString(start, ref start);
        string oppositePlayerID = v.GetString(start, ref start);
        Type[] types = protocols[protoName].types;
        object[] info = new object[types.Length];
        for(int i=0; i<types.Length; i++)
        {
            if(types[i] == typeof(int)) { info[i] = v.GetInt(start, ref start); continue; }
            if(types[i] == typeof(float)) { info[i] = v.GetFloat(start, ref start); continue; }
            if(types[i] == typeof(string)) { info[i] = v.GetString(start, ref start); continue; }
            if(types[i] == typeof(bool)) { info[i] = v.GetBool(start, ref start); continue; }
            if(types[i] == typeof(Vector2)) 
            {
                info[i] = new Vector2(
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start));
                continue;
            }
            if(types[i] == typeof(Vector3))
            {
                info[i] = new Vector3(
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start));
                continue;
            }
            if(types[i] == typeof(Color))
            {
                info[i] = new Color(
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start));
                continue;
            }
            if(types[i] == typeof(Quaternion))
            {
                info[i] = new Quaternion(
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start),
                    v.GetFloat(start, ref start));
                continue;
            }
            
            Debug.LogError("Unknown type in protocol data!");
            break;
        }
        if(Client.IsRoomServer())
        {
            protocols[protoName].serverReceiver(info);
        }
        if(!Client.IsRoomServer() && Client.IsNamedServer(oppositePlayerID))
        {
            protocols[protoName].clientReceiver(info);
        }
    }
    
    
    // ======================================================================
    //                        Custom Protocal Section
    // ======================================================================
    
    public void AddProtocol(
        string name,
        ServerSender toClient = null, ClientSender toServer = null,
        ServerReceiver serverReceiver = null, ClientReceiver clientReceiver = null,
        params object[] tp)
    {
        var types = new Type[tp.Length];
        for(int i=0; i<types.Length; i++)
        {
            Type tpc;
            
            if(tp[i] is Type)
            {
                tpc = tp[i] as Type;
            }
            else if(tp[i].GetType() == typeof(string) ||
                tp[i].GetType() == typeof(int) ||
                tp[i].GetType() == typeof(bool) ||
                tp[i].GetType() == typeof(float) ||
                tp[i].GetType() == typeof(Vector2) ||
                tp[i].GetType() == typeof(Vector3) ||
                tp[i].GetType() == typeof(Quaternion) ||
                tp[i].GetType() == typeof(Color))
            {
                tpc = tp[i].GetType();
            }
            else
            {
                Debug.LogError("Type not supported! " + "Adding protocol " + name);
                return;
            }
            types[i] = tpc;
        }
        
        protocols.Add(name, new ProtocolEntry(name, serverReceiver, clientReceiver, toClient, toServer, types));
        netObject.AddListener(name, Callback);
    }
    
    // ======================================================================
    //                        Tools Functions Section
    // ======================================================================
    
    public static ExNetworkBehaviour GetNetworkBehaviour(string id)
    {
        return networkScripts[id];
    }
    
    // ======================================================================
    //                           Clean Up Section
    // ======================================================================
    
    protected virtual object[] DestroyingSend()
    {
        return new object[0];
    }
    
    protected virtual void DestroyingReceive(object[] info)
    {
        Destroy(this.gameObject);
    }
    

    protected virtual void OnDestroy()
    {
        if(Client.IsRoomServer()) Send("DefaultDestroying");
        networkScripts.Remove(netObject.NetID);
        netObject.SelfRemove();
    }
}