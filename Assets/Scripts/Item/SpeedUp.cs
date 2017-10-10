using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour {
    public float speedSize;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if(collision.gameObject.tag == "Player")
        {
            if(collision.gameObject.GetComponent<PlayerController>().SpeedLevel < 4)
            {
                collision.gameObject.GetComponent<PlayerController>().SpeedLevel++;
            }
            Destroy(this.gameObject);
        }
        */
        
        /// Try something interesting , may remove or re-design.
        if(collision.gameObject.tag == "Player")
        {
            // do nothing if there are 4 same buffers exist.
            if(collision.gameObject.GetComponents<BuffSpeedUp>().Length >= 4) return;
            
            collision.gameObject.AddComponent<BuffSpeedUp>();
            Destroy(this.gameObject);
        }
    }
}
