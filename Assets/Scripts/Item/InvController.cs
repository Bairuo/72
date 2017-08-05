using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvController : MonoBehaviour {

    private float timer;
    private bool start;
    public bool tag = true;
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(start);
        if(start)
        {
            timer -= Time.deltaTime;
            //Debug.Log(timer);
            if(timer <= 0 && this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>().status == 1)
            {
                this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>().ChangeStatus(0);
                Destroy(this.gameObject);
            }
        }
	}

    private void FatherObjChanger(GameObject obj)
    {
        this.gameObject.transform.SetParent(obj.transform);
    }

    public void InvStart(GameObject obj, float time)
    {
        FatherObjChanger(obj);
        if (this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>().status != 1 && obj.transform.Find("Inv"))
            this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>().ChangeStatus(1);
        timer = time;
        start = true;
        //Debug.Log(start);
    }

    public void SetTimer(float time)
    {
        timer = time;
    }

    public void InvInit()
    {
        start = false;
    }

}
