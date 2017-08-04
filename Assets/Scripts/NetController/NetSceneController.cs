using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class NetSceneController : MonoBehaviour {
    public Text IPInput; 
    public Text cInformation;
    public Text sInformation;
    public GameObject pStart;
    public GameObject pConnect;
    public GameObject pStartServer;
    int port = 9990;

    public static NetSceneController instance;

    public NetSceneController()
    {
        instance = this;
    }

    void Start()
    {
        pStart.SetActive(true);
        pConnect.SetActive(false);
        pStartServer.SetActive(false);
    }

    void Update()
    {
        if (Client.IsUse())
        {
            Client.instance.Update();

            if (ServerNet.IsUse())
            {
                if (Client.instance.roomnum == 0) sInformation.text = "创建服务器失败";
                else if (Client.instance.roomnum == 1) sInformation.text = "服务器IP:" + Network.player.ipAddress + ", 等待玩家接入";
                else if (Client.instance.roomnum == 2) sInformation.text = "服务器IP:" + Network.player.ipAddress + ", 已接入玩家：1";
                else if (Client.instance.roomnum == 3) sInformation.text = "服务器IP:" + Network.player.ipAddress + ", 已接入玩家：2, 即将开始游戏";
            }
            else
            {
                if (Client.instance.roomnum == 0) cInformation.text = "连接服务器失败";
                else if (Client.instance.roomnum == 1) cInformation.text = "房间人数错误！";
                else if (Client.instance.roomnum == 2) cInformation.text = "成功连接, 当前玩家:2";
                else if (Client.instance.roomnum == 3) cInformation.text = "成功连接, 当前玩家:3, 即将开始游戏";
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

        sInformation.text = "服务器IP： " + Network.player.ipAddress;
    }

    public void StartConnect()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return;
        }

        Client client = new Client();

        /*
        if (client.Connect(IPInput.text, port))
        {
            cInformation.text = "成功连接服务器";
        }
        else
        {
            cInformation.text = "加入服务器失败";
        }*/

    }

    public void DisConnect()
    {
        if (ServerNet.IsUse())
            ServerNet.instance.Close();
        if (Client.IsUse())
            Client.instance.Close();
    }

    public void OnStartClick()
    {
        pStart.SetActive(false);
        pConnect.SetActive(true);
        pStartServer.SetActive(false);
    }

    public void OnReturnClick()
    {
        pStart.SetActive(true);
        pConnect.SetActive(false);
        pStartServer.SetActive(false);
        DisConnect();
    }

    public void OnStartServerClick()
    {
        StartServer();
        pStart.SetActive(false);
        pConnect.SetActive(false);
        pStartServer.SetActive(true);
    }

}
