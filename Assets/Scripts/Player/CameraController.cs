using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    Quaternion t;
    Vector3 offset;
    GameObject player;
	// Use this for initialization

    public void Init(GameObject player)
    {
        this.player = player;
        offset = transform.position;
    }

	void Start () {
        t = transform.rotation;
        offset = transform.position;
	}

	// Update is called once per frame
	void Update () {
        
        //transform.localRotation = t;
        //Debug.Log(t);
	}

    void LateUpdate()
    {
        transform.rotation = t;
        //transform.position =  player.transform.position + offset;
    }
}
