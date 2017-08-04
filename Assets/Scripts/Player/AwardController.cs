using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardController : MonoBehaviour {

    public GameObject su;
    public GameObject sd;
    public GameObject mu;
    public GameObject md;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gbj = collision.gameObject;
        if (gbj.GetComponent<SpeedUp>())
        {
            GameObject obj = Object.Instantiate(su, this.gameObject.transform);
            obj.transform.Translate(0, 0, -1);
        }
        if (gbj.GetComponent<SlowDown>())
        {
            GameObject obj = Object.Instantiate(sd, this.gameObject.transform);
            obj.transform.Translate(0, 0, -1);
        }
        if (gbj.GetComponent<MassUp>())
        {
            GameObject obj = Object.Instantiate(mu, this.gameObject.transform);
            obj.transform.Translate(0, 0, -1);
        }
        if (gbj.GetComponent<MassDown>())
        {
            GameObject obj = Object.Instantiate(md, this.gameObject.transform);
            obj.transform.Translate(0, 0, -1);
        }
    }
}
