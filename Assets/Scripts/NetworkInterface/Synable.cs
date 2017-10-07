using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Synable : MonoBehaviour{

    protected NetObject NetObject;

    public Synable()
    {
        NetObject = GetComponent<NetObject>();

        if (NetObject == null)
        {
            throw new Exception("No NetObject");
        }
    }

    public virtual byte[] Encode()
    {
        return new byte[] { };
    }

    // 解码
    public virtual ProtocolBytes Decode(ProtocolBytes proto)
    {
        return new ProtocolBytes();
    }

}
