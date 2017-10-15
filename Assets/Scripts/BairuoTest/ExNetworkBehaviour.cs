using System;
using UnityEngine;

public class ExNetworkBehaviour : MonoBehaviour
{
    NetObject NetObject;
    
    public virtual string protocolName { get; }
    
	protected virtual void Start()
    {
        NetObject = GetComponent<NetObject>();
        NetObject.AddListener(protocolName, NetCallback);
	}

    public void Send(params object[] info)
    {
        ProtocolBytes proto = NetObject.GetObjectProtocol();
        proto.AddName(protocolName);
        
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
        NetObject.Send(proto);
    }
    
    ProtocolBytes proto;
    int start;
    
    protected string GetString() { return proto.GetString(start, ref start); }
    protected float GetFloat() { return proto.GetFloat(start, ref start); }
    protected int GetInt() { return proto.GetInt(start, ref start); }
    protected bool GetBool() { return proto.GetBool(start, ref start); }
    protected Vector2 GetVec2() { return new Vector2(GetFloat(), GetFloat()); }
    protected Vector3 GetVec3() { return new Vector3(GetFloat(), GetFloat(), GetFloat()); }
    protected Quaternion GetQuat() { return new Quaternion(GetFloat(), GetFloat(), GetFloat(), GetFloat()); }
    
    
    void NetCallback(ProtocolBase protocol)
    {
        proto = (ProtocolBytes)protocol;
        start = 0;
        
        string name = proto.GetString(start, ref start);
        
        Recieve();
    }
    
    protected virtual void Recieve()
    {
        // do nothing...
    }
    
}