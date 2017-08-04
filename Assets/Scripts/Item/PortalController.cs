using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {

    private float radius;
    public GameObject original;
    private GameObject obj;
    private Vector3 pos_2;

    public static int index = 0;
    public int PortalID;

	// Use this for initialization
	void Start ()
    {
        PortalID = index++;
        radius = GameObject.Find("saftyArea").GetComponent<SaftyArea>().radius;
        Vector2 pos_t = Random.insideUnitCircle * radius;

        if (Client.instance.playerid == "0")
        {
            Client.instance.SendPortalCreate(PortalID, pos_t);
            SetAnother(pos_t);
        }
	}

    public static void SetAnother(int id, Vector2 pos)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
        {
            PortalController t = item.GetComponent<PortalController>();
            if (t != null && t.PortalID == id)
            {
                t.SetAnother(pos);
            }
        }
    }

    public static void DestroyBoth(int id)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
        {
            PortalController t = item.GetComponent<PortalController>();
            if (t != null && t.PortalID == id)
            {
                t.DestroyBoth();
            }
        }
    }

    public void SetAnother(Vector2 pos_t)
    {
        obj = Object.Instantiate(original);
        obj.transform.position = pos_t;
        pos_2 = pos_t;
    }

    public void DestroyBoth()
    {
        Destroy(obj);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gbj = collision.gameObject;
        if(gbj.tag == "Player")
        {
            if (Client.instance.playerid == "0")
            {
                gbj.GetComponent<PlayerController>().ChangePosition(pos_2.x, pos_2.y, pos_2.z);
                Client.instance.SendPortalDestroy(PortalID);
                DestroyBoth();
            }
        }
    }
}
