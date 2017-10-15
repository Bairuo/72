using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExGameController : MonoBehaviour {
    public GameObject ExPrefab;
	// Use this for initialization
	void Start () {
        Vector3 pos = new Vector3(0,0,0);
        Client.instance.posmanager.Init(Client.instance.playerid);  // 原主场景中已包含这条语句
        
        // 非固定物体的id分配需要通过类似ExController中的协议发送来实现
        // 由构造器将id发送到另一个构造器同一id再创建
        GameObject Ex = GameObject.Instantiate(ExPrefab, pos, Quaternion.identity);
        Ex.GetComponent<NetObject>().ObjectRegister("1");
	}
	
	// Update is called once per frame
	void Update () {
        Client.instance.Update();
	}
}
