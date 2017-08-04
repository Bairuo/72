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

        if (ServerNet.IsUse() && num == 1) NetSceneController.instance.EnterGame();

        if (num == 2)
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

    public void PortalDestroy(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        int PortalID = proto.GetInt(start, ref start);

        PortalController.DestroyBoth(PortalID);

    }
    public void PortalCreate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        int PortalID = proto.GetInt(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);

        UnityEngine.Vector2 pos = new UnityEngine.Vector2(x, y);
        PortalController.SetAnother(PortalID, pos);

    }
    public void Fail(ProtocolBase protoBase)
    {

    }
    public void PlayerDestroy(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);

        Client.instance.posmanager.PlayerDestroy(id);
    }
    public void PlayerGenerate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);

        GenerateController.instance.CreatePlayer(x, y, id);
    }
    public void PlayerClick(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);
        float z = proto.Getfloat(start, ref start);

        Client.instance.posmanager.PlayerClick(id, x, y, z);
    }
    public void ChangePosition(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);
        float z = proto.Getfloat(start, ref start);

        Client.instance.posmanager.ChangePosition(id, x, y, z);
    }
    public void PropGenerate(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string tag = proto.GetString(start, ref start);
        int id = proto.GetInt(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);

        Client.instance.posmanager.PropGenerate(tag, id, new UnityEngine.Vector2(x, y));
    }

    public void ChangeMassLevel(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int masslevel = proto.GetInt(start, ref start);

        Client.instance.posmanager.ChangeMassLevel(id, masslevel);
    }
    public void ChangeSpeedLevel(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int speedlevel = proto.GetInt(start, ref start);

        Client.instance.posmanager.ChangeSpeed(id, speedlevel);
    }
    public void ChangeMass(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float mass = proto.Getfloat(start, ref start);

        Client.instance.posmanager.ChangeMass(id, mass);
    }
    public void ChangeBrake(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float brake = proto.Getfloat(start, ref start);

        Client.instance.posmanager.ChangeSpeed(id, brake);
    }
    public void ChangeSpeed(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float speed = proto.Getfloat(start, ref start);

        Client.instance.posmanager.ChangeSpeed(id, speed);
    }
    public void ChangeHealth(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float health = proto.Getfloat(start, ref start);

        Client.instance.posmanager.ChangeHealth(id, health);
    }
    public void ChangeStatus(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string name = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int status = proto.GetInt(start, ref start);

        Client.instance.posmanager.ChangeStatus(id, status);
    }

    public void PlayerTurn(ProtocolBase protoBase)
    {
        //Client.instance.posmanager.PlayerTurn(protoBase);
    }
    public void Shoot(ProtocolBase protoBase)
    {
        //Client.instance.posmanager.Shoot(protoBase);
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
