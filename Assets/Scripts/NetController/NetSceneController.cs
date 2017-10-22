using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetSceneController : MonoBehaviour {
    public Text IPInput;
    public Text cInformation;
    public Text sInformation;
    public Text singleWaitInfo;
    public GameObject pStart;
    public GameObject pConnect;
    public GameObject pStartServer;
    public GameObject pSinglePlayer;
    public GameObject mulityGroup;
    bool isPrepare = false;
    int port = 9970;

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
        mulityGroup.SetActive(false);
    }

    void Update()
    {
        if (Client.IsUse())
        {
            Client.instance.Update();

            // 代码待重构
            if (ServerNet.IsUse())
            {
                if (Client.instance.roomnum == 0) sInformation.text = "创建服务器失败";
                else
                {
                    sInformation.text = NetInteraction.GetServerMsg() + "\n" + "准备玩家：" + Client.instance.prepareNum + "/" + Client.instance.roomnum + " 全部准备或人数达到4时开始游戏！";
                }
            }
            else
            {
                if (Client.instance.roomnum == 0) cInformation.text = "连接服务器失败";
                else if (Client.instance.roomnum == 1) cInformation.text = "连接服务器失败";
                else
                {
                    cInformation.text = "准备玩家：" + Client.instance.prepareNum + "/" + Client.instance.roomnum + " 全部准备或人数达到4时开始游戏！";
                }
            }
            
            
        }
    }
    
    public string[] gameSceneName;
    public int nextSceneID;
    
    public void EnterGame()
    {
        SceneManager.LoadScene(gameSceneName[nextSceneID]);
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


    public void PlayerWithFriends()
    {
        mulityGroup.SetActive(true);
        pStart.SetActive(false);

        pConnect.SetActive(false);
        pStartServer.SetActive(false);
    }

    public void OnLANServerClick()
    {
        NetInteraction.StartLANServer();
        pStart.SetActive(false);
        pConnect.SetActive(false);
        pStartServer.SetActive(true);
        mulityGroup.SetActive(false);
    }

    public void OnWANServerClick()
    {
        NetInteraction.StartWANRoom();
        pStart.SetActive(false);
        pConnect.SetActive(false);
        pStartServer.SetActive(true);
        mulityGroup.SetActive(false);
    }

    // Find Bug
    public void OnReturnClick()
    {
        DisConnect();
        pStart.SetActive(true);
        pConnect.SetActive(false);
        pStartServer.SetActive(false);
        mulityGroup.SetActive(false);
    }

    //public void OnStartServerClick()
    //{
    //    StartLANServer();
    //    pStart.SetActive(false);
    //    pConnect.SetActive(false);
    //    pStartServer.SetActive(true);
    //    mulityGroup.SetActive(false);
    //}

}
