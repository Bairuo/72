// Created by DK 2017/10/15


using UnityEngine;



public class Invulnerable : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D x)
    {
        var o = x.gameObject;
        if(o.GetComponent<Unit>() != null)
        {
            if(o.GetComponent<BuffInvulnerable>() != null) return;
            o.AddComponent<BuffInvulnerable>();
            Destroy(this.gameObject);
        }
    }
}