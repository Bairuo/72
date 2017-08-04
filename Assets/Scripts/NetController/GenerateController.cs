using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateController : MonoBehaviour {
    public static GenerateController instance;

    public GenerateController()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        float p = float.Parse(Client.instance.playerid) * 4;
        Client.instance.posmanager.Init(Client.instance.playerid);

        Client.instance.SendPlayerGenerate(p, p, Client.instance.playerid);
        CreatePlayer(p, p, Client.instance.playerid);

	}
	
	// Update is called once per frame
	void Update () {
        Client.instance.Update();
	}

    public void CreatePlayer(float x, float y, string PlayerID)
    {
        Vector3 pos = new Vector3(x, y, 0);
        GameObject prefab = Resources.Load("Player") as GameObject;
        GameObject Player = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        Player.GetComponent<PlayerController>().PlayerID = PlayerID;
        Player.GetComponent<PlayerController>().SetCir();

        if (PlayerID == Client.instance.playerid)
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.transform.SetParent(Player.transform);
        }

        Client.instance.posmanager.PlayerRegister(Player);
    }
}
