using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class SynInt : Synable {
    public int Date
    {
       get
       {
           return _Date;
       }
       set
       {
           _Date = value;
           if (NetObject.Protocol != null)
           {
               NetObject.AddProtocolInt(this, _Date);
           }
       }
    }
    int _Date;

    public SynInt(int date)
    {
        Date = date;
    }

    public override byte[] Encode()
    {
        return BitConverter.GetBytes(Date);
    }

    public override ProtocolBytes Decode(ProtocolBytes proto)
    {
        int start = 0;
        _Date = proto.GetInt(start, ref start);
        return proto.GetRestProtocol(start);
    }
}
