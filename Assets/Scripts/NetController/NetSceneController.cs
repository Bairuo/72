using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class NetSceneController : MonoBehaviour {
    public Text IPInput; 
    public Text Information;
    int port = 9990;

    public static NetSceneController instance;

    public NetSceneController()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if (Client.IsUse())
        {
            Client.instance.Update();

            if (ServerNet.IsUse())
            {
                if (Client.instance.roomnum == 0) Information.text = "创建服务器失败";
                else if (Client.instance.roomnum == 1) Information.text = "成功创建服务器, 等待玩家接入";
                else if (Client.instance.roomnum == 2) Information.text = "已接入玩家：1";
                else if (Client.instance.roomnum == 3) Information.text = "已接入玩家：2, 即将开始游戏";
            }
            else
            {
                if (Client.instance.roomnum == 0) Information.text = "连接服务器失败";
                else if (Client.instance.roomnum == 1) Information.text = "房间人数错误！";
                else if (Client.instance.roomnum == 2) Information.text = "成功连接, 当前玩家:2";
                else if (Client.instance.roomnum == 3) Information.text = "成功连接, 当前玩家:3, 即将开始游戏";
            }
        }
    }

    public void ClickGame()
    {
        //EnterGame();
    }

    public void EnterGame()
    {
        Application.LoadLevel("BairuoTest");
    }

    public void StartServer()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return;
        }

        ServerNet server = new ServerNet();
        Client client = new Client();

        ServerNet.instance.Start(Network.player.ipAddress, port);
        Client.instance.Connect(Network.player.ipAddress, port);

        Information.text = "服务器IP： " + Network.player.ipAddress;
    }

    public void StartConnect()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return;
        }

        Client client = new Client();

        if (client.Connect(IPInput.text, port))
        {
            Information.text = "成功连接服务器";
        }
        else
        {
            Information.text = "加入服务器失败";
        }

    }

    public void DisConnect()
    {

    }
}
