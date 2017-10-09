using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassDown : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().MassLevel > 1)
            {
                collision.gameObject.GetComponent<PlayerController>().MassLevel--;
            }
            Destroy(this.gameObject);
        }
        */
        
        /// Try something interesting , may remove or re-design.
        if(collision.gameObject.tag == "Player")
        {
            var c = collision.gameObject.GetComponent<BuffMassUp>();
            if(c != null) Destroy(c);
            Destroy(this.gameObject);
        }
    }
}
