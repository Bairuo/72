using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!Client.IsRoomServer()) return;
        
        /// Try something interesting , may remove or re-design.
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.AddComponent<BuffSpeedUp>();
            Destroy(this.gameObject);
        }
    }
}
