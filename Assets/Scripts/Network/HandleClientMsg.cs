using System;

public class HandleClientMsg{
    // 流程、连接相关
    public void StartGame(ProtocolBase protoBase)
    {
        NetSceneController.instance.EnterGame();
    }
    public void PrepareNum(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        proto.GetString(start, ref start);
        int num = proto.GetInt(start, ref start);
        Client.instance.prepareNum = num;
    }
    public void RoomNum(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        proto.GetString(start, ref start);
        int num = proto.GetInt(start, ref start);
        Client.instance.roomnum = num;

        //if (ServerNet.IsUse() && num == 1) NetSceneController.instance.EnterGame();

        if (num == 4)
        {
            NetSceneController.instance.EnterGame();
        }

    }
    public void Success(ProtocolBase protoBase)
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("AddRoom");
        proto.AddString(Client.instance.questroom);
        Client.instance.Send(proto);
    }
    public void ID(ProtocolBase protoBase)
    {
        ProtocolBytes protol = (ProtocolBytes)protoBase;
        int start = 0;
        protol.GetString(start, ref start);
        int id = protol.GetInt(start, ref start);
        int roomid = protol.GetInt(start, ref start);
        int conn_id = protol.GetInt(start, ref start);
        Client.instance.playerid = id.ToString();
        Client.instance.
            roomid = roomid.ToString();
        Client.instance.conn_id = conn_id;

        Client.instance.UDPConnect();
    }
    public void P2P(ProtocolBase protoBase)
    {
        ProtocolBytes protol = (ProtocolBytes)protoBase;
        int start = 0;
        protol.GetString(start, ref start);
        string ip = protol.GetString(start, ref start);
        int port = protol.GetInt(start, ref start);

        Client.instance.AddP2Premote(ip, port);
    }

    //（针对）战斗类协议

    public void PortalDestroy(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        proto.GetString(start, ref start);
        int PortalID = proto.GetInt(start, ref start);

        PortalController.DestroyBoth(PortalID);

    }
    public void PortalCreate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        proto.GetString(start, ref start);
        int PortalID = proto.GetInt(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);

        UnityEngine.Vector2 pos = new UnityEngine.Vector2(x, y);
        PortalController.SetAnother(PortalID, pos);

    }

    // 改变属性
    public void ChangePosition(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);
        float z = proto.GetFloat(start, ref start);

        Client.instance.posmanager.ChangePosition(id, x, y, z);
    }

}
