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
           return Date;
       }
       set
       {
           Date = value;
           if (NetObject.Protocol != null)
           {
               NetObject.Protocol.AddInt(Date);
           }
       }
    }

    public SynInt(int date)
    {
        Date = date;
    }

    public override byte[] Encode()
    {
        return BitConverter.GetBytes(Date);
    }
}
