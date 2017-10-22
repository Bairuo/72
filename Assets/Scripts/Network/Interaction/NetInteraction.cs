using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetInteraction{
    const int port = 9970;
    const string remoteIP = "119.23.52.136";
    public enum NetServerMode{
        None,
        LANServer,
        WANRommer,
    }
    public static NetServerMode netServerMode = NetServerMode.None;

    static NetInteraction()
    {
        new ServerNet();
        new Client();
    }

    public static string GetServerMsg()
    {
        switch (netServerMode)
        {
            case NetServerMode.None:
                return "";
            case NetServerMode.LANServer:
                return "服务器IP：" + Network.player.ipAddress;
            case NetServerMode.WANRommer:
                return "房间号：" + Client.instance.roomid;
            default:
                return "";
        }
    }

    public static bool StartLANServer()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return false;
        }

        ServerNet.instance.Start(Network.player.ipAddress, port);

        if (Client.instance.Connect(Network.player.ipAddress, port))
        {
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
            netServerMode = NetServerMode.WANRommer;
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

        netServerMode = NetServerMode.None;
        return ConnectWAN();
    }

    public static bool ConnectWANRadom()
    {
        Client.instance.questroom = "-2";

        netServerMode = NetServerMode.None;
        return ConnectWAN();
    }

    public static bool ConnectWAN()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return false;
        }
        return Client.instance.Connect(remoteIP, port);
    }

    public static bool ConnectLANServer(string ip, int port)
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return false;
        }

        Client client = new Client();

        netServerMode = NetServerMode.None;
        return client.Connect(ip, port);
    }


}
