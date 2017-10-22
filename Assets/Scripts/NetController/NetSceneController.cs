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
    public Text joinInfo;
    public GameObject pStart;
    public GameObject pConnect;
    public GameObject pStartServer;
    public GameObject pSinglePlayer;
    public GameObject mulityGroup;
    bool isPrepare = false;
    int port = 9970;

    enum PrepareMode
    {
        None,
        LANJoin,
        WANJoin
    }

    PrepareMode prepareMode = PrepareMode.None;

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
        // 代码可能得重构
        if (Client.IsUse())
        {
            Client.instance.Update();

            
            if (NetInteraction.IsStartServer() || NetInteraction.IsWANRandom())
            {
                if (Client.instance.roomnum == 0) sInformation.text = "创建服务器失败";
                else
                {
                    sInformation.text = NetInteraction.GetServerInfoMsg() + "\n" + "准备玩家：" + Client.instance.prepareNum + "/" + Client.instance.roomnum + " 全部准备或人数达到4时开始游戏！";
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
        joinInfo.text = GetConnectPromptMsg();
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

    string GetConnectPromptMsg()
    {
        switch (prepareMode)
        {
            case PrepareMode.None:
                return "";
            case PrepareMode.LANJoin:
                return "请输入服务器IP";
            case PrepareMode.WANJoin:
                return "请输入房间号";
            default:
                return "";
        }
    }

    public void StartConnect()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            SendPrepare();
            return;
        }

        if (prepareMode == PrepareMode.LANJoin)
            NetInteraction.ConnectLANServer(IPInput.text, port);
        else if (prepareMode == PrepareMode.WANJoin)
            NetInteraction.ConnectWANRoom(IPInput.text);
    }

    public void DisConnect()
    {
        if (ServerNet.IsUse())
            ServerNet.instance.Close();
        if (Client.IsUse())
            Client.instance.Close();
    }

    // 感觉这么多false，true很丑，以后又有多的面板怎么办，逻辑也很混乱——fyl去重构吧
    public void OnJoinLANRoomClick()
    {
        prepareMode = PrepareMode.LANJoin;
        pStart.SetActive(false);
        pConnect.SetActive(true);
        pStartServer.SetActive(false);
        mulityGroup.SetActive(false);
    }

    public void OnJoinWANRoomClick()
    {
        prepareMode = PrepareMode.WANJoin;
        pStart.SetActive(false);
        pConnect.SetActive(true);
        pStartServer.SetActive(false);
        mulityGroup.SetActive(false);
    }

    public void OnWANRandomClick()
    {
        NetInteraction.ConnectWANRadom();
        pStart.SetActive(false);
        pConnect.SetActive(false);
        pStartServer.SetActive(true);
        mulityGroup.SetActive(false);
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
