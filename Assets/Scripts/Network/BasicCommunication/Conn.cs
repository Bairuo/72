using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

public class Conn{

    public const int BUFFER_SIZE = 1024;
    public Socket socket;
    public EndPoint UDPRemote = null;
    public bool isUse = false;

    public byte[] readBuff = new byte[BUFFER_SIZE];
    public byte[] UDPreadBuff = new byte[BUFFER_SIZE];
    public int buffCount = 0;
    public byte[] lenBytes = new byte[sizeof(UInt32)];
    public Int32 msgLength = 0;

    // 心跳时间
    public long lastTickTime = long.MinValue;
    //对应的Player
    public int id = 0;
    public int idinroom = 0;
    public int roomid = -1;

    public Conn()
    {
        readBuff = new byte[BUFFER_SIZE];
        UDPreadBuff = new byte[BUFFER_SIZE];
    }

    public void Init(Socket socket)
    {
        this.socket = socket;
        isUse = true;
        buffCount = 0;
        //心跳处理
        lastTickTime = Sys.GetTimeStamp();
    }

    public int BuffRemain()
    {
        return BUFFER_SIZE - buffCount;
    }

    public string GetAdress()
    {
        if (!isUse)
            return "无法获取地址";
        //return socket.RemoteEndPoint.ToString();
        return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
    }

    public void Close()
    {
        if (!isUse)
            return;
        //玩家退出处理
        UDPRemote = null;
        socket.Shutdown(SocketShutdown.Both);

        socket.Close();
        isUse = false;
    }

}
