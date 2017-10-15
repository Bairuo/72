using System;
using UnityEngine;

public class NetworkBehaviour : MonoBehaviour
{
    NetObject NetObject;
    
    public static int id;
    
    protected virtual string networkName{ get; }
    
	protected virtual void Start()
    {
        NetObject = this.gameObject.GetComponent<NetObject>();
        NetObject.AddListener(networkName, Receive);
    }
    
    public virtual void Send(params object[] info)
    {
        ProtocolBytes proto = NetObject.GetObjectProtocol();
        proto.AddName(networkName);
        
        foreach(var i in info)
        {
            if(i is string)
            {
                proto.AddString((string)i);
            }
            else if(i is byte[])
            {
                proto.AddByte((byte[])i);
            }
            else if(i is Vector2)
            {
                proto.AddFloat(((Vector2)i).x);
                proto.AddFloat(((Vector2)i).y);
            }
            else if(i is Vector3)
            {
                proto.AddFloat(((Vector3)i).x);
                proto.AddFloat(((Vector3)i).y);
                proto.AddFloat(((Vector3)i).z);
            }
            else if(i is Quaternion)
            {
                proto.AddFloat(((Quaternion)i).x);
                proto.AddFloat(((Quaternion)i).y);
                proto.AddFloat(((Quaternion)i).z);
                proto.AddFloat(((Quaternion)i).w);
            }
            else
            {
                switch(Type.GetTypeCode(i.GetType()))
                {
                    case TypeCode.Int32:
                        proto.AddInt((int)i);
                    break;
                    case TypeCode.Single:
                        proto.AddFloat((float)i);
                    break;
                    case TypeCode.Boolean:
                        proto.AddBool((bool)i);
                    break;
                    default:
                        Debug.LogError("param not supported.");
                    break;
                }
            }
        }
        NetObject.Send(proto);
    }
    
    ProtocolBytes proto;
    int start;
    public virtual void Receive(ProtocolBase protocol)
    {
        Debug.LogError("Rec!");
        proto = (ProtocolBytes)protocol;
        start = 0;
        string name = proto.GetString(start, ref start);
        // get value here.
        NetworkCallback();
    }
    
    protected int GetInt(){ return proto.GetInt(start, ref start); }
    protected float GetFloat(){ return proto.GetFloat(start, ref start); }
    protected string GetString(){ return proto.GetString(start, ref start); }
    protected bool GetBool(){ return proto.GetBool(start, ref start); }
    protected Vector2 GetVec2() { return new Vector2(GetFloat(), GetFloat()); }
    protected Vector3 GetVec3() { return new Vector3(GetFloat(), GetFloat(), GetFloat()); }
    protected Quaternion GetQuat() { return new Quaternion(GetFloat(), GetFloat(), GetFloat(), GetFloat()); }
    
    public virtual void NetworkCallback()
    {
        // Do nithing...
    }
}