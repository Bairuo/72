using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;

public class Client
{
    Socket socket;
    Socket UDPsocket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    public byte[] UDPreadBuff = new byte[BUFFER_SIZE];

    int buffCount = 0;

    public bool isUse = false;
    public string ipAdress;

    byte[] lenBytes = new byte[sizeof(UInt32)];
    byte[] UDPlenBytes = new byte[sizeof(UInt32)];
    IPEndPoint ip;
    Int32 msgLength = 0;

    ProtocolBase proto = new ProtocolBytes();

    public float lastTickTime = 0;
    public float heartBeatTime = 30;

    public static Client instance;

    public HandleClientMsg handleClientMsg = new HandleClientMsg();

    //MsgDistribution
    public int num = 15; //每帧处理消息的内容
    public List<ProtocolBase> msgList = new List<ProtocolBase>();
    public delegate void Delegate(ProtocolBase proto);

    private Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();
    private Dictionary<string, Delegate> onceDict = new Dictionary<string, Delegate>();
    
    //位置管理相关
    //服务器返回的标志信息
    public PosManager posmanager = new PosManager();
    public string playerid;
    public string roomid = "未知";
    public string questroom = "-1";
    public int prepareNum = 0;
    public int roomnum = 0;
    public int conn_id = -1;
    string Host;

    // 超时设置
    bool IsConnect = false;
    const int timeoutMSec = 1500;
    ManualResetEvent TimeoutObjct = new ManualResetEvent(false);

    // UDP P2P
    public List<IPEndPoint> P2Premote = new List<IPEndPoint>();

    public Client()
    {
        instance = this;
    }

    public static bool IsUse()
    {
        if (instance == null)
        {
            return false;
        }
        else
        {
            return instance.isUse;
        }
    }
    public static bool IsRoomServer()
    {
        if (!IsUse())
        {
            return false;
        }
        else
        {
            return instance.playerid == "0";
        }
    }
    
    public bool Connect(string host, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            UDPsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Host = host;
            //ip = new IPEndPoint(IPAddress.Any, port - 2);
            //ip = new IPEndPoint(IPAddress.Any, port);

            //socket.Connect(host, port);
            IsConnect = false;
            TimeoutObjct.Reset();

            socket.BeginConnect(host, port, ConnectCb, socket);
            
            if (TimeoutObjct.WaitOne(timeoutMSec, false))
            {
                if (IsConnect)
                {

                }
                else
                {
                    //Debug.Log(1);
                    Close();
                    return false;
                }
            }
            else
            {
                Close();
                return false;
            }
            //Debug.Log(((IPEndPoint)socket.LocalEndPoint).Port);
            //ip = new IPEndPoint(IPAddress.Any, ((IPEndPoint)socket.LocalEndPoint).Port);
            //UDPsocket.Bind(ip);

            isUse = true;
            ipAdress = host;

            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveCb, readBuff);
            UDPsocket.BeginReceive(UDPreadBuff, 0, BUFFER_SIZE, SocketFlags.None, UDPReceiveCb, UDPreadBuff);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(isUse);
            Debug.Log(e.Message);
            return false;
        }
    }

    private void ConnectCb(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndConnect(ar);

            IsConnect = true;
        }
        catch (Exception e)
        {
            IsConnect = false;
            Debug.Log(e.Message);
        }
        finally
        {
            TimeoutObjct.Set();
        }
    }
    private void UDPReceiveCb(IAsyncResult ar)
    {
        int count = UDPsocket.EndReceive(ar);
        Array.Copy(UDPreadBuff, UDPlenBytes, sizeof(Int32));
        int msgLength = BitConverter.ToInt32(UDPlenBytes, 0);
        
        ProtocolBase protocol = proto.Decode(UDPreadBuff, sizeof(Int32), msgLength);
        //Debug.Log(protocol.GetName());

        lock (msgList)
        {
            msgList.Add(protocol);
        }

        Array.Copy(UDPreadBuff, sizeof(Int32) + msgLength, UDPreadBuff, 0, count);

        UDPsocket.BeginReceive(UDPreadBuff, 0, BUFFER_SIZE, SocketFlags.None, UDPReceiveCb, UDPreadBuff);
    }
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount += count;
            
            ProcessData();
            
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveCb, readBuff);
        }
        catch (Exception e)
        {
            //socket.Close();
            return;
        }
    }
    private void ProcessData()
    {
        /******是否开始处理******/
        if (buffCount < sizeof(Int32))
            return;
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);

        if (buffCount < msgLength + sizeof(Int32))
            return;

        /******处理消息******/
        ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);

        lock (msgList)
        {
            msgList.Add(protocol);
        }


        /******去掉已经处理的消息******/
        int count = buffCount - msgLength - sizeof(Int32);
        Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
        {
            ProcessData();
        }

    }

    public void UDPSend(ProtocolBase protocol)
    {
        byte[] bytes = protocol.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Host), 9990);
        UDPsocket.SendTo(sendbuff, ip);
    }
    public void UDPSend(ProtocolBase protocol, IPEndPoint remote)
    {
        byte[] bytes = protocol.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        UDPsocket.SendTo(sendbuff, remote);
    }
    public void UDPP2PBroadcast(ProtocolBase protocol)
    {
        foreach (var item in P2Premote)
        {
            UDPSend(protocol, item);
        }
    }

    public void Send(ProtocolBase protocol)
    {
        byte[] bytes = protocol.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        socket.Send(sendbuff);
    }
    public void Send(ProtocolBase protocol, string cbName, Delegate cb)
    {
        AddOnceListener(cbName, cb);
        Send(protocol);
    }
    public void Send(ProtocolBase protocol, Delegate cb)
    {
        string cbName = protocol.GetName();
        Send(protocol, cbName, cb);
    }

    //内置协议
    public void SendDisConnect()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("DisConnect");
        Send(proto);
    }
    public void UDPConnect()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddInt(conn_id);
        proto.AddString("A");
        UDPSend(proto);
    }
    public void AddP2Premote(string ip, int port)
    {
        IPEndPoint t = new IPEndPoint(IPAddress.Parse(ip), port);
        P2Premote.Add(t);
        //Debug.Log(ip + ":" + port);
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("T");

        // Make a hole
        UDPSend(proto, t);
    }

    //针对性协议
    /*
    public void BlockGenerate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);
        int ID = proto.GetInt(start, ref start);
        
        if(BlockGenerator.instance != null) BlockGenerator.instance.Generate(x, y, ID);
        else
        {
            lock (msgList)
            {
                msgList.Add(protoBase);
            }
        }
    }*/
    public void SendPortalDestroy(int PortalID)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("PortalDestroy");
        protocol.AddInt(PortalID);

        Send(protocol);
    }
    public void SendPortalCreate(int PortalID, Vector2 pos)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("PortalCreate");
        protocol.AddInt(PortalID);
        protocol.AddFloat(pos.x);
        protocol.AddFloat(pos.y);

        Send(protocol);
    }
    public void SendTaggerGenerate(string tag, Vector2 pos)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("TaggerGenerate");
        protocol.AddString(tag);
        protocol.AddFloat(pos.x);
        protocol.AddFloat(pos.y);

        Send(protocol);
    }
    public void SendPropGenerate(string tag, int id, Vector2 pos)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("PropGenerate");
        protocol.AddString(tag);
        protocol.AddInt(id);
        protocol.AddFloat(pos.x);
        protocol.AddFloat(pos.y);

        Send(protocol);
    }
    public void SendSafyAreaInfo(float radius)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("SafyAreaInfo");
        protocol.AddFloat(radius);

        Send(protocol);
    }
    public void SendPlayerDestroy(string PlayerID)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("PlayerDestroy");
        protocol.AddString(PlayerID);

        Send(protocol);
    }
    public void SendFail()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Fail");

        Send(protocol);
    }
    public void SendPlayerGenerate(string PlayerID, float x, float y)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("PlayerGenerate");
        protocol.AddString(PlayerID);
        protocol.AddFloat(x);
        protocol.AddFloat(y);
 
        Send(protocol);
    }
    public void SendChangePosition(string PlayerID, float x, float y, float z)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangePosition");
        protocol.AddString(PlayerID);
        protocol.AddFloat(x);
        protocol.AddFloat(y);
        protocol.AddFloat(z);

        Send(protocol);
    }
    public void SendPlayerClick(string PlayerID, float x, float y, float z)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        //protocol.AddString("PlayerClick");
        protocol.AddInt(conn_id);
        protocol.AddString("C");
        protocol.AddString(PlayerID);
        protocol.AddFloat(x);
        protocol.AddFloat(y);
        protocol.AddFloat(z);

        UDPSend(protocol);
    }

    public void SendChangeMassLevel(string PlayerID, int masslevel)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangeMassLevel");
        protocol.AddString(PlayerID);
        protocol.AddInt(masslevel);
        Send(protocol);
    }
    public void SendChangeSpeedLevel(string PlayerID, int speedlevel)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangeSpeedLevel");
        protocol.AddString(PlayerID);
        protocol.AddInt(speedlevel);

        Send(protocol);
    }
    public void SendChangeMass(string PlayerID, float mass)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangeMass");
        protocol.AddString(PlayerID);
        protocol.AddFloat(mass);

        Send(protocol);
    }
    public void SendChangeBrake(string PlayerID, float brake)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangeBrake");
        protocol.AddString(PlayerID);
        protocol.AddFloat(brake);

        Send(protocol);
    }
    public void SendChangeSpeed(string PlayerID, float speed)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangeSpeed");
        protocol.AddString(PlayerID);
        protocol.AddFloat(speed);

        Send(protocol);
    }

    public void SendChangeHealth(string PlayerID, float health)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangeHealth");
        protocol.AddString(PlayerID);
        protocol.AddFloat(health);

        Send(protocol);
    }
    public void SendChangeStatus(string PlayerID, int status)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ChangeStatus");
        protocol.AddString(PlayerID);
        protocol.AddInt(status);

        Send(protocol);
    }
    
    // NI
    public void SendPlayerInit()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("PlayerInit");
        protocol.AddString(playerid);

        Send(protocol);
    }

    // 通用
    public void SendStartGame()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartGame");

        Send(protocol);
    }
    public void SendStartScene(int index)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartScene");
        protocol.AddInt(index);

        Send(protocol);
    }
    public void SendNextLevel()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("NextLevel");

        Send(protocol);
    }
    public void SendPrepare()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Prepare");

        Send(protocol);
    }
    public void SendPause()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Pause");
        Client.instance.Send(protocol);
    }
    public void SendReturn()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Return");
        Client.instance.Send(protocol);
    }
    public void SendReStart()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ReStart");
        Client.instance.Send(protocol);
    }
    public void SendCurtainStart()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("CurtainStart");
        Client.instance.Send(protocol);
    }

    // DispatchMsgEvent
    public void Update()
    {
        if (posmanager.isInit)
        {
            posmanager.Update();
        }
        

        //每帧处理消息
        for (int i = 0; i < num; i++)
        {
            if (msgList.Count > 0)
            {
                DispatchMsgEvent(msgList[0]);
                lock (msgList)
                {
                    msgList.RemoveAt(0);
                }
            }
            else
            {
                break;
            }
        }
        //心跳
        if (Time.time - lastTickTime > heartBeatTime)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("HeatBeat");
            Send(protocol);
            lastTickTime = Time.time;
        }
    }

    public void DispatchMsgEvent(ProtocolBase protocol)
    {
        if (protocol == null) return;
        string name = protocol.GetName();
        Debug.Log("Receive Name " + name);
        if (eventDict.ContainsKey(name))
        {
            eventDict[name](protocol);
            return;
        }

        if (onceDict.ContainsKey(name))
        {
            onceDict[name](protocol);
            onceDict[name] = null;
            onceDict.Remove(name);
            return;
        }

        MethodInfo mm = handleClientMsg.GetType().GetMethod(name);
        if (mm != null)
        {
            object[] obj = new object[] { protocol };
            mm.Invoke(handleClientMsg, obj);
        }
    }
    public void AddListener(string name, Delegate cb)
    {
        if (eventDict.ContainsKey(name))
            eventDict[name] += cb;
        else
            eventDict[name] = cb;
    }
    public void AddOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
            onceDict[name] += cb;
        else
            onceDict[name] = cb;
    }
    public void DelListener(string name, Delegate cb)
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name] -= cb;
            if (eventDict[name] == null)
                eventDict.Remove(name);
        }
    }
    public void DelOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
        {
            onceDict[name] -= cb;
            if (onceDict[name] == null)
                onceDict.Remove(name);
        }
    }

    public bool Close()
    {
        try
        {
            Client.instance.SendDisConnect();
            socket.Close();
            UDPsocket.Close();
            isUse = false;
            IsConnect = false;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
