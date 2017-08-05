using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardImageSuiciders : MonoBehaviour {

    // Use this for initialization
    public float time;
    private float timer;
    
	void Start () {
        timer = time;
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Destroy(this.gameObject);
        }
	}
}
