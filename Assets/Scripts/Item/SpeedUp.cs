using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour
{
    public float speedSize;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /// Try something interesting , may remove or re-design.
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.AddComponent<BuffSpeedUp>();
            Destroy(this.gameObject);
        }
    }
}
