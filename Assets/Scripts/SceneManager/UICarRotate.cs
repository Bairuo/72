using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICarRotate : MonoBehaviour {

    Vector3 rotateaxis = new Vector3(0, 0, 1);
    public float RotateSpeed = 30;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotateaxis, RotateSpeed * Time.deltaTime);
	}
}
