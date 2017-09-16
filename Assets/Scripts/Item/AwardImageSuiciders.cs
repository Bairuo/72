using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardImageSuiciders : MonoBehaviour {

    // Use this for initialization
    public float time;
    private float timer;
    public float speed = 1;
	void Start () {
        timer = time;
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);
        this.gameObject.transform.Translate(new Vector3(0, speed, 0));
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Destroy(this.gameObject);
        }
	}
}
