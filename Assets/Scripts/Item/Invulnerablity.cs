using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invulnerablity : MonoBehaviour {
    public float time;
    private GameObject obj;
    private GameObject childObj;
    void Start()
    {
        childObj = this.gameObject.transform.Find("Inv").gameObject;
        childObj.GetComponent<InvController>().InvInit();
        childObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        obj = collision.gameObject;
        if (obj.tag == "Player")
        {
            childObj.SetActive(true);
            childObj.GetComponent<InvController>().InvStart(obj, time);
            Destroy(this.gameObject);
        }
    }

}
