using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invulnerablity : MonoBehaviour {
    public float time;
    private GameObject obj;
    private GameObject childObj;
<<<<<<< HEAD
    public bool _tag = true;
=======
    private bool _tag = true;
>>>>>>> f9f1af245a7e51703bdf606dde13d4eb609c4d93
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
            if(!obj.transform.Find("Inv"))
            {
<<<<<<< HEAD
                childObj.SetActive(true);
                if(_tag)
                {
=======
                if(_tag)
                {
                    childObj.SetActive(true);
>>>>>>> f9f1af245a7e51703bdf606dde13d4eb609c4d93
                    childObj.GetComponent<InvController>().InvStart(obj, time);
                    _tag = false;
                }
            }
            else
            {
                obj.transform.Find("Inv").gameObject.GetComponent<InvController>().SetTimer(time);
            }
            Destroy(this.gameObject);
        }
    }

}
