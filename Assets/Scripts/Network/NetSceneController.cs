using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetSceneController : MonoBehaviour {
    public Text IPInput; 
    public Text Information;
    int port = 9990;


    void Start()
    {

    }

    public void StartServer()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return;
        }

        ServerNet server = new ServerNet();
        server.Start(Network.player.ipAddress, port);

        Information.text = Network.player.ipAddress.ToString();
    }

    public void StartConnect()
    {
        if (ServerNet.IsUse() || Client.IsUse())
        {
            return;
        }

        Client client = new Client();

        if (client.Connect(IPInput.text, port) == false)
        {
            
        }

    }

    public void DisConnect()
    {

    }
}
