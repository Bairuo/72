using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class NetSceneController : MonoBehaviour {
    public Text IPInput;
    public Text ipInformation;
    public Text cInformation;
    public Text sInformation;
    public GameObject pStart;
    public GameObject pConnect;
    public GameObject pStartServer;
    bool isPrepare = false;
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
                else
                {
                    sInformation.text = "服务器IP：" + Network.player.ipAddress + "\n" + "准备玩家：" + Client.instance.prepareNum + "/" + Client.instance.roomnum + " 全部准备或人数达到4时开始游戏！";
                    //sInformation.text = "准备玩家：" + Client.instance.prepareNum + "/" + Client.instance.roomnum + " 全部准备或人数达到4时开始游戏！";
                }

                //else if (Client.instance.roomnum == 1) sInformation.text = "服务器IP:" + Network.player.ipAddress + ", 等待玩家接入";
                //else if (Client.instance.roomnum == 2) sInformation.text = "服务器IP:" + Network.player.ipAddress + ", 已接入玩家：1";
                //else if (Client.instance.roomnum == 3) sInformation.text = "服务器IP:" + Network.player.ipAddress + ", 已接入玩家：2, 即将开始游戏";
            }
            else
            {
                if (Client.instance.roomnum == 0) cInformation.text = "连接服务器失败";
                else if (Client.instance.roomnum == 1) cInformation.text = "连接服务器失败";
                else
                {
                    cInformation.text = "准备玩家：" + Client.instance.prepareNum + "/" + Client.instance.roomnum + " 全部准备或人数达到4时开始游戏！";
                }

                //else if (Client.instance.roomnum == 2) cInformation.text = "成功连接, 当前玩家:2";
                //else if (Client.instance.roomnum == 3) cInformation.text = "成功连接, 当前玩家:3, 即将开始游戏";
            }
            
            
        }
    }

    public void EnterGame()
    {
        //Application.LoadLevel("MainGame");
        Application.LoadLevel("NetInterfaceEx");
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

    public void SendPrepare()
    {
        if (Client.IsUse() && !isPrepare)
        {
            isPrepare = true;
            Client.instance.SendPrepare();
        }
    }

    public void StartConnect()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            SendPrepare();
            return;
        }

        Client client = new Client();

        client.Connect(IPInput.text, port);

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

    // Find Bug
    public void OnReturnClick()
    {
        DisConnect();
        pStart.SetActive(true);
        pConnect.SetActive(false);
        pStartServer.SetActive(false);
    }

    public void OnStartServerClick()
    {
        StartServer();
        pStart.SetActive(false);
        pConnect.SetActive(false);
        pStartServer.SetActive(true);
    }

}
