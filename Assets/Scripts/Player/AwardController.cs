using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardController : MonoBehaviour {

    public GameObject su;
    public GameObject sd;
    public GameObject mu;
    public GameObject md;

    public float time;
    private float timer;

    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gbj = collision.gameObject;
        if(gbj.GetComponent<SpeedUp>())
        {
            GameObject obj = Object.Instantiate(su,this.gameObject.transform);
            obj.transform.Translate(0, 1f, 0);
        }
    }
}
