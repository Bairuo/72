using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {

    private float radius;
    public GameObject original;
    private GameObject obj;
    private Vector3 pos_2;

	// Use this for initialization
	void Start ()
    {
        radius = GameObject.Find("safety-area").GetComponent<SaftyArea>().radius;
        Vector2 pos_t = Random.insideUnitCircle * radius;
        pos_2 = pos_t;
        obj = Object.Instantiate(original);
        obj.transform.position = pos_2;
	}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gbj = collision.gameObject;
        if(gbj.tag == "Player")
        {
            gbj.GetComponent<PlayerController>().ChangePosition(pos_2.x, pos_2.y, pos_2.z);
            Destroy(obj);
            Destroy(this.gameObject);
        }
    }
}
