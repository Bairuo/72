using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CreatePlayer(0, 0, Client.instance.playerid);
        //CreatePlayer(0, 0, "0");
	}
	
	// Update is called once per frame
	void Update () {
        if (Client.IsUse())
        {
            Client.instance.Update();
        }	
	}

    public void CreatePlayer(float x, float y, string PlayerID)
    {
        Vector3 pos = new Vector3(x, y, 0);
        GameObject prefab = Resources.Load("Player") as GameObject;
        GameObject Player = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        Player.GetComponent<PlayerController>().PlayerID = PlayerID;

        if (PlayerID == Client.instance.playerid)
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.transform.SetParent(Player.transform);
        }

        Client.instance.posmanager.PlayerRegister(Player);
    }
}
