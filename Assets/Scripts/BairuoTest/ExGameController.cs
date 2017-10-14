using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExGameController : MonoBehaviour {
    public GameObject ExPrefab;
	// Use this for initialization
	void Start () {
        Vector3 pos = new Vector3(0,0,0);
        Client.instance.posmanager.Init(Client.instance.playerid);
        GameObject Ex = GameObject.Instantiate(ExPrefab, pos, Quaternion.identity);
        Ex.GetComponent<NetObject>().ObjectRegister("1");
        //Ex.GetComponent<ExController>().foo();
	}
	
	// Update is called once per frame
	void Update () {
        Client.instance.Update();
	}
}
