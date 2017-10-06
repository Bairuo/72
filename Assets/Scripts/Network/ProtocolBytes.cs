using System;
using System.Collections;
using System.Linq;

public class ProtocolBytes : ProtocolBase {
    //消息长度，消息内容
    //协议名称长度，协议名称，协议内容
    public byte[] Bytes;

    public override ProtocolBase Decode(byte[] readbuff, int start, int length)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.Bytes = new byte[length];
        Array.Copy(readbuff, start, protocol.Bytes, 0, length);
        return protocol;
    }

    public override byte[] Encode()
    {
        return Bytes;
    }
    
    public override string GetName()
    {
        return GetString(0);
    }
    public override string GetDesc()
    {
        string str = "";
        if (Bytes == null) return str;
        for (int i = 0; i < Bytes.Length; i++)
        {
            int b = (int)Bytes[i];
            str += b.ToString() + " ";
        }
        return str;
    }

    public void AddString(string str)
    {
        Int32 len = str.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
        
        if (Bytes == null)
            Bytes = lenBytes.Concat(strBytes).ToArray();
        else
            Bytes = Bytes.Concat(lenBytes).Concat(strBytes).ToArray();
    }
    public string GetString(int start, ref int end)
    {
        if (Bytes == null)
            return "";
        if (Bytes.Length < start + sizeof(Int32))
            return "";
        Int32 strLen = BitConverter.ToInt32(Bytes, start);
        if (Bytes.Length < start + sizeof(Int32) + strLen)
            return "";

        string str = System.Text.Encoding.UTF8.GetString(Bytes, start + sizeof(Int32), strLen);
        end = start + sizeof(Int32) + strLen;
        return str;
    }
    public string GetString(int start)
    {
        int end = 0;
        return GetString(start, ref end);
    }

    public void AddInt(int num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (Bytes == null)
            Bytes = numBytes;
        else
            Bytes = Bytes.Concat(numBytes).ToArray();
    }
    public void AddBool(bool num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (Bytes == null)
            Bytes = numBytes;
        else
            Bytes = Bytes.Concat(numBytes).ToArray();
    }

    public void AddByte(byte[] bytes)
    {
        if (Bytes == null)
            Bytes = bytes;
        else
            Bytes = Bytes.Concat(bytes).ToArray();
    }

    public ProtocolBytes GetRestProtocol(int start)
    {
        byte[] rest;
        int newLength = Bytes.Length - start;
        rest = new byte[newLength];

        for (int i = 0; i < newLength; i++)
        {
            rest[i] = Bytes[i + start];
        }

        ProtocolBytes proto = new ProtocolBytes();
        proto.AddByte(rest);
        return proto;

    }
    
    public int GetInt(int start, ref int end)
    {
        if (Bytes == null)
            return 0;
        if (Bytes.Length < start + sizeof(Int32))
            return 0;
        end = start + sizeof(Int32);
        return BitConverter.ToInt32(Bytes, start);
    }
    public bool GetBool(int start, ref int end)
    {
        if (Bytes == null)
            return false;
        if (Bytes.Length < start + sizeof(bool))
            return false;
        end = start + sizeof(bool);
        return BitConverter.ToBoolean(Bytes, start);
    }
    public int GetInt(int start)
    {
        int end = 0;
        return GetInt(start, ref end);
    }
    public void AddFloat(float num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (Bytes == null)
            Bytes = numBytes;
        else
            Bytes = Bytes.Concat(numBytes).ToArray();
    }
    public float Getfloat(int start, ref int end)
    {
        if (Bytes == null)
            return 0;
        if (Bytes.Length < start + sizeof(float))
            return 0;
        end = start + sizeof(float);
        return BitConverter.ToSingle(Bytes, start);
    }
    public float Getfloat(int start)
    {
        int end = 0;
        return Getfloat(start, ref end);
    }
}
