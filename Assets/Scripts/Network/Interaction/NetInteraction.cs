using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetInteraction{
    const int port = 9970;
    const string remoteIP = "119.23.52.136";
    public enum NetServerMode
    {
        None,
        LANServer,
        WANRommer,
        WANRandom
    }
    public enum NetPlayerMode
    {
        None,
        LANPlayer,
        WANPlayer,
        WANRandom
    }
    public static NetServerMode netServerMode = NetServerMode.None;
    public static NetPlayerMode netPlayerMode = NetPlayerMode.None;

    static NetInteraction()
    {
        new ServerNet();
        new Client();
    }

    public static bool IsStartServer()
    {
        return !(netServerMode == NetServerMode.None);
    }

    public static bool IsWANRandom()
    {
        return netPlayerMode == NetPlayerMode.WANRandom;
    }

    public static string GetServerInfoMsg()
    {
        switch (netServerMode)
        {
            case NetServerMode.None:
                return "";
            case NetServerMode.LANServer:
                return "服务器IP：" + Network.player.ipAddress;
            case NetServerMode.WANRommer:
                return "房间号：" + Client.instance.roomid;
            case NetServerMode.WANRandom:
                return "成功加入房间";
            default:
                return "";
        }
    }

    //public static string GetPlayerPromptMsg()
    //{
    //    switch (netPlayerMode)
    //    {
    //        case NetPlayerMode.None:
    //            return "";
    //        case NetPlayerMode.LANPlayer:
    //            return "请输入服务器IP：";
    //        case NetPlayerMode.WANPlayer:
    //            return "请输入房间号：";
    //        case NetPlayerMode.WANPRandom:
    //            return "";
    //        default:
    //            return "";
    //    }
    //}

    public static bool StartLANServer()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return false;
        }

        ServerNet.instance.Start(Network.player.ipAddress, port);

        if (Client.instance.Connect(Network.player.ipAddress, port))
        {
            netPlayerMode = NetPlayerMode.None;
            netServerMode = NetServerMode.LANServer;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool StartWANRoom()
    {
        Client.instance.questroom = "-1";

        if (ConnectWAN())
        {
            netPlayerMode = NetPlayerMode.None;
            netServerMode = NetServerMode.WANRommer;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool ConnectWANRadom()
    {
        Client.instance.questroom = "-2";

        netPlayerMode = NetPlayerMode.WANRandom;
        if (ConnectWAN())
        {

            netServerMode = NetServerMode.WANRandom;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool ConnectWANRoom(string roomID)
    {
        Client.instance.questroom = roomID;

        netPlayerMode = NetPlayerMode.WANPlayer;
        if (ConnectWAN())
        {
            
            netServerMode = NetServerMode.None;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool ConnectLANServer(string ip, int port)
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return false;
        }

        Client client = new Client();

        netPlayerMode = NetPlayerMode.LANPlayer;
        if (Client.instance.Connect(ip, port))
        {
            
            netServerMode = NetServerMode.None;
            return true;
        }
        else
        {
            return false;
        }
    }



    static bool ConnectWAN()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return false;
        }
        return Client.instance.Connect(remoteIP, port);
    }




}
