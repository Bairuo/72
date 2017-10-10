using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassUp : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().MassLevel < 4)
            {
                collision.gameObject.GetComponent<PlayerController>().MassLevel++;
            }
            Destroy(this.gameObject);
        }
        */
        
        /// Try something interesting , may remove or re-design.
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.AddComponent<BuffMassUp>();
            Destroy(this.gameObject);
        }
    }
}
