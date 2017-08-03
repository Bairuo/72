using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformController : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown("w"))
        {
            obj.transform.Translate(0, 0.1f, 0);
        }
        if (Input.GetKeyDown("a"))
        {
            obj.transform.Translate(0.1f, 0, 0);
        }
        if (Input.GetKeyDown("s"))
        {
            obj.transform.Translate(0, -0.1f, 0);
        }
        if (Input.GetKeyDown("d"))
        {
            obj.transform.Translate(-0.1f, 0, 0);
        }
    }
}
