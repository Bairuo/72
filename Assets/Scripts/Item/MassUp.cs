using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassUp : ExNetworkBehaviour
{
    static int cnt;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!Client.IsRoomServer()) return;
        
        /// Try something interesting , may remove or re-design.
        if(collision.gameObject.tag == "Player")
        {
            if(collision.gameObject.GetComponents<BuffMassUp>().Length >= 4) return;
            ExComponentGenerator.instance.CreateComponentAt("BuffMassUp", collision.gameObject, "MassUp" + (cnt += 1));
            Destroy(this.gameObject);
        }
    }
}
