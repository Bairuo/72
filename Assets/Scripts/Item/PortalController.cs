using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {

    private float radius;
    private float areaMulti;
    public GameObject original;
    private GameObject obj;
    private Vector3 pos_2;

	// Use this for initialization
	void Start ()
    {
        //radius = GameObject.Find("safety-area").GetComponent<SaftyArea>().radius;
        //areaMulti = GameObject.Find("ObjectGenerator").GetComponent<ObjectGenerator>().areaMulti;
        radius = 10f;
        areaMulti = 0.9f;
        Vector2 pos_t = Random.insideUnitCircle * radius * areaMulti;
        pos_2 = pos_t;
        obj = Object.Instantiate(original);
        obj.transform.position = pos_2;
	}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gbj = collision.gameObject;
        if(gbj.tag == "Player")
        {
            gbj.transform.position = pos_2;
            Destroy(obj);
            Destroy(this.gameObject);
        }
    }
}
