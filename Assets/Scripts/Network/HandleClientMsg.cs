using System;

public class HandleClientMsg{
    //连接类（信息回馈）
    public void RoomNum(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int num = proto.GetInt(start, ref start);
        Client.instance.roomnum = num;
        if (num == 2 || ServerNet.IsUse())
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
        string protoName = protol.GetString(start, ref start);
        int id = protol.GetInt(start, ref start);
        int roomid = protol.GetInt(start, ref start);
        int conn_id = protol.GetInt(start, ref start);
        Client.instance.playerid = id.ToString();
        Client.instance.roomid = roomid.ToString();
        Client.instance.conn_id = conn_id;

        Client.instance.UDPConnect();
    }
    public void P2P(ProtocolBase protoBase)
    {
        ProtocolBytes protol = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = protol.GetString(start, ref start);
        string ip = protol.GetString(start, ref start);
        int port = protol.GetInt(start, ref start);

        Client.instance.AddP2Premote(ip, port);
    }

    //（针对）战斗类协议
    public void EnterBlock(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string playerID = proto.GetString(start, ref start);
        string blockID = proto.GetString(start, ref start);

        //Client.instance.posmanager.EnterBlock(playerID, blockID);
    }
    public void LeaveBlock(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string blockID = proto.GetString(start, ref start);

        //Client.instance.posmanager.LeaveBlock(blockID);
    }
    public void PlayerInit(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string net_id = proto.GetString(start, ref start);

        //PlayerController.PlayerInit(net_id);
    }
    public void AreaInit(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string tag = proto.GetString(start, ref start);
        int patternID = proto.GetInt(start, ref start);

        //TargetArea.NetAreaInit(tag, patternID);
    }
    public void BlockGenerate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string net_id = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);
        int ID = proto.GetInt(start, ref start);

        //BlockGenerator.instance.Generate(net_id, x, y, ID);
    }

    public void PlayerTurn(ProtocolBase protoBase)
    {
        //Client.instance.posmanager.PlayerTurn(protoBase);
    }
    public void Shoot(ProtocolBase protoBase)
    {
        //Client.instance.posmanager.Shoot(protoBase);
    }
    public void Hit(ProtocolBase protoBase)
    {
        Client.instance.posmanager.Hit(protoBase);
    }

    //(通用）战斗类协议
    /*
    public void StartScene(ProtocolBase protoBase)
    {
        SceneManager.instance.StartScene();
    }
    */
    /*
    public void AddPlayer(ProtocolBase protoBase)
    {
        if (GlobalInformation.instance.state == GlobalInformation.STATE.Curtain || GlobalInformation.instance.state == GlobalInformation.STATE.Fight)
        {
            Client.instance.posmanager.AddPlayer(protoBase);
        }
    }
    public void AddEnemy(ProtocolBase protoBase)
    {
        if (GlobalInformation.instance.state == GlobalInformation.STATE.Curtain || GlobalInformation.instance.state == GlobalInformation.STATE.Fight)
        {
            Client.instance.posmanager.AddEnemy(protoBase);
        }
    }

    //流程类协议
    public void NextLevel(ProtocolBase protoBase)
    {
        Client.instance.posmanager.Close();
        GameController.instance.REnterNextLevel();
    }

    public void Pause(ProtocolBase protoBase)
    {
        if (GlobalInformation.instance.state == GlobalInformation.STATE.Curtain || GlobalInformation.instance.state == GlobalInformation.STATE.Fight)
        {
            GameController.instance.RPause();
        }

    }
    public void CurtainStart(ProtocolBase protoBase)
    {
        if (GlobalInformation.instance.state == GlobalInformation.STATE.Curtain)
        {
            Client.instance.posmanager.StartFight();
            CurtainController.instance.RCurtainStart();
        }

    }
    public void ReStart(ProtocolBase protoBase)
    {
        if (GlobalInformation.instance.state == GlobalInformation.STATE.Curtain || GlobalInformation.instance.state == GlobalInformation.STATE.Fight)
        {
            Client.instance.posmanager.Close();
            GameController.instance.RReStart();
        }

    }
    public void Return(ProtocolBase protoBase)
    {
        if (GlobalInformation.instance.state == GlobalInformation.STATE.Curtain || GlobalInformation.instance.state == GlobalInformation.STATE.Fight)
        {
            Client.instance.posmanager.Close();
            GameController.instance.RReturn();
        }

    }
     * */
}
