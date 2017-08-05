using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenView : MonoBehaviour {
    public float time;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"  && collision.gameObject.GetComponent<PlayerController>().IsMine())
        {
            GameObject.FindWithTag("fog").GetComponent<FogController>().MakeVisible(time);
        }
        Destroy(this.gameObject);
    }
}
